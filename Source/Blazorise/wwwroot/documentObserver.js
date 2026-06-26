const documentObserver = createDocumentObserver();

globalThis.Blazorise ??= {};
globalThis.Blazorise.documentObserver = documentObserver;

export function initialize(dotNetReference) {
    documentObserver.initialize(dotNetReference);
}

export function addSubscription(subscription) {
    documentObserver.subscribe(subscription);
}

export function removeSubscription(subscriptionId) {
    documentObserver.unsubscribe(subscriptionId);
}

export function capturePointer(ownerId, pointerId) {
    documentObserver.capturePointer(ownerId, pointerId);
}

export function releasePointer(ownerId, pointerId) {
    documentObserver.releasePointer(ownerId, pointerId);
}

function createDocumentObserver() {
    const subscriptions = new Map();
    const listeners = new Map();
    const pointerCaptures = new Map();
    const pendingAnimationFrameEvents = new Map();
    let dotNetReference = null;
    let nextContextElementId = 0;

    return {
        initialize,
        subscribe,
        unsubscribe,
        capturePointer,
        releasePointer,
    };

    function initialize(reference) {
        dotNetReference = reference;
    }

    function subscribe(subscription) {
        if (!subscription?.id || !Array.isArray(subscription.eventNames)) {
            return null;
        }

        unsubscribe(subscription.id);
        subscriptions.set(subscription.id, normalizeSubscription(subscription));
        updateListeners();

        return subscription.id;
    }

    function unsubscribe(subscriptionId) {
        if (!subscriptionId || !subscriptions.delete(subscriptionId)) {
            return;
        }

        updateListeners();
    }

    function capturePointer(ownerId, pointerId) {
        if (!ownerId || pointerId == null) {
            return;
        }

        pointerCaptures.set(String(pointerId), ownerId);
    }

    function releasePointer(ownerId, pointerId) {
        if (pointerId == null) {
            return;
        }

        const pointerKey = String(pointerId);

        if (!ownerId || pointerCaptures.get(pointerKey) === ownerId) {
            pointerCaptures.delete(pointerKey);
        }
    }

    function normalizeSubscription(subscription) {
        return {
            ...subscription,
            eventNames: subscription.eventNames.filter(Boolean),
            priority: Number.isFinite(subscription.priority) ? subscription.priority : 0,
            capture: subscription.capture !== false,
            preventDefault: subscription.preventDefault === true,
            stopPropagation: subscription.stopPropagation === true,
            throttle: subscription.throttle === true,
            ignorePointerCapture: subscription.ignorePointerCapture === true,
            dotNet: subscription.dotNet === true,
        };
    }

    function updateListeners() {
        const requiredKeys = new Set();

        for (const subscription of subscriptions.values()) {
            for (const eventName of subscription.eventNames) {
                for (const targetName of getListenerTargetNames(eventName)) {
                    requiredKeys.add(createListenerKey(eventName, subscription.capture, targetName));
                }
            }
        }

        for (const [key, listener] of listeners) {
            if (!requiredKeys.has(key)) {
                listener.target.removeEventListener(listener.eventName, listener.handler, listener.capture);
                listeners.delete(key);
            }
        }

        for (const key of requiredKeys) {
            if (listeners.has(key)) {
                continue;
            }

            const [eventName, captureValue, targetName] = key.split(":");
            const capture = captureValue === "1";
            const target = getListenerTarget(targetName);
            const handler = event => dispatch(event, capture);

            target.addEventListener(eventName, handler, { capture, passive: false });
            listeners.set(key, { eventName, capture, target, handler });
        }
    }

    function dispatch(event, capture) {
        const candidates = getCandidates(event, capture);

        for (const subscription of candidates) {
            if (!matchesPointerCapture(subscription, event) || !matchesTarget(subscription, event)) {
                continue;
            }

            if (subscription.preventDefault && typeof event.preventDefault === "function") {
                event.preventDefault();
            }

            if (subscription.stopPropagation && typeof event.stopPropagation === "function") {
                event.stopPropagation();
            }

            if (subscription.throttle) {
                dispatchThrottled(subscription, event);
            }
            else {
                invokeSubscription(subscription, event);
            }
        }
    }

    function getCandidates(event, capture) {
        return Array.from(subscriptions.values())
            .filter(subscription => subscription.capture === capture && subscription.eventNames.includes(event.type))
            .sort((a, b) => b.priority - a.priority);
    }

    function matchesPointerCapture(subscription, event) {
        if (subscription.ignorePointerCapture || event.pointerId == null || event.type === "pointerdown") {
            return true;
        }

        const ownerId = pointerCaptures.get(String(event.pointerId));

        return !ownerId || ownerId === subscription.ownerId;
    }

    function matchesTarget(subscription, event) {
        const target = event.target;

        if (!(target instanceof Element)) {
            return !subscription.selector && !subscription.excludeSelector;
        }

        if (subscription.excludeSelector && target.closest(subscription.excludeSelector)) {
            return false;
        }

        return !subscription.selector || !!target.closest(subscription.selector);
    }

    function dispatchThrottled(subscription, event) {
        const key = subscription.id;
        const current = pendingAnimationFrameEvents.get(key);

        if (current) {
            current.event = event;
            return;
        }

        const pending = { event };
        pendingAnimationFrameEvents.set(key, pending);

        requestAnimationFrame(() => {
            pendingAnimationFrameEvents.delete(key);
            invokeSubscription(subscription, pending.event);
        });
    }

    function invokeSubscription(subscription, event) {
        const args = createDocumentEventArgs(subscription, event);

        if (typeof subscription.handler === "function") {
            subscription.handler(event, args);
        }

        if (subscription.dotNet && dotNetReference) {
            dotNetReference.invokeMethodAsync("NotifyDocumentEvent", subscription.id, args);
        }
    }

    function createDocumentEventArgs(subscription, event) {
        const target = event.target instanceof Element ? event.target : null;
        const matched = subscription.selector && target ? target.closest(subscription.selector) : null;
        const contextElement = matched ?? target;
        const point = getEventPoint(event);

        return {
            type: getDocumentEventType(event.type),
            eventName: event.type,
            pointerId: typeof event.pointerId === "number" ? event.pointerId : 0,
            clientX: point.clientX,
            clientY: point.clientY,
            pageX: point.pageX,
            pageY: point.pageY,
            key: event.key ?? null,
            ctrlKey: event.ctrlKey === true,
            shiftKey: event.shiftKey === true,
            altKey: event.altKey === true,
            metaKey: event.metaKey === true,
            matchedSelector: matched ? subscription.selector : null,
            contextElementSelector: getElementSelector(contextElement),
            targetTagName: target?.tagName?.toLowerCase?.() ?? null,
            targetId: target?.id ?? null,
            targetClassName: typeof target?.className === "string" ? target.className : null,
        };
    }

    function getEventPoint(event) {
        if (typeof event.clientX === "number" || typeof event.clientY === "number") {
            return {
                clientX: event.clientX ?? 0,
                clientY: event.clientY ?? 0,
                pageX: event.pageX ?? ((event.clientX ?? 0) + window.scrollX),
                pageY: event.pageY ?? ((event.clientY ?? 0) + window.scrollY),
            };
        }

        const touch = event.changedTouches?.[0] ?? event.touches?.[0];

        return {
            clientX: touch?.clientX ?? 0,
            clientY: touch?.clientY ?? 0,
            pageX: touch?.pageX ?? ((touch?.clientX ?? 0) + window.scrollX),
            pageY: touch?.pageY ?? ((touch?.clientY ?? 0) + window.scrollY),
        };
    }

    function getListenerTargetNames(eventName) {
        if (eventName === "blur") {
            return ["window"];
        }

        return ["document"];
    }

    function getListenerTarget(targetName) {
        return targetName === "window" ? window : document;
    }

    function createListenerKey(eventName, capture, targetName) {
        return `${eventName}:${capture ? "1" : "0"}:${targetName}`;
    }

    function getElementSelector(element) {
        if (!(element instanceof Element)) {
            return null;
        }

        if (element.id) {
            return `[id="${escapeCssStringValue(element.id)}"]`;
        }

        const attributeName = "data-b-document-observer-context-id";
        let contextId = element.getAttribute(attributeName);

        if (!contextId) {
            contextId = `b-document-observer-context-${++nextContextElementId}`;
            element.setAttribute(attributeName, contextId);
        }

        return `[${attributeName}="${escapeCssStringValue(contextId)}"]`;
    }

    function escapeCssStringValue(value) {
        return String(value).replace(/\\/g, "\\\\").replace(/"/g, "\\\"");
    }
}

function getDocumentEventType(eventName) {
    switch (eventName) {
        case "pointerdown":
            return 1;
        case "pointermove":
            return 2;
        case "pointerup":
            return 3;
        case "pointercancel":
            return 4;
        case "mousedown":
            return 5;
        case "mousemove":
            return 6;
        case "mouseup":
            return 7;
        case "touchmove":
            return 8;
        case "touchend":
            return 9;
        case "touchcancel":
            return 10;
        case "click":
            return 11;
        case "dblclick":
            return 12;
        case "keydown":
            return 13;
        case "keyup":
            return 14;
        case "focusin":
            return 15;
        case "focusout":
            return 16;
        case "dragstart":
            return 17;
        case "dragover":
            return 18;
        case "dragend":
            return 19;
        case "drop":
            return 20;
        case "contextmenu":
            return 21;
        case "blur":
            return 22;
        default:
            return 0;
    }
}