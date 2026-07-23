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
        startTarget: null,
        endTarget: null,
        startResizeElement: null,
        endResizeElement: null,
        options: normalizeOptions(options),
        active: false,
        focusVisible: false,
        documentObserverScope: null,
        pointerId: null,
        pointerType: "",
        startCoordinate: 0,
        startSize: 0,
        currentSize: 0,
        startEndSize: null,
        currentEndSize: null,
        startOriginalPropertyValue: null,
        endOriginalPropertyValue: null,
        lastResizeNotification: 0,
        resizeObserver: null,
        originalBodyCursor: "",
        originalBodyUserSelect: "",
        onPointerDown: null,
        onKeyDown: null,
        onFocus: null,
        onBlur: null
    };

    instance.onPointerDown = (event) => pointerDownHandler(instance, event);
    instance.onKeyDown = (event) => keyDownHandler(instance, event);
    instance.onFocus = () => focusHandler(instance);
    instance.onBlur = () => setFocusVisibleState(instance, false);

    element.addEventListener("pointerdown", instance.onPointerDown);
    element.addEventListener("keydown", instance.onKeyDown);
    element.addEventListener("focus", instance.onFocus);
    element.addEventListener("blur", instance.onBlur);

    instances.set(elementId, instance);
    resolveTargets(instance);
    synchronizeSize(instance, true);
}

