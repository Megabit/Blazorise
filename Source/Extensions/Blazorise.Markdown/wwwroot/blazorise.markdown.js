window.blazoriseMarkdown = {
    _instances: [],

    initialize: (dotNetObjectRef, elementId, value) => {
        const instances = window.blazoriseMarkdown._instances = window.blazoriseMarkdown._instances || {};

        const easyMDE = new EasyMDE({
            element: document.getElementById(elementId),
            showIcons: ["code", "table"],
            renderingConfig: {
                singleLineBreaks: false,
                codeSyntaxHighlighting: true
            },
            initialValue: value,
            sideBySideFullscreen: false,
            hideIcons: ["side-by-side", "fullscreen"]
        });

        easyMDE.codemirror.on("change", function () {
            dotNetObjectRef.invokeMethodAsync("UpdateInternalValue", easyMDE.value());
        });

        instances[elementId] = {
            dotNetObjectRef: dotNetObjectRef,
            elementId: elementId,
            editor: easyMDE
        };
    },

    destroy: (elementId) => {
        var instances = window.blazoriseMarkdown._instances || {};
        delete instances[elementId];
    },

    getValue: (elementId) => {
        const instance = window.blazoriseMarkdown._instances[elementId];

        if (instance) {
            return instance.editor.value();
        }

        return null;
    },

    setValue: (elementId, value) => {
        const instance = window.blazoriseMarkdown._instances[elementId];

        if (instance) {
            instance.editor.value(value);
        }
    },
};