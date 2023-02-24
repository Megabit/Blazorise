import { getRequiredElement } from "./utilities.js?v=1.2.1.0";
import { createPopper } from "./popper.js?v=1.2.1.0";
import { createAttributesObserver, observeClassChanged, destroyObserver } from "./observer.js?v=1.2.1.0";

const _instances = [];
const DIRECTION_DEFAULT = 'Default'
const DIRECTION_DOWN = 'Down'
const DIRECTION_UP = 'Up'
const DIRECTION_END = 'End'
const DIRECTION_START = 'Start'

function getPopperDirection(direction, rightAligned) {
    let suffixAlignment = rightAligned ? "end" : "start";

    if (direction === DIRECTION_DEFAULT || direction === DIRECTION_DOWN)
        return 'bottom-' + suffixAlignment;
    else if (direction === DIRECTION_UP)
        return 'top-' + suffixAlignment;
    else if (direction === DIRECTION_END)
        return 'right-' + suffixAlignment;
    else if (direction === DIRECTION_START)
        return 'left-' + suffixAlignment;

    return 'bottom-' + suffixAlignment;
}

// optimize this
function createSelector(value) {
    const classNames = '.' + value.split(' ').filter(i => i).join('.');

    return classNames;
}

export function initialize(element, elementId, targetElementId, altTargetElementId, menuElementId, showElementId, options) {

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
        placement: getPopperDirection(options.direction, options.rightAligned),
        strategy: "fixed",
        modifiers: [
            {
                name: "preventOverflow",
                enabled: true,
                options: {
                    padding: 0,
                }
            }
        ]
    });

    instance.update();

    createAttributesObserver(showElementId, (mutationsList) => observeClassChanged(mutationsList, options.dropdownShowClassName, () => instance.update(), true));

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

        destroyObserver(elementId);
    }
}