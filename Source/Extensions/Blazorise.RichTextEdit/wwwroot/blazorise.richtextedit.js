window.blazoriseRichTextEdit = {
    initialize: (dotnetAdapter, containerRef, readOnly, placeholder, theme, onContentChanged, bindEnter,
        onEnter, onFocus, onBlur, configure) => {
        if (!containerRef)
            return false;

        const editorRef = containerRef.getElementsByClassName("rte-editor")[0];
        const toolbarRef = containerRef.getElementsByClassName("rte-toolbar")[0];
        const contentRef = containerRef.getElementsByClassName("rte-content")[0];

        let options = {
            modules: {
                toolbar: toolbarRef,
                keyboard: undefined
            },
            bounds: containerRef,
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
                                dotnetAdapter.invokeMethodAsync(onEnter);
                            }
                        }
                    }
                }
            };
        }
        if (configure) {
            try {
                blazoriseRichTextEdit.configure(configure, window, [ options ]);
            } catch (err) {
                console.error(err);
            }
        }
        const quill = new Quill(editorRef, options);
        quill.on("text-change",
            (_dx, _dy, source) => {
                if (source === "user") {
                    dotnetAdapter.invokeMethodAsync(onContentChanged);
                }
            });
        quill.on("selection-change", function (range, oldRange, source) {
            if (range === null && oldRange !== null) {
                dotnetAdapter.invokeMethodAsync(onBlur);
            } else if (range !== null && oldRange === null)
                dotnetAdapter.invokeMethodAsync(onFocus);
        });

        function setContent() {
            quill.setContents(quill.clipboard.convert(contentRef.innerHTML));
            dotnetAdapter.invokeMethodAsync(onContentChanged);
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
        return true;
    },
    configure: (functionName, context, args) => {
        const namespaces = functionName.split(".");
        const func = namespaces.pop();
        for (let i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }
        return context[func].apply(context, args);
    },
    destroy: (editorRef) => {
        if (!editorRef)
            return false;

        if (editorRef.quill.contentObserver)
            editorRef.quill.contentObserver.disconnect();

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