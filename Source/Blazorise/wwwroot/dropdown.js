import { getRequiredElement, registerDisconnectCleanup, unregisterDisconnectCleanup } from "./utilities.js?v=__BLAZORISE_VERSION__";
import { createFloatingUiAutoUpdate } from './floatingUi.js?v=__BLAZORISE_VERSION__';

const _instances = [];

function createSelector(value) {
    const classNames = '.' + value.trim().split(/\s+/).map(CSS.escape).join('.');
    return classNames;
}

export function initialize(element, elementId, targetElementId, menuElementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const targetElement = targetElementId
        ? document.getElementById(targetElementId)
        : element.querySelector(createSelector(options.dropdownToggleClassNames));

    const menuElement = menuElementId
        ? document.getElementById(menuElementId)
        : element.querySelector(createSelector(options.dropdownMenuClassNames));

    if (!targetElement || !menuElement)
        return;

    const instanceCleanupFunction = createFloatingUiAutoUpdate(targetElement, menuElement, options);

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