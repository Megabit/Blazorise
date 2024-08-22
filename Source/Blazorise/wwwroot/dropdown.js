import { getRequiredElement } from "./utilities.js?v=1.6.1.0";
import { createFloatingUiAutoUpdate } from './floatingUi.js?v=1.6.1.0';

const _instances = [];

function createSelector(value) {
    const classNames = '.' + value.split(' ').filter(i => i).join('.');

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

    const instanceCleanupFunction = createFloatingUiAutoUpdate(targetElement, menuElement, options);

    _instances[elementId] = instanceCleanupFunction;
}


export function destroy(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instances = _instances || {};

    const instanceCleanupFunction = instances[elementId];

    if (instanceCleanupFunction) {
        instanceCleanupFunction();
        delete instances[elementId];
    }
}