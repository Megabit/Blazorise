window.blazoriseRichTextEdit = {
    initialize: (dotnetAdapter, editorRef, toolbarRef, readOnly, placeholder, theme, onContentChanged, bindEnter, onEnter) => {
        if (!editorRef) return false;

        const options = {
            modules: { },
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
                        handler: () => {
                            dotnetAdapter.invokeMethodAsync(onEnter);
                        }
                    }
                }
            };
        }

        if (toolbarRef) {
            options.modules.toolbar = toolbarRef;
        }

        const quill = new Quill(editorRef, options);

        quill.on('text-change', (delta, oldDelta, source) => {
            if (source === 'user') {
                const html = quill.root.innerHTML;
                dotnetAdapter.invokeMethodAsync(onContentChanged, html);
            }
        });

        editorRef.quill = quill;
        return true;
    },
    destroy: (editorRef) => {
        if (!editorRef) return false;

        delete editorRef.quill;

        return true;
    },
    setReadOnly: (editorRef, value) => {
        var editor = editorRef.quill;
        if (!editor) return;

        if (value)
            editor.disable();
        else
            editor.enable();
    },
    getContent: (editorRef) => {
        var editor = editorRef.quill;
        if (!editor) return undefined;

        return editor.root.innerHTML;
    },
    setContent: (editorRef, html) => {
        var editor = editorRef.quill;
        if (!editor) return; 

        var delta = editor.clipboard.convert(html);
        editor.setContents(delta);
    },
    clearContent: (editorRef) => {
        var editor = editorRef.quill;
        if (!editor) return; 

        editor.setContents([]);
    }
}