import "./vendors/quill.js?v=2.0.2.0";
import { getRequiredElement } from "../Blazorise/utilities.js?v=2.0.2.0";

var rteLoadedStyleUrls = new Set();
var rteSanitizedPasteLoaded = false;
var rteSanitizedPasteLoader = null;
var rteSanitizedPasteModule = null;
var rteTableBetterLoaded = false;
var rteTableBetterLoader = null;
var rteResizeLoaded = false;
var rteResizeLoader = null;

async function loadSanitizedPasteModule() {
    if (rteSanitizedPasteLoaded)
        return rteSanitizedPasteModule;

    if (!rteSanitizedPasteLoader) {
        rteSanitizedPasteLoader = import("./vendors/quill-paste-smart.js?v=2.0.2.0")
            .then((module) => {
                rteSanitizedPasteModule = module;
                rteSanitizedPasteLoaded = true;
                return module;
            });
    }

    try {
        return await rteSanitizedPasteLoader;
    } catch (error) {
        rteSanitizedPasteLoader = null;
        rteSanitizedPasteModule = null;
        throw error;
    }
}

async function loadTableBetterModule() {
    if (rteTableBetterLoaded)
        return;

    if (!rteTableBetterLoader) {
        rteTableBetterLoader = import("./vendors/quill-table-better.js?v=2.0.2.0")
            .then(() => {
                rteTableBetterLoaded = true;
            });
    }

    try {
        await rteTableBetterLoader;
    } catch (error) {
        rteTableBetterLoader = null;
        throw error;
    }
}

async function loadResizeModule() {
    if (rteResizeLoaded)
        return;

    if (!rteResizeLoader) {
        rteResizeLoader = import("./vendors/quill-resize-module.js?v=2.0.2.0")
            .then(() => {
                rteResizeLoaded = true;
            });
    }

    try {
        await rteResizeLoader;
    } catch (error) {
        rteResizeLoader = null;
        throw error;
    }
}

function resolveSanitizedPasteApi(module) {
    if (module?.registerPasteSmartClipboard)
        return module;

    if (module?.default?.registerPasteSmartClipboard)
        return module.default;

    if (window.QuillPasteSmart?.registerPasteSmartClipboard)
        return window.QuillPasteSmart;

    return undefined;
}

function applySanitizedPasteOptions(clipboardOptions, sanitizedPasteOptions) {
    if (!sanitizedPasteOptions)
        return;

    if (Array.isArray(sanitizedPasteOptions.allowedTags) || Array.isArray(sanitizedPasteOptions.allowedAttributes)) {
        clipboardOptions.allowed = {};

        if (Array.isArray(sanitizedPasteOptions.allowedTags))
            clipboardOptions.allowed.tags = sanitizedPasteOptions.allowedTags;

        if (Array.isArray(sanitizedPasteOptions.allowedAttributes))
            clipboardOptions.allowed.attributes = sanitizedPasteOptions.allowedAttributes;
    }

    if (typeof sanitizedPasteOptions.keepSelection === "boolean")
        clipboardOptions.keepSelection = sanitizedPasteOptions.keepSelection;

    if (typeof sanitizedPasteOptions.substituteBlockElements === "boolean")
        clipboardOptions.substituteBlockElements = sanitizedPasteOptions.substituteBlockElements;

    if (typeof sanitizedPasteOptions.magicPasteLinks === "boolean")
        clipboardOptions.magicPasteLinks = sanitizedPasteOptions.magicPasteLinks;

    if (typeof sanitizedPasteOptions.removeConsecutiveSubstitutionTags === "boolean")
        clipboardOptions.removeConsecutiveSubstitutionTags = sanitizedPasteOptions.removeConsecutiveSubstitutionTags;
}

export function loadStylesheets(styles, version) {
    if (!styles?.length)
        return;

    styles.forEach(sheet => {
        const href = `_content/Blazorise.RichTextEdit/vendors/${sheet}.css?v=${version}`;

        if (rteLoadedStyleUrls.has(href))
            return;

        document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", `<link rel="stylesheet" href="${href}"/>`);
        rteLoadedStyleUrls.add(href);
    });
}

