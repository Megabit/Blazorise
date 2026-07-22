import { getRequiredElement } from "./utilities.js?v=2.2.2.0";

const instances = new Map();

export function initialize(dotNetObjectRef, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    destroy(element, elementId);

    const instance = {
        dotNetObjectRef: dotNetObjectRef,
        element: element,
        target: null,
        options: normalizeOptions(options),
        active: false,
        documentObserverScope: null,
        pointerId: null,
        pointerType: "",
        startCoordinate: 0,
        startSize: 0,
        currentSize: 0,
        lastResizeNotification: 0,
        resizeObserver: null,
        originalBodyCursor: "",
        originalBodyUserSelect: "",
        onPointerDown: null,
        onKeyDown: null
    };

    instance.onPointerDown = (event) => pointerDownHandler(instance, event);
    instance.onKeyDown = (event) => keyDownHandler(instance, event);

    element.addEventListener("pointerdown", instance.onPointerDown);
    element.addEventListener("keydown", instance.onKeyDown);

    instances.set(elementId, instance);
    resolveTarget(instance);
    synchronizeSize(instance, true);
}

export function updateOptions(element, elementId, options) {
    const instance = instances.get(elementId);

    if (!instance)
        return;

    const previousOptions = instance.options;
    const previousTargetElementId = previousOptions.targetElementId;

    if (instance.active) {
        toggleClassNames(instance.element, previousOptions.resizingClassNames, false);
        toggleClassNames(instance.target, previousOptions.targetResizingClassNames, false);
    }

    instance.options = normalizeOptions(options);

    if (previousTargetElementId !== instance.options.targetElementId)
        resolveTarget(instance);

    if (instance.active) {
        toggleClassNames(instance.element, instance.options.resizingClassNames, true);
        toggleClassNames(instance.target, instance.options.targetResizingClassNames, true);
    }

    if (instance.options.disabled && instance.active)
        cancelResize(instance, true);
    else if (!instance.active)
        synchronizeSize(instance, true);
}

export function destroy(element, elementId) {
    const instance = instances.get(elementId);

    if (!instance)
        return;

    if (instance.active)
        cancelResize(instance, false);

    instance.resizeObserver?.disconnect();

    instance.element?.removeEventListener("pointerdown", instance.onPointerDown);
    instance.element?.removeEventListener("keydown", instance.onKeyDown);

    instance.dotNetObjectRef = null;
    instance.element = null;
    instance.target = null;

    instances.delete(elementId);
}

function pointerDownHandler(instance, event) {
    if (instance.options.disabled || !instance.target || !instance.dotNetObjectRef)
        return;

    if (event.isPrimary === false)
        return;

    if (event.pointerType === "mouse" && event.button !== 0)
        return;

    const documentObserverScope = globalThis.Blazorise?.documentObserver?.createScope("resize-handle") ?? null;

    if (!documentObserverScope)
        return;

    event.preventDefault();

    instance.active = true;
    instance.pointerId = event.pointerId;
    instance.pointerType = event.pointerType || "";
    instance.startCoordinate = getPointerCoordinate(instance, event);
    instance.startSize = measureTarget(instance);
    instance.currentSize = instance.startSize;
    instance.lastResizeNotification = 0;

    instance.element.focus({ preventScroll: true });

    if (instance.element.setPointerCapture) {
        try {
            instance.element.setPointerCapture(event.pointerId);
        }
        catch {
        }
    }

    addDocumentListeners(instance, documentObserverScope);
    beginResizeState(instance);

    if (instance.options.notifyResizeStarted)
        notify(instance, "OnResizeStarted", createEventArgs(instance, false));
}

function pointerMoveHandler(instance, event) {
    if (!instance.active || event.pointerId !== instance.pointerId)
        return;

    applyPointerSize(instance, event);
    notifyResizing(instance, performance.now());
}

function finishPointerResize(instance, event, canceled) {
    if (!instance.active || event.pointerId !== instance.pointerId)
        return;

    if (canceled) {
        applySize(instance, instance.startSize);
    }
    else {
        applyPointerSize(instance, event);
    }

    const eventArgs = createEventArgs(instance, canceled);

    completeResize(instance, eventArgs, instance.options.notifyResizeEnded);
}

