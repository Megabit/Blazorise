window.blazoriseRichTextEdit = {
    initialize: (dotnetAdapter, editorRef, toolbarRef, readOnly, placeholder, theme, onContentChanged, bindEnter, onEnter, configure) => {
        if (!editorRef) return false;

        var options = {
            modules: {
                toolbar: toolbarRef
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
                                return;
                            }

                            dotnetAdapter.invokeMethodAsync(onEnter);
                        }
                    }
                }
            };
        }

        function executeFunctionByName(functionName, context /*, args */) {
            const args = Array.prototype.slice.call(arguments, 2);
            const namespaces = functionName.split(".");
            const func = namespaces.pop();
            for (let i = 0; i < namespaces.length; i++) {
                context = context[namespaces[i]];
            }
            return context[func].apply(context, args);
        };

        if (configure) {
            try {
                const updatedOptions = executeFunctionByName(configure, window, options);
                if (updatedOptions) {
                    options = updatedOptions;
                }
            } catch (err) {
                console.error(err);
            } 
        }

        const quill = new Quill(editorRef, options);

        quill.on('text-change', (delta, oldDelta, source) => {
            if (source === 'user') {
                dotnetAdapter.invokeMethodAsync(onContentChanged);
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
    getHtml: (editorRef) => {
        var editor = editorRef.quill;
        if (!editor) return undefined;

        return editor.root.innerHTML;
    },
    setHtml: (editorRef, html) => {
        var editor = editorRef.quill;
        if (!editor) return;

        var delta = editor.clipboard.convert(html);
        editor.setContents(delta);
    },
    getDelta: (editorRef) => {
        var editor = editorRef.quill;
        if (!editor) return undefined;

        return JSON.stringify(editor.getContents());
    },
    setDelta: (editorRef, delta) => {
        var editor = editorRef.quill;
        if (!editor) return;

        editor.setContents(delta);
    },
    getText: (editorRef) => {
        var editor = editorRef.quill;
        if (!editor) return undefined;

        return editor.getText();
    },
    setText: (editorRef, txt) => {
        var editor = editorRef.quill;
        if (!editor) return;

        editor.setText(txt);
    },
    clearContent: (editorRef) => {
        var editor = editorRef.quill;
        if (!editor) return;

        editor.setContents([]);
    }
};