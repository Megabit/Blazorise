const zoomWheelHandlers = new WeakMap();
const dataDragHandlers = new WeakMap();
const animationObservers = new WeakMap();
const animationStates = new WeakMap();
const streamingAnimationStates = new WeakMap();

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

export function initializeDataDrag(element, dotNetReference) {
    if (!element) {
        return;
    }

    const existing = dataDragHandlers.get(element);

    if (existing) {
        existing.dotNetReference = dotNetReference;
        return;
    }

    const state = {
        dotNetReference,
        candidate: null,
        pendingKeys: [],
        keyInFlight: false,
        suppressClick: false,
        documentObserverScope: null,
        finishPromise: null,
        destroyed: false
    };

    const onPointerDown = event => {
        const target = findDraggablePoint(element, event.target);

        if (!target || event.button !== 0 || event.isPrimary === false) {
            return;
        }

        event.preventDefault();

        state.candidate = {
            pointerId: event.pointerId,
            startClientX: event.clientX,
            startClientY: event.clientY,
            startPoint: toSvgPoint(element, event.clientX, event.clientY),
            seriesIndex: parseIndex(target.dataset.svgChartSeriesIndex),
            pointIndex: parseIndex(target.dataset.svgChartPointIndex),
            target,
            active: false,
            starting: false,
            startPromise: null,
            pendingMove: null,
            moveInFlight: false,
            animationFrame: 0,
            readyPromise: state.finishPromise,
            canceled: false
        };

        const candidate = state.candidate;

        if (typeof element.setPointerCapture === "function") {
            try {
                element.setPointerCapture(event.pointerId);
            } catch {
            }
        }

        addDataDragDocumentListeners(element, state, candidate, onPointerMove, onPointerUp, onPointerCancel);

        event.stopPropagation();
    };

    const onPointerMove = event => {
        const candidate = state.candidate;

        if (!candidate || candidate.pointerId !== event.pointerId) {
            return;
        }

        event.preventDefault();
        event.stopPropagation();

        const distance = Math.hypot(event.clientX - candidate.startClientX, event.clientY - candidate.startClientY);

        if (!candidate.active && !candidate.starting && distance >= 3) {
            candidate.starting = true;
            candidate.startPromise = (candidate.readyPromise ?? Promise.resolve())
                .then(() => state.destroyed || candidate.canceled
                    ? false
                    : state.dotNetReference.invokeMethodAsync("Start", candidate.seriesIndex, candidate.pointIndex, candidate.startPoint.x, candidate.startPoint.y))
                .then(started => {
                    candidate.starting = false;
                    candidate.active = started === true;

                    if (candidate.active && state.candidate === candidate) {
                        candidate.target.style.cursor = "grabbing";

                        if (candidate.pendingMove) {
                            scheduleDataDragMove(state, candidate);
                        }
                    }

                    return candidate.active;
                })
                .catch(() => {
                    candidate.starting = false;
                    candidate.active = false;
                    return false;
                });
        }

        if (candidate.active || candidate.starting) {
            candidate.pendingMove = toSvgPoint(element, event.clientX, event.clientY);
            scheduleDataDragMove(state, candidate);
        }
    };

    const onPointerUp = event => {
        if (!state.candidate || state.candidate.pointerId !== event.pointerId) {
            return;
        }

        if (state.candidate.active || state.candidate.starting) {
            state.suppressClick = true;
            setTimeout(() => state.suppressClick = false, 0);
        }

        finishDataDrag(element, state, false);
    };

    const onPointerCancel = event => {
        if (!state.candidate || state.candidate.pointerId !== event.pointerId) {
            return;
        }

        finishDataDrag(element, state, true);
    };

    const onMouseDown = event => {
        if (findDraggablePoint(element, event.target)) {
            event.stopPropagation();
        }
    };

    const onClick = event => {
        if (!state.suppressClick) {
            return;
        }

        state.suppressClick = false;
        event.preventDefault();
        event.stopImmediatePropagation();
    };

    const onKeyDown = event => {
        if (event.key === "Escape" && (state.candidate?.active || state.candidate?.starting)) {
            event.preventDefault();
            finishDataDrag(element, state, true);
            return;
        }

        if (!["ArrowLeft", "ArrowRight", "ArrowUp", "ArrowDown"].includes(event.key)) {
            return;
        }

        const target = findDraggablePoint(element, event.target);

        if (!target) {
            return;
        }

        event.preventDefault();
        event.stopPropagation();
        state.pendingKeys.push({
            seriesIndex: parseIndex(target.dataset.svgChartSeriesIndex),
            pointIndex: parseIndex(target.dataset.svgChartPointIndex),
            key: event.key,
            shiftKey: event.shiftKey
        });
        dispatchDataDragKey(state);
    };

    state.onPointerDown = onPointerDown;
    state.onMouseDown = onMouseDown;
    state.onClick = onClick;
    state.onKeyDown = onKeyDown;

    element.addEventListener("pointerdown", onPointerDown);
    element.addEventListener("mousedown", onMouseDown, true);
    element.addEventListener("click", onClick, true);
    window.addEventListener("keydown", onKeyDown);
    dataDragHandlers.set(element, state);
}

