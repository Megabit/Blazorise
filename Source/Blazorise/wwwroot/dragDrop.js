import { getRequiredElement, registerDisconnectCleanup, unregisterDisconnectCleanup } from "./utilities.js?v=2.3.1.0";

const reorderZones = new Map();

export function initialize(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    element.addEventListener('dragover', dragOverHandler);
    element.addEventListener('dragstart', dragStartHandler);
}

export function destroy(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    destroyReorderZone(element);
    element.removeEventListener('dragover', dragOverHandler);
    element.removeEventListener('dragstart', dragStartHandler);
}

export function updateOptions(element, elementId, dotnetAdapter, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    destroyReorderZone(element);

    if (!options.animated || options.animationDuration <= 0 || !dotnetAdapter)
        return;

    const state = {
        element,
        dotnetAdapter,
        duration: options.animationDuration,
        items: new Map(),
        isActive: false,
        isCurrent: false,
        pointer: null,
        index: null,
        reducedMotion: window.matchMedia('(prefers-reduced-motion: reduce)')
    };

    state.observer = new MutationObserver(records => {
        if (records.some(record => record.target === element || record.target.parentElement === element)) {
            synchronizeItems(state);
        }
    });
    state.resizeObserver = new ResizeObserver(() => synchronizeItems(state));
    state.onMotionChanged = () => {
        for (const item of state.items.values()) {
            cancelAnimation(item);
        }

        synchronizeItems(state);
    };
    state.onDragLeave = event => {
        const rect = element.getBoundingClientRect();

        if (event.clientX < rect.left || event.clientX >= rect.right || event.clientY < rect.top || event.clientY >= rect.bottom) {
            state.pointer = null;
            state.index = null;
        }
    };

    reorderZones.set(element, state);
    synchronizeItems(state);
    state.observer.observe(element, {
        childList: true,
        subtree: true,
        attributes: true,
        attributeFilter: ['class', 'style', 'data-index', 'data-dragging', 'data-transaction-active', 'data-transaction-current']
    });
    state.resizeObserver.observe(element);
    state.reducedMotion.addEventListener('change', state.onMotionChanged);
    element.addEventListener('dragleave', state.onDragLeave);
    state.cleanupId = registerDisconnectCleanup(element, () => destroyReorderZone(element));
}

function destroyReorderZone(element) {
    const state = reorderZones.get(element);

    if (!state)
        return;

    reorderZones.delete(element);
    state.observer.disconnect();
    state.resizeObserver.disconnect();
    state.reducedMotion.removeEventListener('change', state.onMotionChanged);
    element.removeEventListener('dragleave', state.onDragLeave);
    unregisterDisconnectCleanup(state.cleanupId);

    for (const item of state.items.values()) {
        cancelAnimation(item);
    }

    state.items.clear();
}

function cancelAnimation(item) {
    if (item.animation) {
        item.animation.cancel();
        item.animation = null;
    }
}

