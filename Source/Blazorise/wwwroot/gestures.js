import { getRequiredElement } from "./utilities.js?v=2.2.1.0";

const GestureDirection = {
    none: 0,
    left: 1,
    right: 2,
    up: 4,
    down: 8
};

const GestureTouchAction = {
    auto: 0,
    none: 1,
    panX: 2,
    panY: 3,
    manipulation: 4
};

let _instances = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    destroy(element, elementId);

    const instance = {
        dotnetAdapter: dotnetAdapter,
        element: element,
        elementId: elementId,
        options: normalizeOptions(options),
        originalTouchAction: element.style.touchAction || "",
        active: false,
        pointerId: null,
        startClientX: 0,
        startClientY: 0,
        startTime: 0,
        lastClientX: 0,
        lastClientY: 0,
        pointerType: "",
        longPressTimer: null,
        longPressFired: false,
        lastMoveNotification: 0,
        onPointerDown: null,
        onPointerMove: null,
        onPointerUp: null,
        onPointerCancel: null,
        onDragStart: null
    };

    instance.onPointerDown = (e) => pointerDownHandler(instance, e);
    instance.onPointerMove = (e) => pointerMoveHandler(instance, e);
    instance.onPointerUp = (e) => pointerUpHandler(instance, e, false);
    instance.onPointerCancel = (e) => pointerUpHandler(instance, e, true);
    instance.onDragStart = (e) => dragStartHandler(instance, e);

    element.addEventListener("pointerdown", instance.onPointerDown);
    element.addEventListener("pointermove", instance.onPointerMove);
    element.addEventListener("pointerup", instance.onPointerUp);
    element.addEventListener("pointercancel", instance.onPointerCancel);
    element.addEventListener("dragstart", instance.onDragStart);

    applyTouchAction(instance);

    _instances[elementId] = instance;
}

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    instance.options = normalizeOptions(options);

    applyTouchAction(instance);
}

export function destroy(element, elementId) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    if (instance.element) {
        instance.element.removeEventListener("pointerdown", instance.onPointerDown);
        instance.element.removeEventListener("pointermove", instance.onPointerMove);
        instance.element.removeEventListener("pointerup", instance.onPointerUp);
        instance.element.removeEventListener("pointercancel", instance.onPointerCancel);
        instance.element.removeEventListener("dragstart", instance.onDragStart);
        instance.element.style.touchAction = instance.originalTouchAction;
    }

    clearLongPressTimer(instance);

    instance.dotnetAdapter = null;
    instance.element = null;

    delete _instances[elementId];
}

function pointerDownHandler(instance, e) {
    if (instance.options.disabled || !instance.dotnetAdapter)
        return;

    if (e.isPrimary === false)
        return;

    if (e.pointerType === "mouse" && e.button !== 0)
        return;

    if (isInteractiveElement(e.target, instance.element))
        return;

    instance.active = true;
    instance.pointerId = e.pointerId;
    instance.startClientX = e.clientX;
    instance.startClientY = e.clientY;
    instance.startTime = e.timeStamp;
    instance.lastClientX = e.clientX;
    instance.lastClientY = e.clientY;
    instance.pointerType = e.pointerType || "";
    instance.longPressFired = false;
    instance.lastMoveNotification = 0;

    if (instance.element.setPointerCapture) {
        try {
            instance.element.setPointerCapture(e.pointerId);
        }
        catch {
        }
    }

    if (instance.options.notifyGestureStarted) {
        instance.dotnetAdapter.invokeMethodAsync("OnGestureStarted", createGestureArgs(instance, e, GestureDirection.none, false));
    }

    startLongPressTimer(instance);
}

function dragStartHandler(instance, e) {
    if (instance.options.disabled || !instance.options.preventNativeDrag)
        return;

    if (isInteractiveElement(e.target, instance.element))
        return;

    e.preventDefault();
}

