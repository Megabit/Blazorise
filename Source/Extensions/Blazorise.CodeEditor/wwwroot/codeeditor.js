import { getRequiredElement, registerDisconnectCleanup, unregisterDisconnectCleanup } from "../Blazorise/utilities.js?v=2.1.3.0";

const instances = {};
const stylesheetUrls = new Set();
let loaderPromise = null;

export async function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    await ensureEditorRuntime(options);

    const editorOptions = buildEditorOptions(options);

    if (options.configureEditorMethod) {
        try {
            configure(options.configureEditorMethod, window, [editorOptions]);
        } catch (err) {
            console.error(err);
        }
    }

    let updating = false;
    const editor = monaco.editor.create(element, editorOptions);
    const disposables = [];

    disposables.push(editor.onDidChangeModelContent(() => {
        if (updating)
            return;

        dotNetAdapter.invokeMethodAsync("UpdateInternalValue", editor.getValue());
    }));

    disposables.push(editor.onDidFocusEditorWidget(() => {
        dotNetAdapter.invokeMethodAsync("OnEditorFocus");
    }));

    disposables.push(editor.onDidBlurEditorWidget(() => {
        dotNetAdapter.invokeMethodAsync("OnEditorBlur");
    }));

    instances[elementId] = {
        dotNetAdapter,
        editor,
        element,
        disposables,
        markerOwner: `blazorise-code-editor-${elementId}`,
        setUpdating: (value) => updating = value
    };

    instances[elementId].disconnectCleanupId = registerDisconnectCleanup(element, () => destroy(element, elementId, false));
}

export function destroy(element, elementId, unregisterCleanup = true) {
    const instance = instances[elementId];

    if (!instance)
        return;

    if (unregisterCleanup) {
        unregisterDisconnectCleanup(instance.disconnectCleanupId);
    }

    if (instance.disposables) {
        instance.disposables.forEach(disposable => disposable?.dispose?.());
    }

    if (instance.editor) {
        const model = instance.editor.getModel();
        monaco.editor.setModelMarkers(model, instance.markerOwner, []);
        instance.editor.dispose();
        model?.dispose?.();
    }

    delete instances[elementId];
}

export function updateOptions(element, elementId, options) {
    const instance = instances[elementId];

    if (!instance?.editor)
        return;

    instance.editor.updateOptions(buildEditorOptions(options, false));

    setLanguage(element, elementId, options.language);
}

export function setDiagnostics(element, elementId, diagnostics) {
    const instance = instances[elementId];

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

export function setValue(element, elementId, value) {
    const instance = instances[elementId];

    if (!instance?.editor)
        return;

    value ??= "";

    if (instance.editor.getValue() === value)
        return;

    instance.setUpdating(true);

    try {
        instance.editor.setValue(value);
    } finally {
        instance.setUpdating(false);
    }
}

export function getValue(element, elementId) {
    const instance = instances[elementId];

    return instance?.editor?.getValue() ?? "";
}

export function focus(element, elementId) {
    const instance = instances[elementId];

    instance?.editor?.focus();
}

export function layout(element, elementId) {
    const instance = instances[elementId];

    instance?.editor?.layout();
}

export async function formatDocument(element, elementId) {
    const instance = instances[elementId];

    await instance?.editor?.getAction("editor.action.formatDocument")?.run();
}

export function revealLine(element, elementId, lineNumber) {
    const instance = instances[elementId];

    instance?.editor?.revealLineInCenter(lineNumber);
}

export function setLanguage(element, elementId, language) {
    const instance = instances[elementId];

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
    const instance = instances[elementId];

    if (!instance?.editor || !selection)
        return;

    instance.editor.setSelection(new monaco.Selection(
        selection.startLineNumber,
        selection.startColumn,
        selection.endLineNumber,
        selection.endColumn));
}

export function getSelection(element, elementId) {
    const instance = instances[elementId];
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
    if (window.monaco?.editor)
        return;

    if (loaderPromise)
        return await loaderPromise;

    const assetsPath = normalizeAssetsPath(options.assetsPath);

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
        throw error;
    }
}

function buildEditorOptions(options, includeValue = true) {
    const editorOptions = {
        readOnly: options.readOnly === true || options.disabled === true,
        automaticLayout: options.automaticLayout !== false,
        minimap: {
            enabled: options.minimap !== false
        },
        lineNumbers: options.lineNumbers === false ? "off" : "on",
        wordWrap: options.wordWrap === true ? "on" : "off",
        tabSize: options.tabSize || 4,
        insertSpaces: options.insertSpaces !== false,
        formatOnPaste: options.formatOnPaste === true,
        formatOnType: options.formatOnType === true,
        renderWhitespace: options.renderWhitespace === true ? "all" : "none",
        scrollBeyondLastLine: options.scrollBeyondLastLine !== false
    };

    if (includeValue) {
        editorOptions.value = options.value || "";
        editorOptions.language = options.language || "plaintext";
        editorOptions.theme = options.theme || "vs";
    }

    if (options.fontFamily) {
        editorOptions.fontFamily = options.fontFamily;
    }

    if (typeof options.fontSize === "number") {
        editorOptions.fontSize = options.fontSize;
    }

    if (options.additionalOptions) {
        Object.assign(editorOptions, options.additionalOptions);
    }

    return editorOptions;
}

function toMarker(diagnostic) {
    return {
        severity: diagnostic.severity || monaco.MarkerSeverity.Error,
        message: diagnostic.message || "",
        code: diagnostic.code || undefined,
        startLineNumber: Math.max(1, diagnostic.startLineNumber || 1),
        startColumn: Math.max(1, diagnostic.startColumn || 1),
        endLineNumber: Math.max(1, diagnostic.endLineNumber || diagnostic.startLineNumber || 1),
        endColumn: Math.max(1, diagnostic.endColumn || diagnostic.startColumn || 1)
    };
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
        return new Promise((resolve, reject) => {
            if (existing.dataset.loaded === "true") {
                resolve();
                return;
            }

            existing.addEventListener("load", resolve, { once: true });
            existing.addEventListener("error", reject, { once: true });
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
        script.addEventListener("error", reject, { once: true });
        document.head.appendChild(script);
    });
}

function configure(functionName, context, args) {
    const namespaces = functionName.split(".");
    const func = namespaces.pop();

    for (const namespace of namespaces) {
        context = context[namespace];

        if (!context)
            return;
    }

    return context[func].apply(context, args);
}