import { getRequiredElement } from "./utilities.js?v=1.2.4.0";
//TODO : JS resource
import { computePosition, autoUpdate, flip, shift, limitShift } from 'https://cdn.jsdelivr.net/npm/@floating-ui/dom@1.4.5/+esm';


const _instances = [];
const DIRECTION_DEFAULT = 'Default'
const DIRECTION_DOWN = 'Down'
const DIRECTION_UP = 'Up'
const DIRECTION_END = 'End'
const DIRECTION_START = 'Start'

function getPlacementDirection(direction, rightAligned) {
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


    //TODO : Shared init
    //https://floating-ui.com/docs/autoUpdate
    const instanceCleanupFunction = autoUpdate(targetElement, menuElement, () => {
        computePosition(targetElement, menuElement, { //https://floating-ui.com/docs/computePosition#anchoring
            placement: getPlacementDirection(options.direction, options.rightAligned), //https://floating-ui.com/docs/computePosition#placement
            strategy: options.strategy, //https://floating-ui.com/docs/computePosition#strategy
            middleware: [flip(), shift({ padding: 0, limiter: limitShift() })] //https://floating-ui.com/docs/computePosition#middleware
        }).then(({ x, y }) => {
            Object.assign(menuElement.style, {
                left: `${x}px`,
                top: `${y}px`
            });
        });
    });

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