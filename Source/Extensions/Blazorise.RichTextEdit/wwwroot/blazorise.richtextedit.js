window.blazoriseRichTextEdit = {
    initialize: (dotnetAdapter, editorRef, toolbarRef, readOnly, placeholder, theme, onContentChanged, bindEnter,
        onEnter, configure) => {
        if (!editorRef)
            return false;
        let options = {
            modules: {
                toolbar: toolbarRef,
                keyboard: undefined
            },
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
                                editorRef.quill.insertText(range.index, '\n');
                            } else {
                                dotnetAdapter.invokeMethodAsync(onEnter);
                            }
                        }
                    }
                }
            };
        }
        if (configure) {
            try {
                const updatedOptions = blazoriseRichTextEdit.configure(configure, window, options);
                if (updatedOptions) {
                    options = updatedOptions;
                }
            } catch (err) {
                console.error(err);
            }
        }
        const quill = new Quill(editorRef, options);
        quill.on('text-change',
            (_dx, _dy, source) => {
                if (source === 'user') {
                    dotnetAdapter.invokeMethod(onContentChanged);
                }
            });
        editorRef.quill = quill;
        return true;
    },
    configure: (functionName, context, options) => {
        const namespaces = functionName.split(".");
        const func = namespaces.pop();
        for (let i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }
        return context[func].apply(context, options);
    },
    destroy: (editorRef) => {
        if (!editorRef)
            return false;
        delete editorRef.quill;
        return true;
    },
    setReadOnly: (editorRef, readOnly) => {
        const editor = editorRef.quill;
        if (!editor)
            return;
        if (readOnly)
            editor.disable();
        else
            editor.enable();
    },
    getHtml: (editorRef) => {
        const editor = editorRef.quill;
        if (!editor)
            return undefined;
        return editor.root.innerHTML;
    },
    setHtml: (editorRef, html) => {
        const editor = editorRef.quill;
        if (!editor)
            return;
        const delta = editor.clipboard.convert(html);
        editor.setContents(delta);
    },
    getDelta: (editorRef) => {
        const editor = editorRef.quill;
        if (!editor)
            return;
        return JSON.stringify(editor.getContents());
    },
    setDelta: (editorRef, delta) => {
        const editor = editorRef.quill;
        if (!editor)
            return;
        editor.setContents(delta);
    },
    getText: (editorRef) => {
        const editor = editorRef.quill;
        if (!editor)
            return;
        return editor.getText();
    },
    setText: (editorRef, txt) => {
        const editor = editorRef.quill;
        if (!editor)
            return;
        editor.setText(txt);
    },
    clearContent: (editorRef) => {
        const editor = editorRef.quill;
        if (!editor)
            return;
        editor.setContents([]);
    }
};