function keyDownHandler(instance, event) {
    if (instance.active && event.key === "Escape") {
        event.preventDefault();
        cancelResize(instance, true);
        return;
    }

    if (instance.options.disabled || !instance.target || instance.active)
        return;

    let nextSize = null;
    const currentSize = measureTarget(instance);
    const physicalDelta = getKeyboardPhysicalDelta(instance, event.key);

    if (physicalDelta !== null) {
        nextSize = currentSize + physicalDelta * getResizeSign(instance);
    }
    else if (event.key === "Home") {
        nextSize = instance.options.minSize;
    }
    else if (event.key === "End" && instance.options.maxSize !== null) {
        nextSize = instance.options.maxSize;
    }

    if (nextSize === null)
        return;

    event.preventDefault();

    instance.pointerType = "keyboard";
    instance.startSize = currentSize;
    instance.currentSize = currentSize;

    if (instance.options.notifyResizeStarted)
        notify(instance, "OnResizeStarted", createEventArgs(instance, false));

    applySize(instance, nextSize);

    if (instance.options.notifyResizing)
        notify(instance, "OnResizing", createEventArgs(instance, false));

    if (instance.options.notifyResizeEnded)
        notify(instance, "OnResizeEnded", createEventArgs(instance, false));

    resetInteraction(instance);
}

function applyPointerSize(instance, event) {
    const physicalDelta = getPointerCoordinate(instance, event) - instance.startCoordinate;
    const nextSize = instance.startSize + physicalDelta * getResizeSign(instance);

    applySize(instance, nextSize);
}

function applySize(instance, size) {
    if (!instance.target)
        return;

    const clampedSize = clampSize(instance, size);

    instance.target.style.setProperty(instance.options.resizeProperty, `${clampedSize}px`);
    instance.currentSize = clampedSize;

    updateAriaValue(instance);
}

function synchronizeSize(instance, applyControlledSize) {
    if (!instance.target)
        return;

    if (applyControlledSize && instance.options.size !== null)
        applySize(instance, instance.options.size);
    else {
        instance.currentSize = measureTarget(instance);
        updateAriaValue(instance);
    }
}

function measureTarget(instance) {
    if (!instance.target)
        return 0;

    const bounds = instance.target.getBoundingClientRect();
    const measuredSize = instance.options.vertical ? bounds.width : bounds.height;

    return clampSize(instance, measuredSize);
}

function clampSize(instance, size) {
    const finiteSize = Number.isFinite(size) ? size : instance.options.minSize;
    const maximum = instance.options.maxSize === null ? Number.POSITIVE_INFINITY : instance.options.maxSize;

    return Math.min(Math.max(finiteSize, instance.options.minSize), maximum);
}

function getPointerCoordinate(instance, event) {
    return instance.options.vertical ? event.clientX : event.clientY;
}

function getResizeSign(instance) {
    let axisDirection = 1;

    if (instance.options.vertical && instance.target && getComputedStyle(instance.target).direction === "rtl")
        axisDirection = -1;

    return instance.options.resizeFromStart ? -axisDirection : axisDirection;
}

function getKeyboardPhysicalDelta(instance, key) {
    if (instance.options.vertical) {
        if (key === "ArrowLeft")
            return -instance.options.keyboardStep;

        if (key === "ArrowRight")
            return instance.options.keyboardStep;
    }
    else {
        if (key === "ArrowUp")
            return -instance.options.keyboardStep;

        if (key === "ArrowDown")
            return instance.options.keyboardStep;
    }

    return null;
}

function beginResizeState(instance) {
    toggleClassNames(instance.element, instance.options.resizingClassNames, true);
    toggleClassNames(instance.target, instance.options.targetResizingClassNames, true);

    instance.originalBodyCursor = document.body.style.cursor || "";
    instance.originalBodyUserSelect = document.body.style.userSelect || "";

    document.body.style.cursor = getComputedStyle(instance.element).cursor;
    document.body.style.userSelect = "none";
}

function endResizeState(instance) {
    toggleClassNames(instance.element, instance.options.resizingClassNames, false);
    toggleClassNames(instance.target, instance.options.targetResizingClassNames, false);

    document.body.style.cursor = instance.originalBodyCursor;
    document.body.style.userSelect = instance.originalBodyUserSelect;
}

function cancelResize(instance, notifyEnded) {
    if (!instance.active)
        return;

    applySize(instance, instance.startSize);

    const eventArgs = createEventArgs(instance, true);

    completeResize(instance, eventArgs, notifyEnded && instance.options.notifyResizeEnded);
}

function completeResize(instance, eventArgs, notifyEnded) {
    releasePointerCapture(instance);
    removeDocumentListeners(instance);
    endResizeState(instance);
    resetInteraction(instance);

    if (notifyEnded)
        notify(instance, "OnResizeEnded", eventArgs);
}

