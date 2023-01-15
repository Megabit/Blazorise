import { getRequiredElement } from "./utilities.js?v=1.1.5.0";
let throttledDragHandler;
let throttledDragOverHandler;

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

export function initializeThrottledDragEvents(element, elementId, dotnetAdapter)
{
    element = getRequiredElement(element, elementId);
    if (!element)
        return;

    let timeOutForDrag = null;
    let timeOutForDragOver = null;

    throttledDragHandler = function (e) {
        e.preventDefault();
        if (!timeOutForDrag) {
            timeOutForDrag = setTimeout(function () {
                timeOutForDrag = null;
                dotnetAdapter.invokeMethodAsync("OnDragHandler", e)
            }.bind(this), 250);
        }
    }

    throttledDragOverHandler = function(e) {
        e.preventDefault();
        if (!timeOutForDragOver) {
            timeOutForDragOver = setTimeout(function () {
                timeOutForDragOver = null;
                dotnetAdapter.invokeMethodAsync("OnDragOverHandler", e)
            }.bind(this), 250);
        }
    }

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