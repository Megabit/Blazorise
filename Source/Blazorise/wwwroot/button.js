import { getRequiredElement } from "./utilities.js?v=1.6.1.0";

const _instances = [];

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    _instances[elementId] = new ButtonInfo(element, elementId, options);

    if (element && element.type === "submit") {
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
    constructor(element, elementId, options) {
        this.elementId = elementId;
        this.element = element;
        this.options = options || {};
    }
}