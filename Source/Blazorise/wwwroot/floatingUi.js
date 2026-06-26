import { computePosition, autoUpdate, flip, shift, limitShift, hide } from './vendors/floating-ui.js?v=2.2.1.0';

const DIRECTION_DEFAULT = 'Default'
const DIRECTION_DOWN = 'Down'
const DIRECTION_UP = 'Up'
const DIRECTION_END = 'End'
const DIRECTION_START = 'Start'

export function createFloatingUiAutoUpdate(targetElement, menuElement, options) {
    //https://floating-ui.com/docs/autoUpdate
    return autoUpdate(targetElement, menuElement, () => {
        if (!shouldUseFloatingUi(options, menuElement)) {
            menuElement.style.left = '';
            menuElement.style.top = '';
            menuElement.style.visibility = '';

            return;
        }

        computePosition(targetElement, menuElement, { //https://floating-ui.com/docs/computePosition#anchoring
            placement: getPlacementDirection(options.direction, options.endAligned), //https://floating-ui.com/docs/computePosition#placement
            strategy: options.strategy, //https://floating-ui.com/docs/computePosition#strategy
            middleware: [flip(), shift({ padding: 0, limiter: limitShift() }), hide()] //https://floating-ui.com/docs/computePosition#middleware
        }).then(({ x, y, middlewareData }) => {
            const { referenceHidden, escaped } = middlewareData.hide ?? {};
            Object.assign(menuElement.style, {
                left: `${x}px`,
                top: `${y}px`,
                visibility: referenceHidden || escaped ? 'hidden' : 'visible'
            });
        });
    });
}

export function createFloatingUiPointAutoUpdate(clientX, clientY, contextElement, menuElement, options) {
    const contextRect = contextElement?.getBoundingClientRect?.();
    const offsetX = contextRect ? clientX - contextRect.left : 0;
    const offsetY = contextRect ? clientY - contextRect.top : 0;
    const targetElement = {
        contextElement: contextElement ?? undefined,
        getBoundingClientRect() {
            const point = getVirtualPoint(clientX, clientY, contextElement, offsetX, offsetY);

            return {
                x: point.x,
                y: point.y,
                left: point.x,
                top: point.y,
                right: point.x,
                bottom: point.y,
                width: 0,
                height: 0,
            };
        }
    };

    return createFloatingUiAutoUpdate(targetElement, menuElement, options);
}

function getVirtualPoint(clientX, clientY, contextElement, offsetX, offsetY) {
    const contextRect = contextElement?.getBoundingClientRect?.();

    if (!contextRect) {
        return {
            x: clientX,
            y: clientY,
        };
    }

    return {
        x: contextRect.left + offsetX,
        y: contextRect.top + offsetY,
    };
}

function shouldUseFloatingUi(options, menuElement) {
    if (!options?.onlyWhenPositioned)
        return true;

    const position = getComputedStyle(menuElement).position;

    return position === 'absolute' || position === 'fixed';
}

function getPlacementDirection(direction, endAligned) {
    let suffixAlignment = endAligned ? "end" : "start";

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