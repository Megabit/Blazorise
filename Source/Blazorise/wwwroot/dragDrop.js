import { getRequiredElement, registerDisconnectCleanup, unregisterDisconnectCleanup } from "./utilities.js?v=2.3.1.0";

const reorderZones = new Map();
let dragSource = null;

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
        attributeFilter: ['class', 'style', 'data-index', 'data-dragging', 'data-reorder-source', 'data-reorder-placeholder', 'data-transaction-active', 'data-transaction-current']
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

    if (dragSource && !dragSource.element.isConnected) {
        clearDragSource();
    }
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

    if (isActive && isCurrent && dragSource) {
        // Size the slot before measuring the neighbors' new layout positions.
        for (const placeholder of element.querySelectorAll(':scope > [data-reorder-placeholder="true"]')) {
            for (const [property, value] of Object.entries(dragSource.placeholderStyles)) {
                if (placeholder.style[property] !== value) {
                    placeholder.style[property] = value;
                }
            }
        }
    }

    const zoneRect = element.getBoundingClientRect();
    const measurements = [];

    // Read all positions before starting any animations. Coordinates are relative
    // to the zone's scrollable content so scrolling is not mistaken for reordering.
    for (const child of element.children) {
        if (!child.classList.contains('b-drop-zone-draggable') || child.dataset.reorderSource === 'true')
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

    if (!isActive && state.isActive && dragSource?.zone === element) {
        clearDragSource();
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

    const source = e.target.closest('.b-drop-zone-draggable');

    if (!source || source.parentElement !== e.currentTarget || source.draggable !== true)
        return;

    clearDragSource();

    const rect = source.getBoundingClientRect();
    const style = getComputedStyle(source);
    dragSource = {
        element: source,
        zone: e.currentTarget,
        placeholderStyles: {
            minHeight: `${rect.height}px`,
            width: `${rect.width}px`,
            marginTop: `${getCollapsedMargin(source, 'Top')}px`,
            marginBottom: `${getCollapsedMargin(source, 'Bottom')}px`,
            marginLeft: style.marginLeft,
            marginRight: style.marginRight
        }
    };

    source.addEventListener('dragend', clearDragSource, { once: true });
    dragSource.cleanupId = registerDisconnectCleanup(source, clearDragSource);

    if (reorderZones.has(e.currentTarget)) {
        // The browser captures the drag image after dragstart. Give it a separate
        // copy before Blazor hides the connected source element for reordering.
        const dragImage = source.cloneNode(true);
        dragImage.removeAttribute('id');
        dragImage.removeAttribute('data-reorder-source');
        dragImage.querySelectorAll('[id]').forEach(child => child.removeAttribute('id'));
        dragImage.classList.add('b-drop-zone-drag-image');
        dragImage.setAttribute('aria-hidden', 'true');
        dragImage.inert = true;
        dragImage.style.width = `${rect.width}px`;
        dragImage.style.height = `${rect.height}px`;
        document.body.appendChild(dragImage);
        e.dataTransfer.setDragImage(dragImage, e.clientX - rect.left, e.clientY - rect.top);
        dragSource.dragImage = dragImage;
        dragSource.dragImageFrameId = requestAnimationFrame(() => dragImage.remove());
    }
}

function getCollapsedMargin(element, side) {
    const style = getComputedStyle(element);
    const margin = parseFloat(style[`margin${side}`]) || 0;
    const child = side === 'Top' ? element.firstElementChild : element.lastElementChild;

    if (child && style.display === 'block' && style.overflow === 'visible'
        && parseFloat(style[`padding${side}`]) === 0 && parseFloat(style[`border${side}Width`]) === 0) {
        const edge = side.toLowerCase();

        if (Math.abs(element.getBoundingClientRect()[edge] - child.getBoundingClientRect()[edge]) < 0.5) {
            const childMargin = getCollapsedMargin(child, side);
            return Math.max(0, margin, childMargin) + Math.min(0, margin, childMargin);
        }
    }

    return margin;
}

function clearDragSource() {
    if (!dragSource)
        return;

    dragSource.element.removeEventListener('dragend', clearDragSource);
    unregisterDisconnectCleanup(dragSource.cleanupId);

    if (dragSource.dragImage) {
        cancelAnimationFrame(dragSource.dragImageFrameId);
        dragSource.dragImage.remove();
    }

    dragSource = null;
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