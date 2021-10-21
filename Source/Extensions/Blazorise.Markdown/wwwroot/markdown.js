const _instances = [];

export function initialize(dotNetObjectRef, element, elementId, options) {
    const instances = _instances;

    const easyMDE = new EasyMDE({
        element: document.getElementById(elementId),
        hideIcons: options.hideIcons,
        showIcons: options.showIcons,
        renderingConfig: {
            singleLineBreaks: false,
            codeSyntaxHighlighting: true
        },
        initialValue: options.value,
        sideBySideFullscreen: false,
        autoDownloadFontAwesome: options.autoDownloadFontAwesome,
        lineNumbers: options.lineNumbers,
        lineWrapping: options.lineWrapping,
        minHeight: options.minHeight,
        maxHeight: options.maxHeight,
        placeholder: options.placeholder,
        tabSize: options.tabSize,
        theme: options.theme,
        direction: options.direction,
        //toolbar: options.toolbar,
        toolbarTips: options.toolbarTips
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

export function destroy(element, elementId) {
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