export function destroyDataDrag(element) {
    const state = dataDragHandlers.get(element);

    if (!element || !state) {
        return;
    }

    state.destroyed = true;
    element.removeEventListener("pointerdown", state.onPointerDown);
    element.removeEventListener("mousedown", state.onMouseDown, true);
    element.removeEventListener("click", state.onClick, true);
    window.removeEventListener("keydown", state.onKeyDown);

    if (state.candidate?.animationFrame) {
        cancelAnimationFrame(state.candidate.animationFrame);
    }

    removeDataDragDocumentListeners(state);

    dataDragHandlers.delete(element);
}

function addDataDragDocumentListeners(element, state, candidate, onPointerMove, onPointerUp, onPointerCancel) {
    removeDataDragDocumentListeners(state);

    const documentObserverScope = globalThis.Blazorise?.documentObserver?.createScope("svg-chart-data-drag") ?? null;

    if (!documentObserverScope) {
        return;
    }

    state.documentObserverScope = documentObserverScope;
    documentObserverScope.subscribe({
        eventNames: ["pointermove"],
        capture: true,
        handler: onPointerMove
    });
    documentObserverScope.subscribe({
        eventNames: ["pointerup", "pointercancel", "blur"],
        capture: true,
        handler: event => {
            if (event.type === "blur") {
                finishDataDrag(element, state, true);
            } else if (event.type === "pointercancel") {
                onPointerCancel(event);
            } else {
                onPointerUp(event);
            }
        }
    });
    documentObserverScope.capturePointer(candidate.pointerId);
}

function removeDataDragDocumentListeners(state) {
    state.documentObserverScope?.dispose();
    state.documentObserverScope = null;
}

function findDraggablePoint(element, target) {
    if (!(target instanceof Element)) {
        return null;
    }

    const point = target.closest("[data-svg-chart-draggable='true']");

    return point && element.contains(point) ? point : null;
}

function parseIndex(value) {
    const index = Number.parseInt(value, 10);

    return Number.isInteger(index) ? index : -1;
}

function toSvgPoint(element, clientX, clientY) {
    const svg = element.querySelector("svg");

    if (!svg) {
        return { x: 0, y: 0 };
    }

    const matrix = svg.getScreenCTM();

    if (matrix) {
        const point = svg.createSVGPoint();
        point.x = clientX;
        point.y = clientY;

        const transformed = point.matrixTransform(matrix.inverse());

        return { x: transformed.x, y: transformed.y };
    }

    const bounds = svg.getBoundingClientRect();
    const viewBox = svg.viewBox.baseVal;

    return {
        x: viewBox.x + (clientX - bounds.left) / Math.max(bounds.width, 1) * viewBox.width,
        y: viewBox.y + (clientY - bounds.top) / Math.max(bounds.height, 1) * viewBox.height
    };
}

function scheduleDataDragMove(state, candidate) {
    if (candidate.animationFrame || state.destroyed || state.candidate !== candidate) {
        return;
    }

    candidate.animationFrame = requestAnimationFrame(() => {
        candidate.animationFrame = 0;
        dispatchDataDragMove(state, candidate);
    });
}

function dispatchDataDragMove(state, candidate) {
    if (!candidate.active || candidate.moveInFlight || !candidate.pendingMove || state.destroyed || state.candidate !== candidate) {
        return;
    }

    const point = candidate.pendingMove;
    candidate.pendingMove = null;
    candidate.moveInFlight = true;

    state.dotNetReference.invokeMethodAsync("Move", point.x, point.y)
        .catch(() => {})
        .finally(() => {
            candidate.moveInFlight = false;

            if (candidate.pendingMove) {
                scheduleDataDragMove(state, candidate);
            }
        });
}

