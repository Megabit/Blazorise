import { getRequiredElement } from "../Blazorise/utilities.js?v=1.0.1.0";
import { createPopper } from "../Blazorise/popper.js?v=1.0.1.0";

const _instances = [];
const SHOW_CLASS = "is-active";

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const btnToggle = element.querySelector(".dropdown-trigger");
    const menu = element.querySelector(".dropdown-menu");

    const instance = createPopper(btnToggle, menu, options);

    element.classList.add(SHOW_CLASS);
    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    element.classList.remove(SHOW_CLASS);

    let instances = _instances || {};

    const instance = instances[elementId];

    if (instance) {
        instance.destroy();

        delete instances[elementId];
    }
}
