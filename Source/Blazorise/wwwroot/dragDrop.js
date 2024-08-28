import { getRequiredElement } from "./utilities.js?v=1.6.1.0";

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

export function initializeThrottledDragEvents(element, elementId, dotnetAdapter) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    element.dotnetAdapter = dotnetAdapter;
    element.timeOutForDrag = null;
    element.timeOutForDragOver = null;

    element.addEventListener('drag', throttledDragHandler);
    element.addEventListener('dragover', throttledDragOverHandler);
}

export function destroyThrottledDragEvents(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    if (typeof throttledDragHandler === "function") {
        element.removeEventListener("drag", throttledDragHandler);
    }

    if (typeof throttledDragOverHandler === "function") {
        element.removeEventListener("dragover", throttledDragOverHandler);
    }
}

function dragOverHandler(e) {
    e.preventDefault();
}

function dragStartHandler(e) {
    e.dataTransfer.setData('', e.target.id);
}

function throttledDragHandler(e) {
    e.preventDefault();

    if (e.target && !e.target.timeOutForDrag) {
        e.target.timeOutForDrag = setTimeout(function () {
            e.target.timeOutForDrag = null;
            if (e.target.dotnetAdapter) {
                e.target.dotnetAdapter.invokeMethodAsync("OnDragHandler", e);
            }
        }.bind(this), 250);
    }
}

function throttledDragOverHandler(e) {
    e.preventDefault();
    if (e.target && !e.target.timeOutForDragOver) {
        e.target.timeOutForDragOver = setTimeout(function () {
            e.target.timeOutForDragOver = null;
            if (e.target.dotnetAdapter) {
                e.target.dotnetAdapter.invokeMethodAsync("OnDragOverHandler", e);
            }
        }.bind(this), 250);
    }
}