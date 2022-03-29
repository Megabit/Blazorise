import { getRequiredElement } from "./utilities.js?v=1.0.1.0";
import { createPopper } from "./popper.js?v=1.0.1.0";

const _instances = [];
const SHOW_CLASS = "show";

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

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const toggleElement = element.querySelector(createSelector(options.dropdownToggleClassNames));
    const menuElement = element.querySelector(createSelector(options.dropdownMenuClassNames));

    const instance = createPopper(toggleElement, menuElement, {
        placement: getPopperDirection(options.direction),
        strategy: "fixed",

        modifiers: [
            {
                name: "preventOverflow",
                options: {
                    scroll: true,
                    resize: true,
                    padding: 0,
                }
            }]
    });

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = getInstance(elementId);

    if (instance) {
        instance.destroy();

        delete instances[elementId];
    }
}

export function show(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = getInstance(elementId);

    if (instance) {
        instance.update();

        element.classList.add(SHOW_CLASS);
    }
}

export function hide(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = getInstance(elementId);

    if (instance) {
        instance.update();

        element.classList.remove(SHOW_CLASS);
    }
}

export function getInstance(elementId) {
    const instances = _instances || {};

    return instances[elementId];
}