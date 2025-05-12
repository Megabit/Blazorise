import { getRequiredElement } from "../Blazorise/utilities.js?v=1.7.6.0";

const _instances = [];

document.addEventListener('mouseup', async function handler(evt) {
    if (evt.button !== 0) return;

    const slotElement = findSchedulerSlotElement(evt.target);

    if (slotElement) {
        // Valid slot clicked: do nothing
        return;
    }

    // Not a valid slot: notify all scheduler instances to cancel
    for (const elementId in _instances) {
        if (!_instances.hasOwnProperty(elementId)) {
            continue;
        }

        const instance = _instances[elementId];

        if (instance?.dotNetAdapter) {
            await instance.dotNetAdapter.invokeMethodAsync("CancelSelection");
        }
    }
});

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

    if (!element)
        return;

    const instance = {
        dotNetAdapter: dotNetAdapter,
        element: element
    };

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        delete instances[elementId];
    }
}