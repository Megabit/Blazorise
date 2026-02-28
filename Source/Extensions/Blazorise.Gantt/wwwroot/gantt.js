import { getRequiredElement } from "../Blazorise/utilities.js?v=2.0.1.0";

const _instances = {};

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
        lastClientX: 0
    };
}

export function barDragStarted(element, elementId) {
    const instance = _instances[elementId];

    if (!instance) {
        return;
    }

    clearDragHandlers(instance);

    instance.mouseMoveHandler = function (evt) {
        instance.lastClientX = evt.clientX;

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

    instance.mouseUpHandler = function () {
        if (instance.dotNetAdapter) {
            instance.dotNetAdapter.invokeMethodAsync("NotifyBarDragMouseUp");
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