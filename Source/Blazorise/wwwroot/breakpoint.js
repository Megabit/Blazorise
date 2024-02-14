// holds the list of components that are triggers to breakpoint
const breakpointComponents = [];
let lastBreakpoint = null;

// Recalculate breakpoint on resize
if (window.attachEvent) {
    window.attachEvent('onresize', windowResized);
}
else if (window.addEventListener) {
    window.addEventListener('resize', windowResized, true);
}
else {
    //The browser does not support Javascript event binding
}

function windowResized() {
    if (breakpointComponents && breakpointComponents.length > 0) {
        var currentBreakpoint = getBreakpoint();

        if (lastBreakpoint !== currentBreakpoint) {
            lastBreakpoint = currentBreakpoint;

            let index = 0;

            for (index = 0; index < breakpointComponents.length; ++index) {
                onBreakpoint(breakpointComponents[index].dotnetAdapter, currentBreakpoint);
            }
        }
    }
}

// Set initial breakpoint
lastBreakpoint = getBreakpoint();

// Get the current breakpoint
export function getBreakpoint() {
    return window.getComputedStyle(document.body, ':before').content.replace(/\"/g, '');
}

export function addBreakpointComponent(elementId, dotnetAdapter) {
    breakpointComponents.push({ elementId: elementId, dotnetAdapter: dotnetAdapter });
}

export function findBreakpointComponentIndex(elementId) {
    let index = 0;

    for (index = 0; index < breakpointComponents.length; ++index) {
        if (breakpointComponents[index].elementId === elementId)
            return index;
    }

    return -1;
}

export function isBreakpointComponent(elementId) {
    let index = 0;

    for (index = 0; index < breakpointComponents.length; ++index) {
        if (breakpointComponents[index].elementId === elementId)
            return true;
    }

    return false;
}

function onBreakpoint(dotnetAdapter, currentBreakpoint) {
    dotnetAdapter.invokeMethodAsync('OnBreakpoint', currentBreakpoint);
}

export function registerBreakpointComponent(dotnetAdapter, elementId) {
    if (isBreakpointComponent(elementId) !== true) {
        addBreakpointComponent(elementId, dotnetAdapter);
    }
}

export function unregisterBreakpointComponent(elementId) {
    const index = findBreakpointComponentIndex(elementId);
    if (index !== -1) {
        breakpointComponents.splice(index, 1);
    }
}