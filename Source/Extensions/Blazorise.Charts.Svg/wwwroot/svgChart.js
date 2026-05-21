const zoomWheelHandlers = new WeakMap();

export function initializeZoomWheel(element) {
    if (!element || zoomWheelHandlers.has(element)) {
        return;
    }

    const handler = event => {
        if (event.cancelable) {
            event.preventDefault();
        }
    };

    element.addEventListener("wheel", handler, { passive: false });
    zoomWheelHandlers.set(element, handler);
}

export function destroyZoomWheel(element) {
    const handler = zoomWheelHandlers.get(element);

    if (!element || !handler) {
        return;
    }

    element.removeEventListener("wheel", handler, { passive: false });
    zoomWheelHandlers.delete(element);
}