// holds the list of components that are triggers to breakpoint
const breakpointComponents = [];
let lastBreakpoint = getBreakpoint();


if (isDocumentReady()) {
    bindWindowResizedBreakpointHandler();
}
else if (document.readyState === "loading") { //https://developer.mozilla.org/en-US/docs/Web/API/Document/DOMContentLoaded_event#checking_whether_loading_is_already_complete
    document.addEventListener("DOMContentLoaded", bindWindowResizedBreakpointHandler);
}

function bindWindowResizedBreakpointHandler() {
    if (window.attachEvent) {
        window.attachEvent('onresize', windowResizedBreakpointHandler);
    }
    else if (window.addEventListener) {
        window.addEventListener('resize', windowResizedBreakpointHandler, true);
    }
    else {
        //The browser does not support Javascript event binding
    }
}

function windowResizedBreakpointHandler() {
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

function isDocumentReady() {
    return document.readyState === 'interactive' || document.readyState === 'complete';
}

function onBreakpoint(dotnetAdapter, currentBreakpoint) {
    dotnetAdapter.invokeMethodAsync('OnBreakpoint', currentBreakpoint);
}

export function getBreakpoint() {
    if (isDocumentReady()) {
        return window.getComputedStyle(document.body, ':before').content.replace(/\"/g, '');
    }
    return "";
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