import "./vendors/easymde.js?v=1.8.9.0";
import "./vendors/highlight.js?v=1.8.9.0";
import { removeAllFileEntries } from "../Blazorise/io.js?v=1.8.9.0";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"_content/Blazorise.Markdown/vendors/easymde.css?v=1.8.9.0\" />");
document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"_content/Blazorise.Markdown/markdown.css?v=1.8.9.0\" />");

const _instances = [];

export function initialize(dotNetObjectRef, element, elementId, options) {
    const instances = _instances;

    if (!options.toolbar) {
        // remove empty toolbar so that we can fallback to the default items
        delete options.toolbar;
    }
    else if (options.toolbar && options.toolbar.length > 0) {
        // map any named action with a real action from EasyMDE
        options.toolbar.forEach(button => {
            // make sure we don't operate on separators
            if (button !== "|") {
                if (button.action) {
                    if (!button.action.startsWith("http")) {
                        button.action = EasyMDE[button.action];
                    }
                }
                else {
                    if (button.name && button.name.startsWith("http")) {
                        button.action = button.name;
                    }
                    else {
                        // custom action is used so we need to trigger custom event on click
                        button.action = (editor) => {
                            dotNetObjectRef.invokeMethodAsync('NotifyCustomButtonClicked', button.name, button.value).then(null, function (err) {
                                throw new Error(err);
                            });
                        }
                    }
                }

                // button without icon is not allowed
                if (!button.className) {
                    button.className = "fa fa-question";
                }
            }
        });
    }

    let nextFileId = 0;
    let imageUploadNotifier = {
        onSuccess: (e) => { },
        onError: (e) => { }
    };

    let fileEntriesToNotifyBuffer = [];
    let notifyUploadTimer = null;

    const mdeOptions = {
        element: document.getElementById(elementId),
        hideIcons: options.hideIcons,
        showIcons: options.showIcons,
        initialValue: options.value,
        autoDownloadFontAwesome: options.autoDownloadFontAwesome,
        lineNumbers: options.lineNumbers,
        lineWrapping: options.lineWrapping,
        minHeight: options.minHeight,
        maxHeight: options.maxHeight,
        placeholder: options.placeholder,
        tabSize: options.tabSize,
        theme: options.theme,
        direction: options.direction,
        toolbar: options.toolbar,
        toolbarTips: options.toolbarTips,

        uploadImage: options.uploadImage,
        imageMaxSize: options.imageMaxSize,
        imageAccept: options.imageAccept,
        imageUploadEndpoint: options.imageUploadEndpoint,
        imagePathAbsolute: options.imagePathAbsolute,
        imageCSRFToken: options.imageCSRFToken,
        imageTexts: options.imageTexts,
        imageUploadFunction: (file, onSuccess, onError) => {
            // hack to save the reference to the callback functions
            imageUploadNotifier.onSuccess = onSuccess;
            imageUploadNotifier.onError = onError;

            // Reduce to purely serializable data, plus build an index by ID
            if (!element._blazorFilesById) {
                element._blazorFilesById = {};
            }

            var fileEntry = {
                id: ++nextFileId,
                lastModified: new Date(file.lastModified).toISOString(),
                name: file.name,
                size: file.size,
                type: file.type
            };

            element._blazorFilesById[fileEntry.id] = fileEntry;

            // Attach the blob data itself as a non-enumerable property so it doesn't appear in the JSON
            Object.defineProperty(fileEntry, 'blob', { value: file });

            fileEntriesToNotifyBuffer.push(fileEntry);

            // Reset debounce timer: if a new file is added within 100ms, reset the timer
            if (notifyUploadTimer) {
                clearTimeout(notifyUploadTimer);
            }

            notifyUploadTimer = setTimeout(() => {
                // Send batched files to .NET when no more files arrive within 100ms
                dotNetObjectRef.invokeMethodAsync('NotifyImageUpload', fileEntriesToNotifyBuffer)
                    .then(() => {
                        fileEntriesToNotifyBuffer = [];
                    })
                    .catch(err => {
                        fileEntriesToNotifyBuffer = [];
                        throw new Error(err);
                    });

                notifyUploadTimer = null;
            }, 100);
        },

        errorMessages: options.errorMessages,
        errorCallback: (errorMessage) => {
            dotNetObjectRef.invokeMethodAsync("NotifyErrorMessage", errorMessage);
        },

        autofocus: options.autofocus,
        autoRefresh: options.autoRefresh,
        autosave: options.autosave,
        blockStyles: options.blockStyles,
        forceSync: options.forceSync,
        indentWithTabs: options.indentWithTabs,
        inputStyle: options.inputStyle,
        insertTexts: options.insertTexts,
        nativeSpellcheck: options.nativeSpellcheck,
        parsingConfig: options.parsingConfig,
        previewClass: options.previewClass,
        previewRender: options.usePreviewRender ? (plainText, preview) => {
            dotNetObjectRef.invokeMethodAsync("NotifyPreviewRender", plainText).then((htmlText) => {
                preview.innerHTML = htmlText;
            }).catch((error) => {
                console.error(error);
            });
        } : null,
        previewImagesInEditor: options.previewImagesInEditor,
        promptTexts: options.promptTexts,
        promptURLs: options.promptURLs,
        renderingConfig: options.renderingConfig ? options.renderingConfig : {
            singleLineBreaks: false,
            codeSyntaxHighlighting: true
        },
        scrollbarStyle: options.scrollbarStyle,
        shortcuts: options.shortcuts,
        sideBySideFullscreen: options.sideBySideFullscreen,
        spellChecker: options.spellChecker,
        status: options.status,
        styleSelectedText: options.styleSelectedText,
        syncSideBySidePreviewScroll: options.syncSideBySidePreviewScroll,
        unorderedListStyle: options.unorderedListStyle,
        toolbarButtonClassPrefix: options.toolbarButtonClassPrefix
    };

    if (!mdeOptions.status) {
        // remove empty status so that we can fallback to the default items
        delete mdeOptions.status;
    }
    else if (mdeOptions.status.length === 0) {
        mdeOptions.status = false;
    }

    const easyMDE = new EasyMDE(mdeOptions);

    easyMDE.codemirror.on("change", function (instance, changeObj) {
        if (changeObj && changeObj.origin === "setValue") {
            return;
        }

        dotNetObjectRef.invokeMethodAsync("UpdateInternalValue", easyMDE.value());
    });

    instances[elementId] = {
        dotNetObjectRef: dotNetObjectRef,
        elementId: elementId,
        editor: easyMDE,
        imageUploadNotifier: imageUploadNotifier
    };

    applyBaseInputOptions(instances[elementId], options);
}

