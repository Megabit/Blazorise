import { getRequiredElement, registerDisconnectCleanup, unregisterDisconnectCleanup } from "../Blazorise/utilities.js?v=2.2.2.0";

const instances = new Map();
const languageOwners = new Map();
const languageRegistry = new Map();
const stylesheetUrls = new Set();
let loaderPromise = null;
let runtimeAssetsPath = null;

export async function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    await ensureEditorRuntime(options);

    if (instances.has(elementId)) {
        destroy(element, elementId);
    }

    synchronizeLanguages(elementId, options.languages);

    const editorOptions = buildEditorOptions(options);

    try {
        let updating = false;
        const editor = monaco.editor.create(element, editorOptions);
        const instance = {
            dotNetAdapter,
            editor,
            element,
            options,
            disposables: [],
            completionDisposable: null,
            formattingDisposable: null,
            markerOwner: `blazorise-code-editor-${elementId}`,
            pendingValue: undefined,
            valueUpdateTimer: null,
            valueUpdatePromise: Promise.resolve(),
            ownsModel: !editorOptions.model,
            setUpdating: (value) => updating = value
        };

        instances.set(elementId, instance);

        instance.disposables.push(editor.onDidChangeModelContent(() => {
            if (!updating) {
                queueValueChange(instance);
            }
        }));

        instance.disposables.push(editor.onDidFocusEditorWidget(() => {
            invokeDotNet(instance, "OnEditorFocus");
        }));

        instance.disposables.push(editor.onDidBlurEditorWidget(async () => {
            await flushValueChange(instance);
            await invokeDotNet(instance, "OnEditorBlur");
        }));

        applyAccessibility(instance);
        registerCompletionProvider(instance, options.completionProvider || { language: options.language });
        registerFormattingProvider(instance, options.formattingProvider);

        instance.disconnectCleanupId = registerDisconnectCleanup(element, () => destroy(element, elementId, false));
    } catch (error) {
        if (instances.has(elementId)) {
            destroy(element, elementId, false);
        } else {
            releaseLanguages(elementId);
        }

        throw error;
    }
}

export function destroy(element, elementId, unregisterCleanup = true) {
    const instance = instances.get(elementId);

    if (!instance)
        return;

    if (unregisterCleanup) {
        unregisterDisconnectCleanup(instance.disconnectCleanupId);
    }

    if (instance.disposables) {
        instance.disposables.forEach(disposable => disposable?.dispose?.());
    }

    instance.completionDisposable?.dispose?.();
    instance.formattingDisposable?.dispose?.();
    clearTimeout(instance.valueUpdateTimer);

    if (instance.editor) {
        const model = instance.editor.getModel();

        if (model) {
            monaco.editor.setModelMarkers(model, instance.markerOwner, []);
        }

        instance.editor.dispose();

        if (instance.ownsModel) {
            model?.dispose?.();
        }
    }

    releaseLanguages(elementId);
    instances.delete(elementId);
}

export function updateOptions(element, elementId, options) {
    const instance = instances.get(elementId);

    if (!instance?.editor)
        return;

    const previousOptions = instance.options;
    const hasPendingValue = instance.pendingValue !== undefined;
    const shouldFlushValue = hasPendingValue
        && ((previousOptions?.immediate === false && options.immediate !== false)
            || (previousOptions?.debounce === true && options.debounce !== true));

    instance.options = options;
    instance.editor.updateOptions(buildEditorOptions(options, false));
    instance.editor.getModel()?.updateOptions(buildModelOptions(options));
    applyAccessibility(instance);

    if (hasPendingValue && options.immediate === false) {
        clearTimeout(instance.valueUpdateTimer);
        instance.valueUpdateTimer = null;
    } else if (shouldFlushValue) {
        flushValueChange(instance);
    } else if (hasPendingValue
        && options.debounce === true
        && previousOptions?.debounceInterval !== options.debounceInterval) {
        clearTimeout(instance.valueUpdateTimer);
        instance.valueUpdateTimer = setTimeout(
            () => flushValueChange(instance),
            Math.max(0, options.debounceInterval || 0));
    }
}

export function setLanguages(element, elementId, languages) {
    if (instances.has(elementId)) {
        synchronizeLanguages(elementId, languages);
    }
}

export function setCompletionProvider(element, elementId, completionProvider) {
    const instance = instances.get(elementId);

    if (!instance?.editor)
        return;

    registerCompletionProvider(instance, completionProvider);
}

