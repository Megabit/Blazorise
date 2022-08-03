import { getRequiredElement } from "./utilities.js?v=1.0.6.0";

export function initialize(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    element.addEventListener('dragover', dragOverHandler);
    element.addEventListener('dragstart', dragStartHandler);
}

export function destroy(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    element.removeEventListener('dragover', dragOverHandler);
    element.removeEventListener('dragstart', dragStartHandler);
}

function dragOverHandler(e) {
    e.preventDefault();
}

function dragStartHandler(e) {
    e.dataTransfer.setData('', e.target.id);
}