export function destroy(element, elementId) {
    const instances = _instances || {};

    const instance = instances[elementId];

    if (instance) {
        if (element) {
            removeAllFileEntries(element);
        }

        instance.editor.toTextArea();
        instance.editor = null;

        delete instances[elementId];
    }
}

export function setValue(elementId, value) {
    const instance = _instances[elementId];

    if (instance) {
        instance.editor.value(value);
    }
}

export function getValue(elementId) {
    const instance = _instances[elementId];

    if (instance) {
        return instance.editor.value();
    }

    return null;
}

export function notifyImageUploadSuccess(elementId, imageUrl) {
    const instance = _instances[elementId];

    if (instance) {
        return instance.imageUploadNotifier.onSuccess(imageUrl);
    }
}

export function notifyImageUploadError(elementId, errorMessage) {
    const instance = _instances[elementId];

    if (instance) {
        return instance.imageUploadNotifier.onError(errorMessage);
    }
}

export function focus(elementId, scrollToElement) {
    const instance = _instances[elementId];

    if (instance) {
        if (instance.baseInputState && instance.editor && instance.editor.codemirror) {
            const readOnly = instance.editor.codemirror.getOption("readOnly");
            if (readOnly === "nocursor") {
                return;
            }
        }

        return instance.editor.codemirror.focus({
            preventScroll: !scrollToElement
        });
    }
}

export function updateBaseInputOptions(elementId, options) {
    const instance = _instances[elementId];

    if (instance) {
        applyBaseInputOptions(instance, options);
    }
}

function splitClassNames(classNames) {
    if (!classNames)
        return [];

    return classNames
        .split(/\s+/g)
        .map(c => c && c.trim())
        .filter(Boolean);
}

function getEditorElements(instance) {
    if (!instance || !instance.editor || !instance.editor.codemirror)
        return {};

    const codeMirrorElement = instance.editor.codemirror.getWrapperElement();
    const containerElement = codeMirrorElement ? codeMirrorElement.closest(".EasyMDEContainer") : null;
    const inputElement = typeof instance.editor.codemirror.getInputField === "function"
        ? instance.editor.codemirror.getInputField()
        : null;

    return {
        containerElement: containerElement,
        codeMirrorElement: codeMirrorElement,
        inputElement: inputElement
    };
}