export function setFormattingProvider(element, elementId, formattingProvider) {
    const instance = instances.get(elementId);

    if (!instance?.editor)
        return;

    registerFormattingProvider(instance, formattingProvider);
}

export function setDiagnostics(element, elementId, diagnostics) {
    const instance = instances.get(elementId);

    if (!instance?.editor)
        return;

    const model = instance.editor.getModel();

    if (!model)
        return;

    const markers = Array.isArray(diagnostics)
        ? diagnostics.map(toMarker)
        : [];

    monaco.editor.setModelMarkers(model, instance.markerOwner, markers);
}

export function getDiagnostics(element, elementId) {
    const model = instances.get(elementId)?.editor?.getModel();

    if (!model)
        return [];

    return monaco.editor.getModelMarkers({ resource: model.uri }).map(toDiagnostic);
}

export function setValue(element, elementId, value) {
    const instance = instances.get(elementId);

    if (!instance?.editor)
        return;

    value ??= "";

    if (instance.editor.getValue() === value)
        return;

    clearTimeout(instance.valueUpdateTimer);
    instance.valueUpdateTimer = null;
    instance.pendingValue = undefined;
    instance.setUpdating(true);

    try {
        instance.editor.setValue(value);
    } finally {
        instance.setUpdating(false);
    }
}

export function getValue(element, elementId) {
    const instance = instances.get(elementId);

    return instance?.editor?.getValue() ?? "";
}

export function focus(element, elementId) {
    const instance = instances.get(elementId);

    if (!instance?.options?.disabled) {
        instance?.editor?.focus();
    }
}

export function layout(element, elementId) {
    const instance = instances.get(elementId);

    instance?.editor?.layout();
}

export async function formatDocument(element, elementId) {
    const instance = instances.get(elementId);
    const action = instance?.editor?.getAction("editor.action.formatDocument");

    if (!action || (typeof action.isSupported === "function" && !action.isSupported()))
        return false;

    await action.run();

    return true;
}

export function revealLine(element, elementId, lineNumber) {
    const instance = instances.get(elementId);

    instance?.editor?.revealLineInCenter(Math.max(1, lineNumber || 1));
}

export function setLanguage(element, elementId, language) {
    const instance = instances.get(elementId);

    if (!instance?.editor || !language)
        return;

    const model = instance.editor.getModel();

    if (model) {
        monaco.editor.setModelLanguage(model, language);
    }
}

export function setTheme(element, elementId, theme) {
    if (theme) {
        monaco.editor.setTheme(theme);
    }
}

export function setSelection(element, elementId, selection) {
    const instance = instances.get(elementId);

    if (!instance?.editor || !selection)
        return;

    instance.editor.setSelection(new monaco.Selection(
        Math.max(1, selection.startLineNumber || 1),
        Math.max(1, selection.startColumn || 1),
        Math.max(1, selection.endLineNumber || selection.startLineNumber || 1),
        Math.max(1, selection.endColumn || selection.startColumn || 1)));
}

export function getSelection(element, elementId) {
    const instance = instances.get(elementId);
    const selection = instance?.editor?.getSelection();

    if (!selection)
        return null;

    return {
        startLineNumber: selection.startLineNumber,
        startColumn: selection.startColumn,
        endLineNumber: selection.endLineNumber,
        endColumn: selection.endColumn
    };
}

async function ensureEditorRuntime(options) {
    const assetsPath = normalizeAssetsPath(options.assetsPath);

    if (runtimeAssetsPath && runtimeAssetsPath !== assetsPath) {
        console.warn(`Blazorise CodeEditor has already loaded Monaco from '${runtimeAssetsPath}'. The requested path '${assetsPath}' cannot be applied after initialization.`);
    }

    if (window.monaco?.editor)
        return;

    if (loaderPromise)
        return await loaderPromise;

    runtimeAssetsPath = assetsPath;
    ensureStylesheet(`${assetsPath}/editor/editor.main.css`);

    loaderPromise = loadScript(`${assetsPath}/loader.js`)
        .then(() => new Promise((resolve, reject) => {
            window.require.config({ paths: { vs: assetsPath } });
            window.require(["vs/editor/editor.main"], resolve, reject);
        }));

    try {
        await loaderPromise;
    } catch (error) {
        loaderPromise = null;
        runtimeAssetsPath = null;
        throw error;
    }
}