function synchronizeItems(state) {
    const element = state.element;

    if (!element.isConnected || reorderZones.get(element) !== state)
        return;

    const isActive = element.dataset.transactionActive === 'true';
    const isCurrent = element.dataset.transactionCurrent === 'true';
    const shouldAnimate = (isActive || state.isActive) && !state.reducedMotion.matches;
    const zoneRect = element.getBoundingClientRect();
    const measurements = [];

    // Read all positions before starting any animations. Coordinates are relative
    // to the zone's scrollable content so scrolling is not mistaken for reordering.
    for (const child of element.children) {
        if (!child.classList.contains('b-drop-zone-draggable'))
            continue;

        const previous = state.items.get(child);
        const progress = previous?.animation?.effect.getComputedTiming().progress ?? 1;
        const offsetX = (previous?.deltaX ?? 0) * (1 - progress);
        const offsetY = (previous?.deltaY ?? 0) * (1 - progress);
        const rect = child.getBoundingClientRect();

        measurements.push({
            child,
            previous,
            offsetX,
            offsetY,
            left: rect.left - zoneRect.left + element.scrollLeft - offsetX,
            top: rect.top - zoneRect.top + element.scrollTop - offsetY,
            width: rect.width,
            height: rect.height,
            index: Number(child.dataset.index)
        });
    }

    const items = new Map();

    for (const measurement of measurements) {
        const { child, previous, offsetX, offsetY, left, top, width, height, index } = measurement;
        const item = previous ?? { animation: null };
        const hasMoved = previous && (Math.abs(previous.left - left) > 0.5 || Math.abs(previous.top - top) > 0.5);

        if (hasMoved) {
            const deltaX = previous.left + offsetX - left;
            const deltaY = previous.top + offsetY - top;
            cancelAnimation(item);

            if (shouldAnimate && index >= 0 && child.dataset.dragging !== 'true' && typeof child.animate === 'function') {
                // Individual, additive translation preserves transforms supplied by the user.
                item.deltaX = deltaX;
                item.deltaY = deltaY;
                const animation = child.animate([
                    { translate: `${deltaX}px ${deltaY}px` },
                    { translate: '0px 0px' }
                ], {
                    duration: state.duration,
                    easing: 'ease-out',
                    composite: 'add',
                    fill: 'both'
                });

                item.animation = animation;
                animation.onfinish = () => {
                    if (item.animation === animation) {
                        cancelAnimation(item);
                    }
                };
            }
        }

        if (child.dataset.dragging === 'true') {
            cancelAnimation(item);
        }

        Object.assign(item, { left, top, width, height, index });
        items.set(child, item);

        if (!previous) {
            state.resizeObserver.observe(child);
        }
    }

    for (const [child, item] of state.items) {
        if (!items.has(child)) {
            cancelAnimation(item);
            state.resizeObserver.unobserve(child);
        }
    }

    state.items = items;
    state.isActive = isActive;

    if (!isActive || isCurrent !== state.isCurrent) {
        state.pointer = null;
        state.index = null;
    }

    state.isCurrent = isCurrent;
}

function updateReorderTarget(state, event) {
    if (event.target.closest('.b-drop-zone') !== state.element) {
        state.pointer = null;
        state.index = null;
        return;
    }

    if (!state.isActive)
        return;

    const rect = state.element.getBoundingClientRect();
    const x = event.clientX - rect.left + state.element.scrollLeft;
    const y = event.clientY - rect.top + state.element.scrollTop;

    // Layout changes beneath a stationary pointer must not change the target.
    if (state.pointer?.x === x && state.pointer?.y === y)
        return;

    state.pointer = { x, y };
    const target = Array.from(state.items.values()).find(item =>
        x >= item.left && x < item.left + item.width && y >= item.top && y < item.top + item.height);
    const index = target?.index ?? (state.items.size === 0 ? -1 : null);

    if (index === null || index === state.index)
        return;

    state.index = index;
    state.dotnetAdapter.invokeMethodAsync('OnReorderDragOver', index).catch(error => {
        if (reorderZones.get(state.element) === state) {
            state.index = null;
            console.error(error);
        }
    });
}

export function initializeThrottledDragEvents(element, elementId, dotnetAdapter) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    element.dotnetAdapter = dotnetAdapter;
    element.timeOutForDrag = null;
    element.timeOutForDragOver = null;

    element.addEventListener('drag', throttledDragHandler);
    element.addEventListener('dragover', throttledDragOverHandler);
}

export function destroyThrottledDragEvents(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    if (typeof throttledDragHandler === "function") {
        element.removeEventListener("drag", throttledDragHandler);
    }

    if (typeof throttledDragOverHandler === "function") {
        element.removeEventListener("dragover", throttledDragOverHandler);
    }
}

function dragOverHandler(e) {
    e.preventDefault();

    const state = reorderZones.get(e.currentTarget);

    if (state) {
        updateReorderTarget(state, e);
    }
}

function dragStartHandler(e) {
    e.dataTransfer.setData('', e.target.id);
}

function throttledDragHandler(e) {
    e.preventDefault();

    if (e.target && !e.target.timeOutForDrag) {
        e.target.timeOutForDrag = setTimeout(function () {
            e.target.timeOutForDrag = null;
            if (e.target.dotnetAdapter) {
                e.target.dotnetAdapter.invokeMethodAsync("OnDragHandler", e);
            }
        }.bind(this), 250);
    }
}

function throttledDragOverHandler(e) {
    e.preventDefault();
    if (e.target && !e.target.timeOutForDragOver) {
        e.target.timeOutForDragOver = setTimeout(function () {
            e.target.timeOutForDragOver = null;
            if (e.target.dotnetAdapter) {
                e.target.dotnetAdapter.invokeMethodAsync("OnDragOverHandler", e);
            }
        }.bind(this), 250);
    }
}