function dispatchDataDragKey(state) {
    if (state.keyInFlight || state.pendingKeys.length === 0 || state.destroyed) {
        return;
    }

    const key = state.pendingKeys.shift();
    state.keyInFlight = true;

    state.dotNetReference.invokeMethodAsync("KeyDown", key.seriesIndex, key.pointIndex, key.key, key.shiftKey)
        .catch(() => {})
        .finally(() => {
            state.keyInFlight = false;

            if (state.pendingKeys.length > 0) {
                dispatchDataDragKey(state);
            }
        });
}

function finishDataDrag(element, state, canceled) {
    const candidate = state.candidate;

    if (!candidate) {
        return;
    }

    candidate.canceled = canceled;
    state.candidate = null;
    removeDataDragDocumentListeners(state);
    releaseDataDragPointerCapture(element, candidate);
    candidate.target.style.cursor = "grab";

    const finishPromise = completeDataDrag(state, candidate, canceled);
    state.finishPromise = finishPromise;

    finishPromise.finally(() => {
        if (state.finishPromise === finishPromise) {
            state.finishPromise = null;
        }
    });

    return finishPromise;
}

async function completeDataDrag(state, candidate, canceled) {
    try {
        if (candidate.startPromise) {
            await candidate.startPromise;
        }

        if (candidate.active) {
            if (candidate.animationFrame) {
                cancelAnimationFrame(candidate.animationFrame);
                candidate.animationFrame = 0;
            }

            while (candidate.moveInFlight) {
                await new Promise(resolve => setTimeout(resolve, 0));
            }

            if (!canceled && candidate.pendingMove) {
                const point = candidate.pendingMove;
                candidate.pendingMove = null;
                await state.dotNetReference.invokeMethodAsync("Move", point.x, point.y);
            }

            await state.dotNetReference.invokeMethodAsync("End", canceled);
        }
    } catch {
    } finally {
        candidate.active = false;
        candidate.starting = false;
        candidate.startPromise = null;
        candidate.pendingMove = null;
    }
}

function releaseDataDragPointerCapture(element, candidate) {
    if (typeof element.hasPointerCapture !== "function" || !element.hasPointerCapture(candidate.pointerId)) {
        return;
    }

    try {
        element.releasePointerCapture(candidate.pointerId);
    } catch {
    }
}

export function runAnimations(element) {
    if (!element) {
        return;
    }

    initializeAnimations(element);

    const items = element.querySelectorAll(animationSelector());

    for (const item of items) {
        runElementAnimation(item);
    }
}

export function destroyAnimations(element) {
    const observer = animationObservers.get(element);

    if (!observer) {
        return;
    }

    observer.disconnect();
    animationObservers.delete(element);
}

export function runStreamingAnimations(element) {
    if (!element) {
        return;
    }

    const items = element.querySelectorAll("[data-svg-chart-streaming-animation='true']");

    for (const item of items) {
        runStreamingAnimation(item);
    }
}

function initializeAnimations(element) {
    if (animationObservers.has(element)) {
        return;
    }

    const observer = new MutationObserver(mutations => {
        const items = new Set();

        for (const mutation of mutations) {
            if (mutation.type === "attributes" && isAnimationAttribute(mutation.attributeName)) {
                items.add(mutation.target);
                continue;
            }

            if (mutation.type === "childList") {
                for (const node of mutation.addedNodes) {
                    collectAnimatedElements(node, items);
                }
            }
        }

        for (const item of items) {
            runElementAnimation(item);
        }
    });

    observer.observe(element, {
        attributes: true,
        childList: true,
        subtree: true
    });

    animationObservers.set(element, observer);
}

function collectAnimatedElements(node, items) {
    if (!(node instanceof Element)) {
        return;
    }

    if (hasAnimationAttributes(node)) {
        items.add(node);
    }

    for (const item of node.querySelectorAll(animationSelector())) {
        items.add(item);
    }
}

function runStreamingAnimation(element) {
    const version = element.dataset.svgChartStreamingVersion || "0";
    const offset = parseFloat(element.dataset.svgChartStreamingOffset);
    const duration = parseDuration(element.dataset.svgChartStreamingDuration);

    if (!Number.isFinite(offset) || duration <= 0) {
        return;
    }

    if (streamingAnimationStates.get(element) === version) {
        return;
    }

    streamingAnimationStates.set(element, version);
    element.style.transition = "none";
    element.style.transform = "translateX(0px)";
    element.getBoundingClientRect();

    if (streamingAnimationStates.get(element) !== version) {
        return;
    }

    element.style.transition = `transform ${duration}ms linear`;
    element.style.transform = `translateX(${formatNumber(offset)}px)`;
}

