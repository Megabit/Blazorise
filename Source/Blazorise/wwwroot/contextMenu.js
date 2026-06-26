import { getRequiredElement, registerDisconnectCleanup, unregisterDisconnectCleanup } from "./utilities.js?v=2.2.1.0";
import { createFloatingUiPointAutoUpdate } from './floatingUi.js?v=2.2.1.0';

const _instances = [];

export function initialize(element, elementId, menuElementId, clientX, clientY, contextElementSelector, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const menuElement = menuElementId
        ? document.getElementById(menuElementId)
        : element.querySelector('[role="menu"]');

    if (!menuElement)
        return;

    destroy(null, elementId, false);

    const contextElement = findContextElement(contextElementSelector) ?? element;

    const instanceCleanupFunction = createFloatingUiPointAutoUpdate(clientX, clientY, contextElement, menuElement, options);

    _instances[elementId] = {
        cleanupFunction: instanceCleanupFunction,
        disconnectCleanupId: registerDisconnectCleanup(element, () => destroy(null, elementId, false))
    };
}

export function destroy(element, elementId, unregisterCleanup = true) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        if (unregisterCleanup) {
            unregisterDisconnectCleanup(instance.disconnectCleanupId);
        }

        if (instance.cleanupFunction) {
            instance.cleanupFunction();
        }

        delete instances[elementId];
    }
}

function findContextElement(contextElementSelector) {
    if (!contextElementSelector) {
        return null;
    }

    try {
        return document.querySelector(contextElementSelector);
    }
    catch {
        return null;
    }
}