function pointerMoveHandler(instance, e) {
    if (!instance.active || e.pointerId !== instance.pointerId)
        return;

    if (instance.options.disabled) {
        resetGesture(instance, e);
        return;
    }

    instance.lastClientX = e.clientX;
    instance.lastClientY = e.clientY;

    if (getDistance(e.clientX - instance.startClientX, e.clientY - instance.startClientY) > instance.options.longPressMoveTolerance) {
        clearLongPressTimer(instance);
    }

    if (!instance.options.notifyGestureMoved)
        return;

    const throttleInterval = Math.max(instance.options.moveThrottleInterval || 0, 0);

    if (throttleInterval > 0 && e.timeStamp - instance.lastMoveNotification < throttleInterval)
        return;

    instance.lastMoveNotification = e.timeStamp;

    const direction = resolveDirection(e.clientX - instance.startClientX, e.clientY - instance.startClientY);

    instance.dotnetAdapter.invokeMethodAsync("OnGestureMoved", createGestureArgs(instance, e, direction, false));
}

function pointerUpHandler(instance, e, canceled) {
    if (!instance.active || e.pointerId !== instance.pointerId)
        return;

    clearLongPressTimer(instance);

    const direction = resolveDirection(e.clientX - instance.startClientX, e.clientY - instance.startClientY);
    const gestureArgs = createGestureArgs(instance, e, direction, canceled);
    const swipe = !canceled && !instance.options.disabled && !instance.longPressFired && isSwipe(instance, gestureArgs);

    if (instance.options.notifyGestureEnded) {
        instance.dotnetAdapter.invokeMethodAsync("OnGestureEnded", gestureArgs);
    }

    if (swipe && instance.options.notifySwiped) {
        instance.dotnetAdapter.invokeMethodAsync("OnSwiped", gestureArgs);
    }
    else if (!canceled && !instance.options.disabled && !instance.longPressFired && instance.options.notifyTapped && isTap(instance, gestureArgs)) {
        instance.dotnetAdapter.invokeMethodAsync("OnTapped", gestureArgs);
    }

    resetGesture(instance, e);
}

function resetGesture(instance, e) {
    clearLongPressTimer(instance);

    if (instance.element && instance.element.releasePointerCapture && instance.pointerId !== null) {
        try {
            instance.element.releasePointerCapture(instance.pointerId);
        }
        catch {
        }
    }

    instance.active = false;
    instance.pointerId = null;
    instance.pointerType = "";
    instance.longPressFired = false;
    instance.lastMoveNotification = 0;
}

function createGestureArgs(instance, e, direction, canceled) {
    const deltaX = e.clientX - instance.startClientX;
    const deltaY = e.clientY - instance.startClientY;
    const distance = getDistance(deltaX, deltaY);
    const duration = Math.max(e.timeStamp - instance.startTime, 0);
    const axisDistance = isHorizontal(direction) ? Math.abs(deltaX) : Math.abs(deltaY);
    const velocity = duration > 0 ? axisDistance / duration : 0;

    return {
        direction: direction,
        startClientX: instance.startClientX,
        startClientY: instance.startClientY,
        clientX: e.clientX,
        clientY: e.clientY,
        deltaX: deltaX,
        deltaY: deltaY,
        distance: distance,
        duration: duration,
        velocity: velocity,
        pointerType: instance.pointerType || e.pointerType || "",
        canceled: canceled
    };
}

function createGestureArgsFromPoint(instance, clientX, clientY, timeStamp, direction, canceled) {
    return createGestureArgs(instance, {
        clientX: clientX,
        clientY: clientY,
        timeStamp: timeStamp,
        pointerType: instance.pointerType
    }, direction, canceled);
}

function isSwipe(instance, eventArgs) {
    if (eventArgs.direction === GestureDirection.none)
        return false;

    if ((eventArgs.direction & instance.options.direction) === 0)
        return false;

    const axisDistance = isHorizontal(eventArgs.direction) ? Math.abs(eventArgs.deltaX) : Math.abs(eventArgs.deltaY);
    const minimumVelocityDistance = Math.min(instance.options.swipeThreshold, 10);

    return axisDistance >= instance.options.swipeThreshold
        || (axisDistance >= minimumVelocityDistance && eventArgs.velocity >= instance.options.swipeVelocityThreshold);
}

function isTap(instance, eventArgs) {
    return eventArgs.distance <= instance.options.tapMaximumDistance
        && eventArgs.duration <= instance.options.tapMaximumDuration;
}

