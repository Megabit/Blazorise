import { getRequiredElement } from "./utilities.js?v=1.0.1.0";
import { createPopper } from "./popper.js?v=1.0.1.0";

const _instances = [];
const SHOW_CLASS = "show";

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const btnToggle = element.querySelector(".dropdown-toggle");
    const menu = element.querySelector(".dropdown-menu");

    const instance = createPopper(btnToggle, menu, options);

    element.classList.add(SHOW_CLASS);
    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    let instances = _instances || {};

    const instance = instances[elementId];

    if (instance) {
        instance.destroy();

        delete instances[elementId];
    }
}
