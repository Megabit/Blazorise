function resetCanvasSize(canvas) {
    if (canvas && canvas.style) {
        canvas.style.removeProperty("width");
        canvas.style.removeProperty("height");
    }
}

function isResponsiveChart(instance) {
    if (!instance || !instance.chart || !instance.chart.options) {
        return false;
    }

    return instance.chart.options.responsive !== false;
}

function forceResize(instance, resetSize) {
    if (!instance || !instance.chart) {
        return;
    }

    const canvas = instance.canvas || instance.chart.canvas;

    if (resetSize && !instance.hasExplicitCanvasSize) {
        resetCanvasSize(canvas);
    }

    instance.chart.resize();
}

export function scheduleResponsiveResizePatch(instance, resetSize) {
    if (!instance) {
        return;
    }

    instance.pendingResetSize = instance.pendingResetSize || !!resetSize;

    if (instance.resizeRequestId) {
        return;
    }

    const schedule = typeof requestAnimationFrame === "function"
        ? requestAnimationFrame
        : (callback) => setTimeout(callback, 16);

    const cancel = typeof cancelAnimationFrame === "function"
        ? cancelAnimationFrame
        : clearTimeout;

    instance.cancelResizeRequest = cancel;
    instance.resizeRequestId = schedule(() => {
        instance.resizeRequestId = null;

        const shouldResetSize = instance.pendingResetSize;
        instance.pendingResetSize = false;

        forceResize(instance, shouldResetSize);
    });
}

export function setupResponsiveResizePatchHandlers(instance) {
    if (typeof window === "undefined") {
        return;
    }

    if (!instance || instance.resizeHandlersInitialized) {
        return;
    }

    instance.resizeHandlersInitialized = true;

    const onWindowResize = () => {
        if (isResponsiveChart(instance)) {
            scheduleResponsiveResizePatch(instance, true);
        }
    };

    window.addEventListener("resize", onWindowResize);
    instance.onWindowResize = onWindowResize;

    if ("ResizeObserver" in window && instance.canvas && instance.canvas.parentElement) {
        const resizeObserver = new ResizeObserver(() => {
            if (isResponsiveChart(instance)) {
                scheduleResponsiveResizePatch(instance, false);
            }
        });

        resizeObserver.observe(instance.canvas.parentElement);
        instance.resizeObserver = resizeObserver;
    }
}

export function cleanupResponsiveResizePatchHandlers(instance) {
    if (!instance) {
        return;
    }

    if (instance.resizeRequestId && instance.cancelResizeRequest) {
        instance.cancelResizeRequest(instance.resizeRequestId);
    }

    instance.resizeRequestId = null;
    instance.cancelResizeRequest = null;
    instance.pendingResetSize = false;

    if (typeof window !== "undefined" && instance.onWindowResize) {
        window.removeEventListener("resize", instance.onWindowResize);
    }

    instance.onWindowResize = null;

    if (instance.resizeObserver) {
        instance.resizeObserver.disconnect();
    }

    instance.resizeObserver = null;
    instance.resizeHandlersInitialized = false;
}