function buildEditorOptions(options, includeValue = true) {
    const editorOptions = options.additionalOptions
        ? { ...options.additionalOptions }
        : {};
    const minimapOptions = editorOptions.minimap && typeof editorOptions.minimap === "object"
        ? { ...editorOptions.minimap }
        : {};

    Object.assign(editorOptions, {
        readOnly: options.readOnly === true || options.disabled === true,
        domReadOnly: options.readOnly === true || options.disabled === true,
        automaticLayout: options.automaticLayout !== false,
        minimap: Object.assign(minimapOptions, { enabled: options.minimap !== false }),
        lineNumbers: options.lineNumbers === false ? "off" : "on",
        wordWrap: options.wordWrap === true ? "on" : "off",
        formatOnPaste: options.formatOnPaste === true,
        formatOnType: options.formatOnType === true,
        renderWhitespace: options.renderWhitespace === true ? "all" : "none",
        scrollBeyondLastLine: options.scrollBeyondLastLine !== false,
        tabIndex: options.disabled === true ? -1 : (options.tabIndex ?? 0),
        ariaRequired: options.ariaRequired === "true"
    });

    if (includeValue) {
        editorOptions.value = options.value ?? "";
        editorOptions.language = options.language || "plaintext";
        editorOptions.theme = options.theme || "vs";
        Object.assign(editorOptions, buildModelOptions(options));
    } else {
        delete editorOptions.value;
        delete editorOptions.language;
        delete editorOptions.theme;
        delete editorOptions.model;
    }

    if (options.fontFamily) {
        editorOptions.fontFamily = options.fontFamily;
    }

    if (typeof options.fontSize === "number") {
        editorOptions.fontSize = options.fontSize;
    }

    return editorOptions;
}

function buildModelOptions(options) {
    return {
        tabSize: Math.max(1, options.tabSize || 4),
        insertSpaces: options.insertSpaces !== false
    };
}

function synchronizeLanguages(ownerId, languages) {
    const previousLanguages = languageOwners.get(ownerId) || new Map();
    const nextLanguages = new Map();

    if (Array.isArray(languages)) {
        for (const language of languages) {
            if (language?.id) {
                nextLanguages.set(language.id, language);
            }
        }
    }

    for (const languageId of previousLanguages.keys()) {
        if (!nextLanguages.has(languageId)) {
            removeLanguageOwner(ownerId, languageId);
        }
    }

    for (const [languageId, language] of nextLanguages) {
        setLanguageOwner(ownerId, languageId, language);
    }

    if (nextLanguages.size > 0) {
        languageOwners.set(ownerId, nextLanguages);
    } else {
        languageOwners.delete(ownerId);
    }
}

function releaseLanguages(ownerId) {
    const ownedLanguages = languageOwners.get(ownerId);

    if (!ownedLanguages)
        return;

    for (const languageId of ownedLanguages.keys()) {
        removeLanguageOwner(ownerId, languageId);
    }

    languageOwners.delete(ownerId);
}

function setLanguageOwner(ownerId, languageId, language) {
    let entry = languageRegistry.get(languageId);

    if (!entry) {
        entry = {
            owners: new Map(),
            registrationDisposable: null,
            tokenizerDisposable: null,
            configurationDisposable: null,
            activeSignature: null
        };

        languageRegistry.set(languageId, entry);
    }

    entry.owners.delete(ownerId);
    entry.owners.set(ownerId, language);
    applyEffectiveLanguage(languageId, entry);
}

function removeLanguageOwner(ownerId, languageId) {
    const entry = languageRegistry.get(languageId);

    if (!entry)
        return;

    entry.owners.delete(ownerId);

    if (entry.owners.size === 0) {
        disposeLanguageEntry(entry);
        languageRegistry.delete(languageId);
        return;
    }

    applyEffectiveLanguage(languageId, entry);
}

