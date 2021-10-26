const _instances = [];

export function initialize(element, elementId, options) {
    _instances[elementId] = new ButtonInfo(element, elementId, options);

    if (element.type === "submit") {
        element.addEventListener("click", (e) => {
            click(_instances[elementId], e);
        });
    }
}

export function destroy(element, elementId) {
    var instances = _instances || {};
    delete instances[elementId];
}

export function click(buttonInfo, e) {
    if (buttonInfo.options.preventDefaultOnSubmit) {
        return e.preventDefault();
    }
}

class ButtonInfo {
    elementId = null;
    element = null;
    options = {};

    constructor(element, elementId, options) {
        this.elementId = elementId;
        this.element = element;
        this.options = options || {};
    }
}