export async function initialize(dotnetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const editorRef = element.getElementsByClassName("b-richtextedit-editor")[0];
    const toolbarRef = element.getElementsByClassName("b-richtextedit-toolbar")[0];
    const contentRef = element.getElementsByClassName("b-richtextedit-content")[0];
    const richTextEditVersion = options.version || "2.0.2.0";

    let quillOptions = {
        modules: {
            toolbar: toolbarRef,
            keyboard: undefined,
            table: false
        },
        bounds: element,
        placeholder: options.placeholder,
        readOnly: options.readOnly,
        theme: options.theme
    };

    const moduleStyles = [];

    if (options.useTables === true)
        moduleStyles.push("quill-table-better");

    if (options.useResize === true)
        moduleStyles.push("quill-resize-module");

    loadStylesheets(moduleStyles, richTextEditVersion);

    if (options.submitOnEnter === true) {
        quillOptions.modules.keyboard = {
            bindings: {
                enter: {
                    key: 'Enter',
                    shiftKey: false,
                    metaKey: false,
                    ctrlKey: false,
                    altKey: false,
                    format: { list: false },
                    handler: () => {
                        dotnetAdapter.invokeMethodAsync("OnEnter");
                    }
                }
            }
        };
    }

    if (options.useTables === true) {
        await loadTableBetterModule();

        Quill.register({ 'modules/table-better': QuillTableBetter }, true);

        quillOptions.modules['table-better'] = {
            toolbarTable: true
        };

        quillOptions.modules.keyboard = {
            bindings: QuillTableBetter.keyboardBindings
        };
    }

    const clipboardOptions = {
        enabled: options.useSanitizedPaste === true
    };

    if (options.useSanitizedPaste === true)
        applySanitizedPasteOptions(clipboardOptions, options.sanitizedPasteOptions);

    quillOptions.modules.clipboard = clipboardOptions;

    if (options.useSanitizedPaste === true) {
        const sanitizedPasteModule = await loadSanitizedPasteModule();
        const sanitizedPasteApi = resolveSanitizedPasteApi(sanitizedPasteModule);

        if (!sanitizedPasteApi?.registerPasteSmartClipboard) {
            console.error("quill-paste-smart is missing registerPasteSmartClipboard export.");
        } else {
            const clipboardModule = Quill.import("modules/clipboard");

            if (!clipboardModule?.__blazoriseSanitizedPasteWrapper) {
                const sanitizedClipboardModule = sanitizedPasteApi.registerPasteSmartClipboard(Quill);

                if (sanitizedClipboardModule)
                    sanitizedClipboardModule.__blazoriseSanitizedPasteWrapper = true;
            }
        }
    }

    if (options.useResize === true) {
        await loadResizeModule();

        Quill.register({ 'modules/resize': QuillResize }, true);

        quillOptions.modules.resize = {
            tools: [
                "left",
                "center",
                "right",
                "full",
                "edit",
                {
                    text: "Alt",
                    verify(activeEle) {
                        return activeEle && activeEle.tagName === "IMG";
                    },
                    handler(evt, button, activeEle) {
                        let alt = activeEle.alt || "";
                        alt = window.prompt("Alt for image", alt);
                        if (alt == null) return;
                        activeEle.setAttribute("alt", alt);
                    },
                },
            ],
        };
    }

    if (options.configureQuillJsMethod) {
        try {
            configure(options.configureQuillJsMethod, window, [quillOptions]);
        } catch (err) {
            console.error(err);
        }
    }

    var contentUpdating = false;

    const quill = new Quill(editorRef, quillOptions);

    const stopArrowKeyPropagation = (event) => {
        const arrowKeys = ["ArrowLeft", "ArrowRight", "ArrowUp", "ArrowDown"];

        if (!arrowKeys.includes(event.key))
            return;

        if (!editorRef.contains(event.target))
            return;

        event.stopImmediatePropagation();
        event.stopPropagation();
    };

    editorRef._stopArrowKeyPropagation = stopArrowKeyPropagation;
    document.addEventListener("keydown", stopArrowKeyPropagation, true);
    document.addEventListener("keyup", stopArrowKeyPropagation, true);

    quill.on("text-change", function (dx, dy, source) {
        if (source === "user") {
            contentUpdating = true;
            dotnetAdapter.invokeMethodAsync("OnContentChanged", quill.root.innerHTML, quill.getText())
                .finally(_ => contentUpdating = false);
        }
    });

    quill.on("selection-change", function (range, oldRange, source) {
        if (range === null && oldRange !== null) {
            dotnetAdapter.invokeMethodAsync("OnEditorBlur");
        } else if (range !== null && oldRange === null)
            dotnetAdapter.invokeMethodAsync("OnEditorFocus");
    });

    function setContent() {
        if (contentUpdating) return;

        quill.clipboard.dangerouslyPasteHTML(contentRef.innerHTML);
    }

    // create an observer for content changes
    quill.contentObserver = new MutationObserver(() => setContent());
    quill.contentObserver.observe(contentRef,
        {
            attributes: true,
            childList: true,
            characterData: true
        });

    setContent();

    editorRef.quill = quill;
}

export function configure(functionName, context, args) {
    const namespaces = functionName.split(".");
    const func = namespaces.pop();
    for (let i = 0; i < namespaces.length; i++) {
        context = context[namespaces[i]];
    }
    return context[func].apply(context, args);
}

export function destroy(editorRef, editorId) {
    if (!editorRef)
        return false;

    if (editorRef._stopArrowKeyPropagation) {
        document.removeEventListener("keydown", editorRef._stopArrowKeyPropagation, true);
        document.removeEventListener("keyup", editorRef._stopArrowKeyPropagation, true);
    }

    if (editorRef.quill.contentObserver)
        editorRef.quill.contentObserver.disconnect();

    delete editorRef.quill;
    delete editorRef._stopArrowKeyPropagation;
    return true;
}

export function setReadOnly(editorRef, readOnly) {
    const editor = editorRef.quill;
    if (!editor)
        return;
    if (readOnly)
        editor.disable();
    else
        editor.enable();
}


export function getHtml(editorRef, htmlOptions) {
    const editor = editorRef.quill;
    if (!editor)
        return undefined;

    if (htmlOptions && htmlOptions.isSemanticHtml) {
        if (htmlOptions.length >= 0) {
            return editor.getSemanticHTML(htmlOptions.index, htmlOptions.length);
        }

        return editor.getSemanticHTML(htmlOptions.index);
    }

    return editor.root.innerHTML;
}

export function setHtml(editorRef, html) {
    const editor = editorRef.quill;
    if (!editor)
        return;

    editor.clipboard.dangerouslyPasteHTML(html);
}

export function getDelta(editorRef) {
    const editor = editorRef.quill;
    if (!editor)
        return;
    return JSON.stringify(editor.getContents());
}

export function setDelta(editorRef, delta) {
    const editor = editorRef.quill;
    if (!editor)
        return;
    editor.setContents(JSON.parse(delta));
}

export function getText(editorRef) {
    const editor = editorRef.quill;
    if (!editor)
        return;
    return editor.getText();
}

export function setText(editorRef, txt) {
    const editor = editorRef.quill;
    if (!editor)
        return;
    editor.setText(txt);
}

export function clearContent(editorRef) {
    const editor = editorRef.quill;
    if (!editor)
        return;
    editor.setContents([]);
}