function applyEffectiveLanguage(languageId, entry) {
    const ownedLanguages = Array.from(entry.owners.values());
    const language = ownedLanguages[ownedLanguages.length - 1];
    const signature = JSON.stringify(language);

    if (entry.activeSignature === signature)
        return;

    if (new Set(ownedLanguages.map(value => JSON.stringify(value))).size > 1) {
        console.warn(`Blazorise CodeEditor received conflicting definitions for the global Monaco language '${languageId}'. The most recently updated definition is active.`);
    }

    disposeLanguageEntry(entry);

    entry.registrationDisposable = monaco.languages.register({
        id: language.id,
        aliases: language.aliases || undefined,
        extensions: language.extensions || undefined,
        mimetypes: language.mimeTypes || undefined
    });

    if (language.tokenizer) {
        entry.tokenizerDisposable = monaco.languages.setMonarchTokensProvider(
            language.id,
            buildMonarchTokensProvider(language.tokenizer));
    }

    if (language.configureLanguageMethod) {
        let disposable;

        try {
            disposable = configure(language.configureLanguageMethod, window, [language, monaco]);
        } catch (error) {
            console.error(error);
        }

        if (disposable?.dispose) {
            entry.configurationDisposable = disposable;
        }
    }

    entry.activeSignature = signature;
}

function disposeLanguageEntry(entry) {
    entry.configurationDisposable?.dispose?.();
    entry.tokenizerDisposable?.dispose?.();
    entry.registrationDisposable?.dispose?.();
    entry.configurationDisposable = null;
    entry.tokenizerDisposable = null;
    entry.registrationDisposable = null;
    entry.activeSignature = null;
}

function buildMonarchTokensProvider(tokenizer) {
    const states = {};

    if (Array.isArray(tokenizer.tokens)) {
        states.root = tokenizer.tokens.map(toMonarchRule).filter(rule => rule);
    }

    if (tokenizer.states && typeof tokenizer.states === "object") {
        for (const [stateName, tokens] of Object.entries(tokenizer.states)) {
            if (stateName && Array.isArray(tokens)) {
                states[stateName] = tokens.map(toMonarchRule).filter(rule => rule);
            }
        }
    }

    states.root ??= [];

    const provider = {
        tokenizer: states
    };

    if (tokenizer.defaultToken) {
        provider.defaultToken = tokenizer.defaultToken;
    }

    if (tokenizer.ignoreCase === true) {
        provider.ignoreCase = true;
    }

    if (tokenizer.unicode === true) {
        provider.unicode = true;
    }

    return provider;
}

function toMonarchRule(token) {
    if (!token?.pattern)
        return null;

    const action = {};

    if (token.token) {
        action.token = token.token;
    }

    if (token.next) {
        action.next = token.next;
    }

    if (token.bracket) {
        action.bracket = token.bracket;
    }

    if (!action.token && (action.next || action.bracket)) {
        action.token = "";
    }

    return [
        token.pattern,
        Object.keys(action).length === 0
            ? ""
            : (Object.keys(action).length === 1 && action.token ? action.token : action)
    ];
}

function registerCompletionProvider(instance, completionProvider) {
    instance.completionDisposable?.dispose?.();
    instance.completionDisposable = null;

    if (!completionProvider)
        return;

    const language = completionProvider.language || instance.editor?.getModel()?.getLanguageId?.();
    const hasItems = Array.isArray(completionProvider.items) && completionProvider.items.length > 0;
    const hasItemsProvider = completionProvider.useItemsProvider === true;

    if (!language || (!hasItems && !hasItemsProvider))
        return;

    instance.completionDisposable = monaco.languages.registerCompletionItemProvider(language, {
        triggerCharacters: completionProvider.triggerCharacters || undefined,
        provideCompletionItems: async (model, position, context, cancellationToken) => {
            if (model !== instance.editor?.getModel())
                return { suggestions: [] };

            const suggestions = hasItems
                ? completionProvider.items.map(item => toCompletionItem(item, model, position))
                : [];

            if (hasItemsProvider && !cancellationToken.isCancellationRequested) {
                const contextualItems = await invokeDotNet(
                    instance,
                    "NotifyCompletion",
                    createCompletionContext(model, position, context));

                if (!cancellationToken.isCancellationRequested && Array.isArray(contextualItems)) {
                    suggestions.push(...contextualItems.map(item => toCompletionItem(item, model, position)));
                }
            }

            return { suggestions };
        }
    });
}

