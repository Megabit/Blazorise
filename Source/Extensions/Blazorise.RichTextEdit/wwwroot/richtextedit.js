import { getRequiredElement } from "../Blazorise/utilities.js?v=1.4.0.0";

export function initialize(dotnetAdapter, element, elementId, readOnly, placeholder, theme, bindEnter, configureQuillJsMethod) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const editorRef = element.getElementsByClassName("rte-editor")[0];
    const toolbarRef = element.getElementsByClassName("rte-toolbar")[0];
    const contentRef = element.getElementsByClassName("rte-content")[0];

    let options = {
        modules: {
            toolbar: toolbarRef,
            keyboard: undefined
        },
        bounds: element,
        placeholder: placeholder,
        readOnly: readOnly,
        theme: theme
    };
    if (bindEnter === true) {
        options.modules.keyboard = {
            bindings: {
                enter: {
                    key: 13,
                    shiftKey: false,
                    metaKey: false,
                    ctrlKey: false,
                    altKey: false,
                    handler: (range, context) => {
                        if (context.format.list) {
                            editorRef.quill.insertText(range.index, "\n");
                        } else {
                            dotnetAdapter.invokeMethodAsync("OnEnter");
                        }
                    }
                }
            }
        };
    }
    if (configureQuillJsMethod) {
        try {
            configure(configureQuillJsMethod, window, [options]);
        } catch (err) {
            console.error(err);
        }
    }
    var contentUpdating = false;
    const quill = new Quill(editorRef, options);
    quill.on("text-change", function (dx, dy, source) {
        if (source === "user") {
            contentUpdating = true;
            dotnetAdapter.invokeMethodAsync("OnContentChanged")
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

        const content = quill.clipboard.convert(contentRef.innerHTML);
        quill.setContents(content);
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

    if (editorRef.quill.contentObserver)
        editorRef.quill.contentObserver.disconnect();

    delete editorRef.quill;
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

export function getHtml(editorRef) {
    const editor = editorRef.quill;
    if (!editor)
        return undefined;
    return editor.root.innerHTML;
}

export function setHtml(editorRef, html) {
    const editor = editorRef.quill;
    if (!editor)
        return;
    const delta = editor.clipboard.convert(html);
    editor.setContents(delta);
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