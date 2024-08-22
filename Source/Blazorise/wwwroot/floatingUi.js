import { computePosition, autoUpdate, flip, shift, limitShift } from './vendors/floating-ui.js?v=1.6.1.0';

const DIRECTION_DEFAULT = 'Default'
const DIRECTION_DOWN = 'Down'
const DIRECTION_UP = 'Up'
const DIRECTION_END = 'End'
const DIRECTION_START = 'Start'

export function createFloatingUiAutoUpdate(targetElement, menuElement, options) {
    //https://floating-ui.com/docs/autoUpdate
    return autoUpdate(targetElement, menuElement, () => {
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
}

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