import { getRequiredElement } from "./utilities.js?v=1.0.2.0";
import { createPopper } from "./popper.js?v=1.0.2.0";

const _instances = [];

const DIRECTION_DEFAULT = 'Default'
const DIRECTION_DOWN = 'Down'
const DIRECTION_UP = 'Up'
const DIRECTION_END = 'End'
const DIRECTION_START = 'Start'

function getPopperDirection(direction) {
    if (direction == DIRECTION_DEFAULT || direction == DIRECTION_DOWN)
        return "bottom-start";
    else if (direction == DIRECTION_UP)
        return "top-start";
    else if (direction == DIRECTION_END)
        return "right-start";
    else if (direction == DIRECTION_START)
        return "left-start";

    return "bottom-start";
}

// optimize this
function createSelector(value) {
    const classNames = '.' + value.split(' ').filter(i => i).join('.');

    return classNames;
}

export function initialize(element, elementId, targetElementId, altTargetElementId, menuElementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const targetElement = (altTargetElementId || targetElementId)
        ? document.getElementById(altTargetElementId || targetElementId)
        : element.querySelector(createSelector(options.dropdownToggleClassNames));

    const menuElement = menuElementId
        ? document.getElementById(menuElementId)
        : element.querySelector(createSelector(options.dropdownMenuClassNames));

    const instance = createPopper(targetElement, menuElement, {
        placement: getPopperDirection(options.direction),
        strategy: "fixed",
        modifiers: [
            {
                name: "preventOverflow",
                options: {
                    padding: 0,
                }
            }]
    });

    instance.update();

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instances = _instances || {};

    const instance = instances[elementId];

    if (instance) {
        instance.destroy();

        delete instances[elementId];
    }
}

export function show(element, elementId) {
    const instance = getInstance(elementId);

    if (instance) {
        instance.update();
    }
}

export function hide(element, elementId) {
    const instance = getInstance(elementId);

    if (instance) {
        instance.update();
    }
}

export function getInstance(elementId) {
    const instances = _instances || {};

    return instances[elementId];
}