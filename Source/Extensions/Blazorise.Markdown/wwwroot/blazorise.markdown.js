const _instances = [];

export function initialize(dotNetObjectRef, elementId, value) {
    const instances = _instances;

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
}

export function destroy(elementId) {
    const instances = _instances || {};
    delete instances[elementId];
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