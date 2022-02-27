import { getRequiredElement } from "./utilities.js";

export function initialize(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    element.addEventListener('dragover', function (e) {
        e.preventDefault();
    });

    element.addEventListener('dragstart', function (e) {
        e.dataTransfer.setData('', e.target.id);
    });
}