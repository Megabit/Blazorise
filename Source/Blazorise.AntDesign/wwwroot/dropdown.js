import { getRequiredElement } from "../Blazorise/utilities.js?v=1.0.1.0";
import { createPopper } from "../Blazorise/popper.js?v=1.0.1.0";

const _instances = [];
const HIDE_CLASS = "ant-dropdown-hidden";

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const btnToggle = element.querySelector(".ant-dropdown-trigger");
    const menu = element.querySelector(".ant-dropdown-menu");

    const instance = createPopper(btnToggle, menu, options);

    element.classList.remove(HIDE_CLASS);
    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    element.classList.add(HIDE_CLASS);

    let instances = _instances || {};

    const instance = instances[elementId];

    if (instance) {
        instance.destroy();

        delete instances[elementId];
    }
}