function registerFormattingProvider(instance, formattingProvider) {
    instance.formattingDisposable?.dispose?.();
    instance.formattingDisposable = null;

    if (!formattingProvider)
        return;

    const model = instance.editor?.getModel();
    const language = formattingProvider.language || model?.getLanguageId?.();

    if (!model || !language || (!formattingProvider.useFormatter && !formattingProvider.providerMethod))
        return;

    const selector = {
        language,
        scheme: model.uri.scheme,
        pattern: model.uri.path
    };

    instance.formattingDisposable = monaco.languages.registerDocumentFormattingEditProvider(selector, {
        provideDocumentFormattingEdits: async (formattingModel, options, cancellationToken) => {
            if (formattingModel !== instance.editor?.getModel() || cancellationToken.isCancellationRequested)
                return [];

            try {
                let result;

                if (formattingProvider.useFormatter) {
                    result = await invokeDotNet(instance, "NotifyDocumentFormatting", formattingModel.getValue());
                } else if (formattingProvider.providerMethod) {
                    result = configure(
                        formattingProvider.providerMethod,
                        window,
                        [instance.editor, formattingModel, options, cancellationToken]);

                    if (result?.then) {
                        result = await result;
                    }
                }

                return normalizeDocumentFormattingResult(result, formattingModel);
            } catch (error) {
                console.error(error);
                return [];
            }
        }
    });
}

function normalizeDocumentFormattingResult(result, model) {
    if (typeof result === "string") {
        return result === model.getValue()
            ? []
            : [{ range: model.getFullModelRange(), text: result }];
    }

    return Array.isArray(result)
        ? result
        : [];
}

function toCompletionItem(item, model, position) {
    const word = model.getWordUntilPosition(position);
    const insertText = item.insertText || item.label || "";
    const completionItem = {
        label: item.label || insertText,
        kind: item.kind ?? monaco.languages.CompletionItemKind.Text,
        insertText,
        range: toCompletionRange(item.range, position, word)
    };

    if (item.detail) {
        completionItem.detail = item.detail;
    }

    if (item.documentation) {
        completionItem.documentation = item.documentation;
    }

    if (item.filterText) {
        completionItem.filterText = item.filterText;
    }

    if (item.sortText) {
        completionItem.sortText = item.sortText;
    }

    if (Array.isArray(item.commitCharacters)) {
        completionItem.commitCharacters = item.commitCharacters;
    }

    if (item.insertTextRules) {
        completionItem.insertTextRules = item.insertTextRules;
    }

    return completionItem;
}

function toCompletionRange(range, position, word) {
    if (!range) {
        return {
            startLineNumber: position.lineNumber,
            startColumn: word.startColumn,
            endLineNumber: position.lineNumber,
            endColumn: word.endColumn
        };
    }

    const startLineNumber = Math.max(1, range.startLineNumber || position.lineNumber);
    const startColumn = Math.max(1, range.startColumn || word.startColumn);
    const endLineNumber = Math.max(startLineNumber, range.endLineNumber || startLineNumber);
    const endColumn = endLineNumber === startLineNumber
        ? Math.max(startColumn, range.endColumn || position.column)
        : Math.max(1, range.endColumn || position.column);

    return {
        startLineNumber,
        startColumn,
        endLineNumber,
        endColumn
    };
}

function createCompletionContext(model, position, context) {
    const word = model.getWordUntilPosition(position);

    return {
        value: model.getValue(),
        lineText: model.getLineContent(position.lineNumber),
        lineNumber: position.lineNumber,
        column: position.column,
        word: word.word || "",
        triggerCharacter: context?.triggerCharacter || null
    };
}

function toMarker(diagnostic) {
    const startLineNumber = Math.max(1, diagnostic.startLineNumber || 1);
    const startColumn = Math.max(1, diagnostic.startColumn || 1);
    const endLineNumber = Math.max(startLineNumber, diagnostic.endLineNumber || startLineNumber);
    const endColumn = endLineNumber === startLineNumber
        ? Math.max(startColumn, diagnostic.endColumn || startColumn)
        : Math.max(1, diagnostic.endColumn || 1);

    return {
        severity: diagnostic.severity ?? monaco.MarkerSeverity.Error,
        message: diagnostic.message || "",
        code: diagnostic.code || undefined,
        startLineNumber,
        startColumn,
        endLineNumber,
        endColumn
    };
}

function toDiagnostic(marker) {
    return {
        severity: marker.severity,
        message: marker.message || "",
        code: typeof marker.code === "object"
            ? marker.code?.value
            : marker.code?.toString(),
        startLineNumber: marker.startLineNumber,
        startColumn: marker.startColumn,
        endLineNumber: marker.endLineNumber,
        endColumn: marker.endColumn
    };
}