export function updateOptions(element, elementId, options) {
    const instance = instances.get(elementId);

    if (!instance)
        return;

    const previousOptions = instance.options;
    const previousTargetSignature = getTargetSignature(previousOptions);

    if (instance.focusVisible)
        toggleClassNames(instance.element, previousOptions.focusedClassNames, false);

    if (instance.active) {
        toggleClassNames(instance.element, previousOptions.resizingClassNames, false);
        toggleTargetClassNames(instance, previousOptions.targetResizingClassNames, false);
    }

    instance.options = normalizeOptions(options);

    if (instance.options.disabled)
        instance.focusVisible = false;
    else if (instance.focusVisible)
        toggleClassNames(instance.element, instance.options.focusedClassNames, true);

    if (previousTargetSignature !== getTargetSignature(instance.options))
        resolveTargets(instance);

    if (instance.active) {
        toggleClassNames(instance.element, instance.options.resizingClassNames, true);
        toggleTargetClassNames(instance, instance.options.targetResizingClassNames, true);
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

    setFocusVisibleState(instance, false);

    instance.element?.removeEventListener("pointerdown", instance.onPointerDown);
    instance.element?.removeEventListener("keydown", instance.onKeyDown);
    instance.element?.removeEventListener("focus", instance.onFocus);
    instance.element?.removeEventListener("blur", instance.onBlur);

    instance.dotNetObjectRef = null;
    instance.element = null;
    instance.target = null;
    instance.startTarget = null;
    instance.endTarget = null;
    instance.startResizeElement = null;
    instance.endResizeElement = null;

    instances.delete(elementId);
}

function pointerDownHandler(instance, event) {
    if (instance.options.disabled || !instance.dotNetObjectRef)
        return;

    if (event.isPrimary === false)
        return;

    if (event.pointerType === "mouse" && event.button !== 0)
        return;

    resolveTargets(instance);

    if (!hasResolvedTargets(instance))
        return;

    const documentObserverScope = globalThis.Blazorise?.documentObserver?.createScope("resizer") ?? null;

    if (!documentObserverScope)
        return;

    event.preventDefault();

    instance.active = true;
    instance.pointerId = event.pointerId;
    instance.pointerType = event.pointerType || "";
    instance.startCoordinate = getPointerCoordinate(instance, event);
    captureInteractionSizes(instance);
    instance.lastResizeNotification = 0;

    setFocusVisibleState(instance, false);
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
        restoreInteractionSizes(instance);
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

    if (instance.options.disabled || instance.active)
        return;

    resolveTargets(instance);

    if (!hasResolvedTargets(instance))
        return;

    let nextSize = null;
    captureInteractionSizes(instance);

    const currentSize = instance.currentSize;
    const physicalDelta = getKeyboardPhysicalDelta(instance, event.key);
    const sizeRange = getSizeRange(instance);

    if (physicalDelta !== null) {
        nextSize = currentSize + physicalDelta * getResizeSign(instance);
    }
    else if (event.key === "Home") {
        nextSize = sizeRange.minimum;
    }
    else if (event.key === "End" && Number.isFinite(sizeRange.maximum)) {
        nextSize = sizeRange.maximum;
    }

    if (nextSize === null)
        return;

    event.preventDefault();

    setFocusVisibleState(instance, true);
    instance.pointerType = "keyboard";

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

function captureInteractionSizes(instance) {
    if (hasCoordinatedTargets(instance)) {
        instance.startSize = measureElement(instance.startTarget, instance.options.vertical);
        instance.currentSize = instance.startSize;
        instance.startEndSize = measureElement(instance.endTarget, instance.options.vertical);
        instance.currentEndSize = instance.startEndSize;
        instance.startOriginalPropertyValue = instance.startResizeElement.style.getPropertyValue(instance.options.targets.start.resizeProperty);
        instance.endOriginalPropertyValue = instance.endResizeElement.style.getPropertyValue(instance.options.targets.end.resizeProperty);
    }
    else {
        instance.startSize = measureTarget(instance);
        instance.currentSize = instance.startSize;
        instance.startEndSize = null;
        instance.currentEndSize = null;
        instance.startOriginalPropertyValue = null;
        instance.endOriginalPropertyValue = null;
    }
}

function restoreInteractionSizes(instance) {
    if (hasCoordinatedTargets(instance)) {
        restoreStyleProperty(instance.startResizeElement, instance.options.targets.start.resizeProperty, instance.startOriginalPropertyValue);
        restoreStyleProperty(instance.endResizeElement, instance.options.targets.end.resizeProperty, instance.endOriginalPropertyValue);
        instance.currentSize = instance.startSize;
        instance.currentEndSize = instance.startEndSize;
        updateAriaValue(instance);
    }
    else {
        applySize(instance, instance.startSize);
    }
}

function applySize(instance, size) {
    if (!hasResolvedTargets(instance))
        return;

    const clampedSize = clampSize(instance, size);

    if (hasCoordinatedTargets(instance)) {
        const totalSize = instance.startSize + instance.startEndSize;
        const endSize = totalSize - clampedSize;

        instance.startResizeElement.style.setProperty(instance.options.targets.start.resizeProperty, `${clampedSize}px`);
        instance.endResizeElement.style.setProperty(instance.options.targets.end.resizeProperty, `${endSize}px`);
        instance.currentEndSize = endSize;
    }
    else {
        instance.target.style.setProperty(instance.options.resizeProperty, `${clampedSize}px`);
    }

    instance.currentSize = clampedSize;

    updateAriaValue(instance);
}

function synchronizeSize(instance, applyControlledSize) {
    if (!hasResolvedTargets(instance))
        return;

    if (hasCoordinatedTargets(instance)) {
        instance.startSize = measureElement(instance.startTarget, instance.options.vertical);
        instance.currentSize = instance.startSize;
        instance.startEndSize = measureElement(instance.endTarget, instance.options.vertical);
        instance.currentEndSize = instance.startEndSize;
        updateAriaValue(instance);
    }
    else if (applyControlledSize && instance.options.size !== null)
        applySize(instance, instance.options.size);
    else {
        instance.currentSize = measureTarget(instance);
        updateAriaValue(instance);
    }
}

function measureTarget(instance) {
    if (!instance.target)
        return 0;

    const measuredSize = measureElement(instance.target, instance.options.vertical);

    return clampSize(instance, measuredSize);
}

function clampSize(instance, size) {
    const range = getSizeRange(instance);
    const finiteSize = Number.isFinite(size) ? size : range.minimum;

    return Math.min(Math.max(finiteSize, range.minimum), range.maximum);
}

function getSizeRange(instance) {
    if (!hasCoordinatedTargets(instance)) {
        return {
            minimum: instance.options.minSize,
            maximum: instance.options.maxSize === null ? Number.POSITIVE_INFINITY : instance.options.maxSize
        };
    }

    const totalSize = instance.startSize + instance.startEndSize;
    const startMinimum = parseTargetSize(instance.options.targets.start.minSize, instance.startTarget, instance.options.vertical, 0);
    const startMaximum = parseTargetSize(instance.options.targets.start.maxSize, instance.startTarget, instance.options.vertical, Number.POSITIVE_INFINITY);
    const endMinimum = parseTargetSize(instance.options.targets.end.minSize, instance.endTarget, instance.options.vertical, 0);
    const endMaximum = parseTargetSize(instance.options.targets.end.maxSize, instance.endTarget, instance.options.vertical, Number.POSITIVE_INFINITY);
    const minimum = Math.min(Math.max(startMinimum, Number.isFinite(endMaximum) ? totalSize - endMaximum : 0), totalSize);
    const maximum = Math.max(Math.min(startMaximum, totalSize - endMinimum), minimum);

    return { minimum, maximum };
}

function measureElement(element, vertical) {
    if (!element)
        return 0;

    const bounds = element.getBoundingClientRect();

    return vertical ? bounds.width : bounds.height;
}

function parseTargetSize(value, context, vertical, fallback) {
    value = resolveTargetSizeValue(value, context);

    if (!value || value.toLowerCase() === "none")
        return fallback;

    if (value.endsWith("px")) {
        const parsed = Number.parseFloat(value);

        return Number.isNaN(parsed) ? fallback : parsed;
    }

    const probe = document.createElement("div");

    probe.style.position = "absolute";
    probe.style.visibility = "hidden";
    probe.style.pointerEvents = "none";

    if (vertical) {
        probe.style.width = value;
        probe.style.height = "0";
    }
    else {
        probe.style.width = "0";
        probe.style.height = value;
    }

    const resolvedProperty = vertical ? probe.style.width : probe.style.height;

    if (!resolvedProperty)
        return fallback;

    (context?.parentElement || document.body).appendChild(probe);

    const result = measureElement(probe, vertical);

    probe.remove();

    return Number.isFinite(result) ? result : fallback;
}

function resolveTargetSizeValue(value, context) {
    const variable = value?.match(/^var\(\s*(--[^,\s)]+)\s*(?:,\s*(.+))?\)$/);

    if (!variable)
        return value;

    const resolvedValue = context
        ? getComputedStyle(context).getPropertyValue(variable[1]).trim()
        : "";

    return resolvedValue || variable[2]?.trim() || "";
}

function restoreStyleProperty(element, property, value) {
    if (!element || !property)
        return;

    if (value)
        element.style.setProperty(property, value);
    else
        element.style.removeProperty(property);
}

function getPointerCoordinate(instance, event) {
    return instance.options.vertical ? event.clientX : event.clientY;
}

function getResizeSign(instance) {
    let axisDirection = 1;

    const directionTarget = hasCoordinatedTargets(instance) ? instance.startTarget : instance.target;

    if (instance.options.vertical && directionTarget && getComputedStyle(directionTarget).direction === "rtl")
        axisDirection = -1;

    return hasCoordinatedTargets(instance) || !instance.options.resizeFromStart ? axisDirection : -axisDirection;
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
    toggleTargetClassNames(instance, instance.options.targetResizingClassNames, true);

    instance.originalBodyCursor = document.body.style.cursor || "";
    instance.originalBodyUserSelect = document.body.style.userSelect || "";

    document.body.style.cursor = getComputedStyle(instance.element).cursor;
    document.body.style.userSelect = "none";
}

function endResizeState(instance) {
    toggleClassNames(instance.element, instance.options.resizingClassNames, false);
    toggleTargetClassNames(instance, instance.options.targetResizingClassNames, false);

    document.body.style.cursor = instance.originalBodyCursor;
    document.body.style.userSelect = instance.originalBodyUserSelect;
}

function cancelResize(instance, notifyEnded) {
    if (!instance.active)
        return;

    restoreInteractionSizes(instance);

    const eventArgs = createEventArgs(instance, true);

    completeResize(instance, eventArgs, notifyEnded && instance.options.notifyResizeEnded);
}

function completeResize(instance, eventArgs, notifyEnded) {
    releasePointerCapture(instance);
    removeDocumentListeners(instance);
    endResizeState(instance);
    setFocusVisibleState(instance, false);
    resetInteraction(instance);

    if (notifyEnded)
        notify(instance, "OnResizeEnded", eventArgs);
}

function focusHandler(instance) {
    if (instance.pointerType)
        return;

    setFocusVisibleState(instance, instance.element?.matches(":focus-visible") === true);
}

function setFocusVisibleState(instance, focusVisible) {
    if (instance.focusVisible === focusVisible)
        return;

    instance.focusVisible = focusVisible;
    toggleClassNames(instance.element, instance.options.focusedClassNames, focusVisible);
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
    instance.startOriginalPropertyValue = null;
    instance.endOriginalPropertyValue = null;
    instance.lastResizeNotification = 0;
}

function notifyResizing(instance, timestamp) {
    if (!instance.options.notifyResizing)
        return;

    if (instance.options.resizingInterval > 0 && timestamp - instance.lastResizeNotification < instance.options.resizingInterval)
        return;

    instance.lastResizeNotification = timestamp;
    notify(instance, "OnResizing", createEventArgs(instance, false));
}

function notify(instance, method, eventArgs) {
    instance.dotNetObjectRef?.invokeMethodAsync(method, eventArgs);
}

function createEventArgs(instance, canceled) {
    return {
        startSize: instance.currentSize,
        previousStartSize: instance.startSize,
        endSize: instance.currentEndSize,
        previousEndSize: instance.startEndSize,
        size: instance.currentSize,
        previousSize: instance.startSize,
        delta: instance.currentSize - instance.startSize,
        pointerType: instance.pointerType,
        canceled: canceled
    };
}

function toggleTargetClassNames(instance, classNames, active) {
    if (hasCoordinatedTargets(instance)) {
        toggleClassNames(instance.startTarget, classNames, active);
        toggleClassNames(instance.endTarget, classNames, active);
    }
    else {
        toggleClassNames(instance.target, classNames, active);
    }
}

function resolveTargets(instance) {
    instance.resizeObserver?.disconnect();
    instance.resizeObserver = null;

    instance.startTarget = instance.options.targets?.start?.elementId
        ? document.getElementById(instance.options.targets.start.elementId)
        : null;
    instance.endTarget = instance.options.targets?.end?.elementId
        ? document.getElementById(instance.options.targets.end.elementId)
        : null;
    instance.startResizeElement = instance.options.targets?.start?.resizeElementId
        ? document.getElementById(instance.options.targets.start.resizeElementId)
        : instance.startTarget;
    instance.endResizeElement = instance.options.targets?.end?.resizeElementId
        ? document.getElementById(instance.options.targets.end.resizeElementId)
        : instance.endTarget;

    instance.target = instance.options.targetId
        ? document.getElementById(instance.options.targetId)
        : instance.element?.parentElement;

    if (!hasResolvedTargets(instance) || typeof ResizeObserver === "undefined")
        return;

    instance.resizeObserver = new ResizeObserver(() => {
        if (!instance.active)
            synchronizeSize(instance, false);
    });

    if (hasCoordinatedTargets(instance)) {
        instance.resizeObserver.observe(instance.startTarget);

        if (instance.endTarget !== instance.startTarget)
            instance.resizeObserver.observe(instance.endTarget);
    }
    else {
        instance.resizeObserver.observe(instance.target);
    }
}

function updateAriaValue(instance) {
    if (!instance.element)
        return;

    const range = getSizeRange(instance);

    instance.element.setAttribute("aria-valuemin", formatNumber(range.minimum));
    instance.element.setAttribute("aria-valuenow", formatNumber(instance.currentSize));

    if (Number.isFinite(range.maximum))
        instance.element.setAttribute("aria-valuemax", formatNumber(range.maximum));
    else
        instance.element.removeAttribute("aria-valuemax");
}

function hasResolvedTargets(instance) {
    return instance.options.targets
        ? hasCoordinatedTargets(instance)
        : !!instance.target;
}

function hasCoordinatedTargets(instance) {
    return !!instance.startTarget
        && !!instance.endTarget
        && !!instance.startResizeElement
        && !!instance.endResizeElement;
}

function getTargetSignature(options) {
    return JSON.stringify({ targetId: options.targetId, targets: options.targets });
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

    const minimum = Math.max(numberOrDefault(options.min, 0), 0);
    const maximumValue = nullableNumber(options.max);
    const maximum = maximumValue === null ? null : Math.max(maximumValue, minimum);

    return {
        targets: normalizeTargets(options.targets, options.vertical === true),
        targetId: options.targetId || null,
        vertical: options.vertical === true,
        resizeFromStart: options.resizeFromStart === true,
        resizeProperty: options.resizeProperty || (options.vertical === true ? "width" : "height"),
        size: nullableNumber(options.value),
        minSize: minimum,
        maxSize: maximum,
        keyboardStep: Math.max(numberOrDefault(options.keyboardStep, 10), 0.0001),
        resizingInterval: Math.max(numberOrDefault(options.resizingInterval, 100), 0),
        disabled: options.disabled === true,
        focusedClassNames: options.focusedClassNames || "",
        resizingClassNames: options.resizingClassNames || "",
        targetResizingClassNames: options.targetResizingClassNames || "",
        notifyResizeStarted: options.notifyResizeStarted === true,
        notifyResizing: options.notifyResizing === true,
        notifyResizeEnded: options.notifyResizeEnded === true
    };
}

function normalizeTargets(targets, vertical) {
    if (!targets?.start || !targets?.end)
        return null;

    const defaultProperty = vertical ? "width" : "height";

    return {
        start: normalizeTarget(targets.start, defaultProperty),
        end: normalizeTarget(targets.end, defaultProperty)
    };
}

function normalizeTarget(target, defaultProperty) {
    return {
        elementId: target.elementId || null,
        resizeElementId: target.resizeElementId || null,
        resizeProperty: target.resizeProperty || defaultProperty,
        minSize: target.minSize || null,
        maxSize: target.maxSize || null
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