function runElementAnimation(element) {
    const attributes = animationAttributes();
    let started = false;

    for (const attribute of attributes) {
        const name = toDatasetName(attribute);

        if (element.dataset[`svgChartAnimation${name}`] !== "true") {
            continue;
        }

        const duration = parseDuration(element.dataset[`svgChartAnimation${name}Duration`]);
        const delay = parseDuration(element.dataset[`svgChartAnimation${name}Delay`]);
        const version = element.dataset[`svgChartAnimation${name}Version`] || "0";
        const keySplines = parseKeySplines(element.dataset[`svgChartAnimation${name}KeySplines`]);
        const from = parseFloat(element.dataset[`svgChartAnimation${name}From`]);
        const to = parseFloat(element.dataset[`svgChartAnimation${name}To`]);

        if (duration <= 0 || !Number.isFinite(from) || !Number.isFinite(to) || from === to) {
            continue;
        }

        const key = `${version}:${attribute}:${from}:${to}`;
        const current = animationStates.get(element);

        if (current?.[attribute] === key) {
            continue;
        }

        animationStates.set(element, { ...current, [attribute]: key });
        animateAttribute(element, attribute, from, to, duration, delay, keySplines);
        started = true;
    }

    if (!started && element.dataset.svgChartAnimationInitial === "true") {
        revealInitialAnimatedElement(element);
    }
}

function animateAttribute(element, attribute, from, to, duration, delay, keySplines) {
    let start = null;

    setAnimatedAttribute(element, attribute, from);
    revealInitialAnimatedElement(element);

    const step = timestamp => {
        if (animationStates.get(element)?.[attribute] !== `${attributeVersionKey(element, attribute)}:${attribute}:${from}:${to}`) {
            return;
        }

        start ??= timestamp + delay;

        if (timestamp < start) {
            requestAnimationFrame(step);
            return;
        }

        const progress = Math.min((timestamp - start) / duration, 1);
        const eased = keySplines ? cubicBezier(progress, keySplines[0], keySplines[1], keySplines[2], keySplines[3]) : progress;
        const value = from + (to - from) * eased;

        setAnimatedAttribute(element, attribute, value);

        if (progress < 1) {
            requestAnimationFrame(step);
        } else {
            setAnimatedAttribute(element, attribute, to);
        }
    };

    requestAnimationFrame(step);
}

function animationAttributes() {
    return ["x", "y", "width", "height", "cx", "cy", "r", "opacity"];
}

function animationSelector() {
    return animationAttributes()
        .map(attribute => `[data-svg-chart-animation-${attribute}='true']`)
        .join(",");
}

function hasAnimationAttributes(element) {
    return animationAttributes().some(attribute => element.dataset[`svgChartAnimation${toDatasetName(attribute)}`] === "true");
}

function isAnimationAttribute(attributeName) {
    return typeof attributeName === "string" && attributeName.startsWith("data-svg-chart-animation-");
}

function attributeVersionKey(element, attribute) {
    return element.dataset[`svgChartAnimation${toDatasetName(attribute)}Version`] || "0";
}

function setAnimatedAttribute(element, attribute, value) {
    element.setAttribute(attribute, formatNumber(value));
}

function revealInitialAnimatedElement(element) {
    if (element.dataset.svgChartAnimationInitial !== "true") {
        return;
    }

    element.style.visibility = "visible";
    element.removeAttribute("data-svg-chart-animation-initial");
}

function parseDuration(value) {
    if (!value) {
        return 0;
    }

    if (value.endsWith("ms")) {
        return parseFloat(value);
    }

    if (value.endsWith("s")) {
        return parseFloat(value) * 1000;
    }

    return parseFloat(value) || 0;
}

function parseKeySplines(value) {
    if (!value) {
        return null;
    }

    const parts = value.split(/\s+/).map(Number);

    return parts.length === 4 && parts.every(Number.isFinite) ? parts : null;
}

function toDatasetName(attribute) {
    return attribute.charAt(0).toUpperCase() + attribute.slice(1);
}

function cubicBezier(progress, x1, y1, x2, y2) {
    let lower = 0;
    let upper = 1;
    let t = progress;

    for (let i = 0; i < 8; i++) {
        const x = bezier(t, x1, x2);

        if (Math.abs(x - progress) < 0.001) {
            break;
        }

        if (x > progress) {
            upper = t;
        } else {
            lower = t;
        }

        t = (lower + upper) / 2;
    }

    return bezier(t, y1, y2);
}

function bezier(t, a, b) {
    const inv = 1 - t;

    return 3 * inv * inv * t * a + 3 * inv * t * t * b + t * t * t;
}

function formatNumber(value) {
    return Number.isFinite(value) ? value.toFixed(3).replace(/\.?0+$/, "") : value;
}