function applyClassNames(element, instanceState, classNames) {
    if (!element)
        return;

    const oldClasses = instanceState.appliedClassNames || [];
    oldClasses.forEach(c => element.classList.remove(c));

    const newClasses = splitClassNames(classNames)
        .filter(c => c !== "EasyMDEContainer");

    newClasses.forEach(c => element.classList.add(c));

    instanceState.appliedClassNames = newClasses;
}

function parseStyleNames(styleNames) {
    const styles = [];

    if (!styleNames)
        return styles;

    styleNames.split(";").forEach(part => {
        if (!part)
            return;

        const style = part.trim();
        if (!style)
            return;

        const colonIndex = style.indexOf(":");
        if (colonIndex < 0)
            return;

        const name = style.substring(0, colonIndex).trim();
        const value = style.substring(colonIndex + 1).trim();

        if (!name)
            return;

        styles.push([name, value]);
    });

    return styles;
}

function applyStyleNames(element, instanceState, styleNames) {
    if (!element)
        return;

    const oldStyleNames = instanceState.appliedStyleNames || [];
    oldStyleNames.forEach(name => element.style.removeProperty(name));

    const newStyles = parseStyleNames(styleNames);
    const newStyleNames = [];

    newStyles.forEach(pair => {
        const name = pair[0];
        const value = pair[1];

        element.style.setProperty(name, value);
        newStyleNames.push(name);
    });

    instanceState.appliedStyleNames = newStyleNames;
}

function applyAttributes(element, instanceState, attributes) {
    if (!element)
        return;

    const oldAttributes = instanceState.appliedAttributes || [];

    oldAttributes.forEach(name => {
        if (!attributes || !(name in attributes)) {
            element.removeAttribute(name);
        }
    });

    const newAttributes = [];

    if (attributes) {
        Object.keys(attributes).forEach(name => {
            if (!name)
                return;

            const lowerName = name.toLowerCase();

            if (lowerName === "class" || lowerName === "style" || lowerName === "id" || lowerName === "readonly" || lowerName === "disabled")
                return;

            const value = attributes[name];

            if (value === null || value === undefined || value === false) {
                element.removeAttribute(name);
                return;
            }

            if (value === true) {
                element.setAttribute(name, "");
            }
            else {
                element.setAttribute(name, value);
            }

            newAttributes.push(name);
        });
    }

    instanceState.appliedAttributes = newAttributes;
}

function applyReadOnlyAndDisabled(instance, containerElement, options) {
    if (!instance || !instance.editor || !instance.editor.codemirror || !options)
        return;

    const disabled = options.disabled === true;
    const readOnly = options.readOnly === true;

    // Disabled takes precedence over ReadOnly.
    if (disabled) {
        instance.editor.codemirror.setOption("readOnly", "nocursor");
    }
    else if (readOnly) {
        instance.editor.codemirror.setOption("readOnly", true);
    }
    else {
        instance.editor.codemirror.setOption("readOnly", false);
    }

    if (containerElement) {
        containerElement.setAttribute("aria-disabled", disabled ? "true" : "false");
        containerElement.classList.toggle("b-markdown-disabled", disabled);
    }

    // Only disable toolbar interactions when Disabled is true.
    if (containerElement) {
        const toolbarElement = containerElement.querySelector(".editor-toolbar");

        if (toolbarElement) {
            toolbarElement.classList.toggle("disabled", disabled);

            toolbarElement.querySelectorAll("button").forEach(btn => {
                btn.disabled = disabled;
            });

            toolbarElement.querySelectorAll("a").forEach(anchor => {
                anchor.setAttribute("aria-disabled", disabled ? "true" : "false");
                anchor.style.pointerEvents = disabled ? "none" : "";

                if (disabled) {
                    anchor.setAttribute("tabindex", "-1");
                }
                else {
                    anchor.removeAttribute("tabindex");
                }
            });
        }
    }
}

function applyBaseInputOptions(instance, options) {
    if (!instance || !options)
        return;

    const instanceState = instance.baseInputState || (instance.baseInputState = {});
    const containerState = instanceState.containerState || (instanceState.containerState = {});
    const inputState = instanceState.inputState || (instanceState.inputState = {});
    const editorElements = getEditorElements(instance);
    const containerElement = editorElements.containerElement;
    const inputElement = editorElements.inputElement;

    applyReadOnlyAndDisabled(instance, containerElement, options);

    // Apply BaseComponent styling/attributes to the visible editor container.
    applyClassNames(containerElement, containerState, options.classNames);
    applyStyleNames(containerElement, containerState, options.styleNames);
    applyAttributes(containerElement, containerState, options.attributes);

    // Apply attributes to the actual focusable input element created by CodeMirror.
    applyAttributes(inputElement, inputState, options.attributes);
}