function queueValueChange(instance) {
    const value = instance.editor?.getValue() ?? "";

    if (instance.options?.immediate === false) {
        instance.pendingValue = value;
        clearTimeout(instance.valueUpdateTimer);
        instance.valueUpdateTimer = null;
        return;
    }

    if (instance.options?.debounce !== true) {
        sendValueChange(instance, value);
        return;
    }

    instance.pendingValue = value;
    clearTimeout(instance.valueUpdateTimer);
    instance.valueUpdateTimer = setTimeout(
        () => flushValueChange(instance),
        Math.max(0, instance.options.debounceInterval || 0));
}

function flushValueChange(instance) {
    clearTimeout(instance.valueUpdateTimer);
    instance.valueUpdateTimer = null;

    if (instance.pendingValue === undefined) {
        return instance.valueUpdatePromise;
    }

    const value = instance.pendingValue;
    instance.pendingValue = undefined;

    return sendValueChange(instance, value);
}

function sendValueChange(instance, value) {
    instance.valueUpdatePromise = instance.valueUpdatePromise
        .then(() => invokeDotNet(instance, "UpdateInternalValue", value));

    return instance.valueUpdatePromise;
}

function invokeDotNet(instance, method, ...args) {
    return instance.dotNetAdapter.invokeMethodAsync(method, ...args)
        .catch(error => {
            console.error(error);
        });
}

function applyAccessibility(instance) {
    const input = instance.element.querySelector("textarea.inputarea");

    if (!input)
        return;

    input.setAttribute("aria-disabled", instance.options.disabled === true ? "true" : "false");
    input.setAttribute("aria-readonly", instance.options.readOnly === true || instance.options.disabled === true ? "true" : "false");
    setOptionalAttribute(input, "aria-invalid", instance.options.ariaInvalid);
    setOptionalAttribute(input, "aria-required", instance.options.ariaRequired);
    setOptionalAttribute(input, "aria-describedby", instance.options.ariaDescribedBy);
    setOptionalAttribute(input, "aria-labelledby", instance.options.ariaLabelledBy);

    const ariaLabel = instance.element.getAttribute("aria-label");

    if (ariaLabel) {
        input.setAttribute("aria-label", ariaLabel);
    }
}

function setOptionalAttribute(element, name, value) {
    if (value === null || value === undefined || value === "") {
        element.removeAttribute(name);
        return;
    }

    element.setAttribute(name, value);
}

function normalizeAssetsPath(assetsPath) {
    assetsPath ||= "_content/Blazorise.CodeEditor/vendors/monaco/min/vs";

    return assetsPath.endsWith("/")
        ? assetsPath.substring(0, assetsPath.length - 1)
        : assetsPath;
}

function ensureStylesheet(href) {
    if (stylesheetUrls.has(href))
        return;

    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = href;
    document.head.appendChild(link);

    stylesheetUrls.add(href);
}

function loadScript(src) {
    const existing = document.querySelector(`script[data-blazorise-code-editor-loader="${src}"]`);

    if (existing) {
        if (existing.dataset.failed === "true") {
            existing.remove();
            return loadScript(src);
        }

        return new Promise((resolve, reject) => {
            if (existing.dataset.loaded === "true") {
                resolve();
                return;
            }

            existing.addEventListener("load", resolve, { once: true });
            existing.addEventListener("error", error => {
                existing.dataset.failed = "true";
                reject(error);
            }, { once: true });
        });
    }

    return new Promise((resolve, reject) => {
        const script = document.createElement("script");
        script.src = src;
        script.async = true;
        script.dataset.blazoriseCodeEditorLoader = src;
        script.addEventListener("load", () => {
            script.dataset.loaded = "true";
            resolve();
        }, { once: true });
        script.addEventListener("error", error => {
            script.dataset.failed = "true";
            reject(error);
        }, { once: true });
        document.head.appendChild(script);
    });
}

function configure(functionName, context, args) {
    if (!functionName)
        return;

    const namespaces = functionName.split(".");
    const func = namespaces.pop();

    for (const namespace of namespaces) {
        context = context?.[namespace];

        if (!context)
            throw new Error(`Unable to find JavaScript namespace '${namespace}' while resolving '${functionName}'.`);
    }

    const callback = context?.[func];

    if (typeof callback !== "function")
        throw new Error(`Unable to find JavaScript function '${functionName}'.`);

    return callback.apply(context, args);
}