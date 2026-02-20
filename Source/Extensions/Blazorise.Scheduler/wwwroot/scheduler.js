import { getRequiredElement, insertCSSIntoDocumentHead } from "../Blazorise/utilities.js?v=2.0.0.0";

insertCSSIntoDocumentHead("_content/Blazorise.Scheduler/scheduler.css?v=2.0.0.0");

const _instances = {};

function findSchedulerSlotElement(startNode) {
    let node = startNode;

    while (node && node !== document) {
        if (node.hasAttribute?.("data-slot-start") && node.hasAttribute?.("data-slot-end")) {
            return node;
        }

        node = node.parentNode;
    }

    return null;
}

export async function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);
    if (!element) return;

    _instances[elementId] = {
        dotNetAdapter,
        element,
        mouseUpHandler: null
    };
}

export function selectionStarted(element, elementId) {
    const instance = _instances[elementId];
    if (!instance) {
        return;
    }

    if (instance.mouseUpHandler) {
        document.removeEventListener("mouseup", instance.mouseUpHandler);
    }

    // Track the initial day column where selection started
    const initialDayColumn = findDayColumn(element);

    const handler = async function (evt) {
        if (evt.button !== 0) return;

        const targetSlot = findSchedulerSlotElement(evt.target);
        const currentDayColumn = findDayColumn(evt.target);

        // Cancel if:
        // 1. We didn't land on a slot, or
        // 2. Landed on a different day column
        if (!targetSlot || !initialDayColumn || currentDayColumn !== initialDayColumn) {
            if (instance.dotNetAdapter) {
                await instance.dotNetAdapter.invokeMethodAsync("CancelSelection");
            }
        }

        // Auto-remove after firing once
        selectionEnded(element, elementId);
    };

    document.addEventListener("mouseup", handler);
    instance.mouseUpHandler = handler;
}

function findDayColumn(startNode) {
    let node = startNode;
    while (node && node !== document) {
        if (node.classList?.contains("b-scheduler-day")) {
            return node;
        }
        node = node.parentNode;
    }
    return null;
}

export function selectionEnded(element, elementId) {
    const instance = _instances[elementId];

    if (!instance || !instance.mouseUpHandler) {
        return;
    }

    document.removeEventListener("mouseup", instance.mouseUpHandler);
    instance.mouseUpHandler = null;
}

export function destroy(element, elementId) {
    const instance = _instances[elementId];

    if (!instance) {
        return;
    }

    if (instance.mouseUpHandler) {
        document.removeEventListener("mouseup", instance.mouseUpHandler);
    }

    delete _instances[elementId];
}