function startLongPressTimer(instance) {
    clearLongPressTimer(instance);

    if (!instance.options.notifyLongPressed)
        return;

    instance.longPressTimer = setTimeout(function () {
        if (!instance.active || instance.options.disabled || !instance.dotnetAdapter)
            return;

        const deltaX = instance.lastClientX - instance.startClientX;
        const deltaY = instance.lastClientY - instance.startClientY;

        if (getDistance(deltaX, deltaY) > instance.options.longPressMoveTolerance)
            return;

        instance.longPressFired = true;

        const direction = resolveDirection(deltaX, deltaY);
        const eventArgs = createGestureArgsFromPoint(instance, instance.lastClientX, instance.lastClientY, instance.startTime + instance.options.longPressDuration, direction, false);

        instance.dotnetAdapter.invokeMethodAsync("OnLongPressed", eventArgs);
    }, instance.options.longPressDuration);
}

function clearLongPressTimer(instance) {
    if (instance.longPressTimer) {
        clearTimeout(instance.longPressTimer);
        instance.longPressTimer = null;
    }
}

function resolveDirection(deltaX, deltaY) {
    if (deltaX === 0 && deltaY === 0)
        return GestureDirection.none;

    if (Math.abs(deltaX) >= Math.abs(deltaY)) {
        return deltaX < 0 ? GestureDirection.left : GestureDirection.right;
    }

    return deltaY < 0 ? GestureDirection.up : GestureDirection.down;
}

function isHorizontal(direction) {
    return direction === GestureDirection.left || direction === GestureDirection.right;
}

function getDistance(deltaX, deltaY) {
    return Math.sqrt(deltaX * deltaX + deltaY * deltaY);
}

function isInteractiveElement(target, root) {
    if (!target || !root || target === root || !target.closest)
        return false;

    const interactiveElement = target.closest("button, a, input, select, textarea, label, summary, [role='button'], [role='link'], [contenteditable='true'], [data-gesture-ignore]");

    return !!interactiveElement && root.contains(interactiveElement);
}

function normalizeOptions(options) {
    options = options || {};
    const defaultDirection = GestureDirection.left | GestureDirection.right | GestureDirection.up | GestureDirection.down;

    return {
        disabled: options.disabled === true,
        direction: typeof options.direction === "number" ? options.direction : defaultDirection,
        swipeThreshold: numberOrDefault(options.swipeThreshold, 50),
        swipeVelocityThreshold: numberOrDefault(options.swipeVelocityThreshold, 0.3),
        tapMaximumDistance: numberOrDefault(options.tapMaximumDistance, 10),
        tapMaximumDuration: numberOrDefault(options.tapMaximumDuration, 300),
        longPressDuration: numberOrDefault(options.longPressDuration, 500),
        longPressMoveTolerance: numberOrDefault(options.longPressMoveTolerance, 10),
        moveThrottleInterval: numberOrDefault(options.moveThrottleInterval, 0),
        touchAction: options.touchAction || GestureTouchAction.auto,
        preventNativeDrag: options.preventNativeDrag !== false,
        notifyGestureStarted: options.notifyGestureStarted === true,
        notifyGestureMoved: options.notifyGestureMoved === true,
        notifyGestureEnded: options.notifyGestureEnded === true,
        notifySwiped: options.notifySwiped === true,
        notifyTapped: options.notifyTapped === true,
        notifyLongPressed: options.notifyLongPressed === true
    };
}

function numberOrDefault(value, defaultValue) {
    const number = Number(value);

    return Number.isFinite(number) ? number : defaultValue;
}

function applyTouchAction(instance) {
    if (!instance.element)
        return;

    instance.element.style.touchAction = toTouchAction(instance.options.touchAction);
}

function toTouchAction(touchAction) {
    switch (touchAction) {
        case GestureTouchAction.none:
            return "none";
        case GestureTouchAction.panX:
            return "pan-x";
        case GestureTouchAction.panY:
            return "pan-y";
        case GestureTouchAction.manipulation:
            return "manipulation";
        default:
            return "auto";
    }
}