function addDocumentListeners(instance, documentObserverScope) {
    instance.documentObserverScope = documentObserverScope;

    documentObserverScope.subscribe({
        eventNames: ["pointermove"],
        capture: true,
        preventDefault: true,
        throttle: true,
        handler: (event) => pointerMoveHandler(instance, event)
    });

    documentObserverScope.subscribe({
        eventNames: ["pointerup", "pointercancel", "blur"],
        capture: true,
        preventDefault: true,
        handler: (event) => event.type === "blur"
            ? cancelResize(instance, true)
            : finishPointerResize(instance, event, event.type === "pointercancel")
    });

    documentObserverScope.capturePointer(instance.pointerId);
}

function removeDocumentListeners(instance) {
    instance.documentObserverScope?.dispose();
    instance.documentObserverScope = null;
}

function releasePointerCapture(instance) {
    if (!instance.element?.releasePointerCapture || instance.pointerId === null)
        return;

    try {
        instance.element.releasePointerCapture(instance.pointerId);
    }
    catch {
    }
}

function resetInteraction(instance) {
    instance.active = false;
    instance.pointerId = null;
    instance.pointerType = "";
    instance.startCoordinate = 0;
    instance.lastResizeNotification = 0;
}

function notifyResizing(instance, timestamp) {
    if (!instance.options.notifyResizing)
        return;

    if (instance.options.resizeEventInterval > 0 && timestamp - instance.lastResizeNotification < instance.options.resizeEventInterval)
        return;

    instance.lastResizeNotification = timestamp;
    notify(instance, "OnResizing", createEventArgs(instance, false));
}

function notify(instance, method, eventArgs) {
    instance.dotNetObjectRef?.invokeMethodAsync(method, eventArgs);
}

function createEventArgs(instance, canceled) {
    return {
        size: instance.currentSize,
        previousSize: instance.startSize,
        delta: instance.currentSize - instance.startSize,
        pointerType: instance.pointerType,
        canceled: canceled
    };
}

function resolveTarget(instance) {
    instance.resizeObserver?.disconnect();
    instance.resizeObserver = null;

    instance.target = instance.options.targetElementId
        ? document.getElementById(instance.options.targetElementId)
        : instance.element?.parentElement;

    if (!instance.target || typeof ResizeObserver === "undefined")
        return;

    instance.resizeObserver = new ResizeObserver(() => {
        if (!instance.active)
            synchronizeSize(instance, false);
    });

    instance.resizeObserver.observe(instance.target);
}

function updateAriaValue(instance) {
    instance.element?.setAttribute("aria-valuenow", formatNumber(instance.currentSize));
}

function toggleClassNames(element, classNames, active) {
    if (!element || !classNames)
        return;

    const tokens = classNames.split(/\s+/).filter(Boolean);

    for (const token of tokens)
        element.classList.toggle(token, active);
}

function normalizeOptions(options) {
    options = options || {};

    const minimum = Math.max(numberOrDefault(options.minSize, 0), 0);
    const maximumValue = nullableNumber(options.maxSize);
    const maximum = maximumValue === null ? null : Math.max(maximumValue, minimum);

    return {
        targetElementId: options.targetElementId || null,
        vertical: options.vertical === true,
        resizeFromStart: options.resizeFromStart === true,
        resizeProperty: options.resizeProperty || (options.vertical === true ? "width" : "height"),
        size: nullableNumber(options.size),
        minSize: minimum,
        maxSize: maximum,
        keyboardStep: Math.max(numberOrDefault(options.keyboardStep, 10), 0.0001),
        resizeEventInterval: Math.max(numberOrDefault(options.resizeEventInterval, 100), 0),
        disabled: options.disabled === true,
        resizingClassNames: options.resizingClassNames || "",
        targetResizingClassNames: options.targetResizingClassNames || "",
        notifyResizeStarted: options.notifyResizeStarted === true,
        notifyResizing: options.notifyResizing === true,
        notifyResizeEnded: options.notifyResizeEnded === true
    };
}

function nullableNumber(value) {
    if (value === null || typeof value === "undefined")
        return null;

    const number = Number(value);

    return Number.isFinite(number) ? number : null;
}

function numberOrDefault(value, defaultValue) {
    const number = Number(value);

    return Number.isFinite(number) ? number : defaultValue;
}

function formatNumber(value) {
    return Number(value.toFixed(4)).toString();
}