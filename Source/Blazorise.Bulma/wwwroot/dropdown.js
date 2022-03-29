import { getRequiredElement } from "../Blazorise/utilities.js?v=1.0.1.0";
import * as dropdown from "../Blazorise/dropdown.js?v=1.0.1.0";

const SHOW_CLASS = "is-active";

export function initialize(element, elementId, targetElementId, menuElementId, options) {
    dropdown.initialize(element, elementId, targetElementId, menuElementId, options);
}

export function destroy(element, elementId) {
    dropdown.destroy(element, elementId, options);
}

export function show(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = dropdown.getInstance(elementId);

    if (instance) {
        instance.update();

        element.classList.add(SHOW_CLASS);
    }
}

export function hide(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = dropdown.getInstance(elementId);

    if (instance) {
        instance.update();

        element.classList.remove(SHOW_CLASS);
    }
}
