import { getRequiredElement } from "../Blazorise/utilities.js?v=2.0.3.0";

const _instances = {};
const dragStartThreshold = 3;

function suppressNextClick() {
    let timeoutId = 0;

    const clickSuppressHandler = function (evt) {
        evt.preventDefault();
        evt.stopPropagation();

        if (typeof evt.stopImmediatePropagation === "function") {
            evt.stopImmediatePropagation();
        }

        if (timeoutId) {
            window.clearTimeout(timeoutId);
        }

        document.removeEventListener("click", clickSuppressHandler, true);
    };

    document.addEventListener("click", clickSuppressHandler, true);

    timeoutId = window.setTimeout(function () {
        document.removeEventListener("click", clickSuppressHandler, true);
    }, 0);
}

function clearDragHandlers(instance) {
    if (!instance) {
        return;
    }

    if (instance.mouseMoveHandler) {
        document.removeEventListener("mousemove", instance.mouseMoveHandler);
        instance.mouseMoveHandler = null;
    }

    if (instance.mouseUpHandler) {
        document.removeEventListener("mouseup", instance.mouseUpHandler);
        window.removeEventListener("blur", instance.mouseUpHandler);
        instance.mouseUpHandler = null;
    }

    instance.lastClientX = 0;
    instance.dragStartClientX = 0;
    instance.dragged = false;
    instance.moveScheduled = false;
}

export function initialize(dotNetAdapter, element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element) {
        return;
    }

    _instances[elementId] = {
        dotNetAdapter,
        element,
        mouseMoveHandler: null,
        mouseUpHandler: null,
        moveScheduled: false,
        lastClientX: 0,
        dragStartClientX: 0,
        dragged: false
    };
}

export function barDragStarted(element, elementId, startClientX) {
    const instance = _instances[elementId];

    if (!instance) {
        return;
    }

    clearDragHandlers(instance);
    instance.dragStartClientX = startClientX;
    instance.lastClientX = startClientX;
    instance.dragged = false;

    instance.mouseMoveHandler = function (evt) {
        instance.lastClientX = evt.clientX;

        if (!instance.dragged && Math.abs(instance.lastClientX - instance.dragStartClientX) >= dragStartThreshold) {
            instance.dragged = true;
        }

        if (instance.moveScheduled) {
            return;
        }

        instance.moveScheduled = true;

        window.requestAnimationFrame(function () {
            instance.moveScheduled = false;

            if (instance.dotNetAdapter) {
                instance.dotNetAdapter.invokeMethodAsync("NotifyBarDragMouseMove", instance.lastClientX);
            }
        });
    };

    instance.mouseUpHandler = function (evt) {
        const clientX = instance.lastClientX;
        const mouseUpClientX = typeof evt?.clientX === "number"
            ? evt.clientX
            : clientX;

        if (!instance.dragged && Math.abs(mouseUpClientX - instance.dragStartClientX) >= dragStartThreshold) {
            instance.dragged = true;
        }

        if (instance.dragged) {
            suppressNextClick();
        }

        if (instance.dotNetAdapter) {
            instance.dotNetAdapter.invokeMethodAsync("NotifyBarDragMouseUp", clientX, instance.dragged);
        }

        clearDragHandlers(instance);
    };

    document.addEventListener("mousemove", instance.mouseMoveHandler);
    document.addEventListener("mouseup", instance.mouseUpHandler);
    window.addEventListener("blur", instance.mouseUpHandler);
}

export function barDragEnded(element, elementId) {
    const instance = _instances[elementId];

    if (!instance) {
        return;
    }

    clearDragHandlers(instance);
}

export function destroy(element, elementId) {
    const instance = _instances[elementId];

    if (!instance) {
        return;
    }

    clearDragHandlers(instance);

    delete _instances[elementId];
}