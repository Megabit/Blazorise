let operation = null;
let animationFrame = null;

export function beginResize(dotNetObjectRef, pane, paneName, nodeId, position, clientX, clientY, minSize, maxSize) {
    cancel();

    const resizeElement = getResizeElement(pane);
    const split = findResizeSplit(resizeElement, nodeId);
    const rect = resizeElement.getBoundingClientRect();
    const horizontal = position === "Left" || position === "Right";
    const splitRect = split?.getBoundingClientRect();
    const totalSize = horizontal ? splitRect?.width ?? rect.width : splitRect?.height ?? rect.height;
    const firstSize = getResizeFirstSize(rect, splitRect, position, horizontal);
    const firstChild = position === "Left" || position === "Top";
    const minPaneSize = parseSize(minSize, resizeElement, horizontal);
    const maxPaneSize = parseSize(maxSize, resizeElement, horizontal);

    if (!Number.isFinite(totalSize) || totalSize <= 0) {
        return;
    }

    operation = {
        type: "resize",
        dotNetObjectRef,
        pane: resizeElement,
        split,
        paneName,
        nodeId,
        position,
        startX: clientX,
        startY: clientY,
        startFirstSize: firstSize,
        totalSize,
        minFirstSize: getResizeMinFirstSize(totalSize, firstChild, minPaneSize, maxPaneSize),
        maxFirstSize: getResizeMaxFirstSize(totalSize, firstChild, minPaneSize, maxPaneSize)
    };

    addDocumentListeners(onResizeMove, onResizeEnd);
}

function getResizeElement(element) {
    if (element?.classList?.contains("dock-splitter")) {
        return element.parentElement;
    }

    return element;
}

export function beginDrag(dotNetObjectRef, layout, paneName, clientX, clientY, dragGroup) {
    cancel();

    operation = {
        type: "drag",
        dotNetObjectRef,
        layout,
        paneName,
        dragGroup,
        startX: clientX,
        startY: clientY,
        dragging: false,
        targetName: null,
        targetNodeId: null,
        zone: null,
        compassX: 0,
        compassY: 0,
        dragPreview: null,
        dropPreview: null
    };

    addDocumentListeners(onDragMove, onDragEnd);
}

export function cancel() {
    if (!operation) {
        return;
    }

    removeDocumentListeners();
    removeDragVisuals();

    if (animationFrame) {
        cancelAnimationFrame(animationFrame);
        animationFrame = null;
    }

    operation = null;
}

function addDocumentListeners(move, end) {
    operation.move = move;
    operation.end = end;

    document.addEventListener("pointermove", move, true);
    document.addEventListener("pointerup", end, true);
    document.addEventListener("pointercancel", end, true);
}

function removeDocumentListeners() {
    document.removeEventListener("pointermove", operation.move, true);
    document.removeEventListener("pointerup", operation.end, true);
    document.removeEventListener("pointercancel", operation.end, true);
}

function onResizeMove(event) {
    if (!operation || operation.type !== "resize") {
        return;
    }

    event.preventDefault();

    const horizontal = operation.position === "Left" || operation.position === "Right";
    const pointerDelta = horizontal ? event.clientX - operation.startX : event.clientY - operation.startY;
    const size = clamp(operation.startFirstSize + pointerDelta, operation.minFirstSize, operation.maxFirstSize);
    const ratio = clamp(size / operation.totalSize, 0.02, 0.98);

    scheduleCallback(() => operation.dotNetObjectRef.invokeMethodAsync("NotifyDockPaneResized", operation.paneName, operation.nodeId, ratio.toString()));
}

function onResizeEnd(event) {
    if (!operation || operation.type !== "resize") {
        return;
    }

    event.preventDefault();
    const currentOperation = operation;

    cancel();

    currentOperation.dotNetObjectRef.invokeMethodAsync("NotifyDockPaneResizeEnded", currentOperation.paneName);
}

function findResizeSplit(pane, nodeId) {
    let current = pane?.parentElement;

    while (current) {
        if (current.classList?.contains("dock-split") && current.getAttribute("data-dock-node-id") === nodeId) {
            return current;
        }

        current = current.parentElement;
    }

    return pane?.parentElement?.classList?.contains("dock-split") ? pane.parentElement : null;
}

function getResizeFirstSize(paneRect, splitRect, position, horizontal) {
    if (!splitRect) {
        return horizontal ? paneRect.width : paneRect.height;
    }

    switch (position) {
        case "Left":
            return paneRect.right - splitRect.left;
        case "Right":
            return paneRect.left - splitRect.left;
        case "Top":
            return paneRect.bottom - splitRect.top;
        case "Bottom":
            return paneRect.top - splitRect.top;
        default:
            return 0;
    }
}

function getResizeMinFirstSize(totalSize, firstChild, minPaneSize, maxPaneSize) {
    const defaultMinSize = Math.min(32, Math.max(0, totalSize / 2));
    const minSize = firstChild ? minPaneSize : totalSize - maxPaneSize;

    return clamp(Number.isNaN(minSize) ? defaultMinSize : Math.max(defaultMinSize, minSize), 0, totalSize);
}

function getResizeMaxFirstSize(totalSize, firstChild, minPaneSize, maxPaneSize) {
    const defaultMaxSize = Math.max(0, totalSize - Math.min(32, Math.max(0, totalSize / 2)));
    const maxSize = firstChild ? maxPaneSize : totalSize - minPaneSize;

    return clamp(Number.isNaN(maxSize) ? defaultMaxSize : Math.min(defaultMaxSize, maxSize), 0, totalSize);
}

function onDragMove(event) {
    if (!operation || operation.type !== "drag") {
        return;
    }

    const distance = Math.max(Math.abs(event.clientX - operation.startX), Math.abs(event.clientY - operation.startY));

    if (!operation.dragging && distance < 4) {
        return;
    }

    event.preventDefault();
    operation.dragging = true;

    ensureDragPreview(event.clientX, event.clientY);
    updateDragPreview(event.clientX, event.clientY);

    const target = findDockTarget(operation.layout, operation.paneName, event.clientX, event.clientY);
    updateDropPreview(target);

    if (operation.targetName !== target.targetName || operation.targetNodeId !== target.targetNodeId || operation.zone !== target.zone || operation.compassX !== target.compassX || operation.compassY !== target.compassY) {
        operation.targetName = target.targetName;
        operation.targetNodeId = target.targetNodeId;
        operation.zone = target.zone;
        operation.compassX = target.compassX;
        operation.compassY = target.compassY;
        scheduleCallback(() => operation.dotNetObjectRef.invokeMethodAsync("NotifyDockPaneDrag", operation.paneName, target.targetName, target.targetNodeId, target.zone, target.compassX, target.compassY));
    }
}

function onDragEnd(event) {
    if (!operation || operation.type !== "drag") {
        return;
    }

    const currentOperation = operation;

    cancel();

    if (!currentOperation.dragging) {
        return;
    }

    event.preventDefault();

    currentOperation.dotNetObjectRef.invokeMethodAsync("NotifyDockPaneDropped", currentOperation.paneName, currentOperation.targetName, currentOperation.targetNodeId, currentOperation.zone);
}

function findDockTarget(layout, paneName, clientX, clientY) {
    const layoutRect = layout.getBoundingClientRect();

    if (clientX < layoutRect.left || clientX > layoutRect.right || clientY < layoutRect.top || clientY > layoutRect.bottom) {
        return emptyDockTarget();
    }

    const pane = findTargetPane(layout, paneName, clientX, clientY);

    if (pane) {
        const paneRect = pane.getBoundingClientRect();
        const zone = findCompassZone(paneRect, clientX, clientY, true);
        const targetNode = findTargetNode(layout, pane, paneName, zone, clientX, clientY) || pane;
        const targetRect = targetNode.getBoundingClientRect();
        const dropRect = getDropRect(layoutRect, targetRect, zone, targetNode !== pane);

        return {
            targetName: pane.getAttribute("data-dock-pane-name"),
            targetNodeId: targetNode.getAttribute("data-dock-node-id"),
            zone,
            ...dropRect,
            compassX: Math.round(targetRect.left - layoutRect.left + targetRect.width / 2),
            compassY: Math.round(targetRect.top - layoutRect.top + targetRect.height / 2)
        };
    }

    const zone = findCompassZone(layoutRect, clientX, clientY, false);

    return {
        targetName: null,
        targetNodeId: null,
        zone,
        dropLeft: 0,
        dropTop: 0,
        dropWidth: layoutRect.width,
        dropHeight: layoutRect.height,
        compassX: Math.round(layoutRect.width / 2),
        compassY: Math.round(layoutRect.height / 2)
    };
}

function findTargetPane(layout, paneName, clientX, clientY) {
    const element = document.elementFromPoint(clientX, clientY);

    if (!element || !layout.contains(element)) {
        return null;
    }

    const pane = element.closest("[data-dock-pane-name]");

    if (!pane || !layout.contains(pane) || pane.getAttribute("data-dock-pane-name") === paneName) {
        return null;
    }

    return pane;
}

function findTargetNode(layout, pane, paneName, zone, clientX, clientY) {
    if (zone === "Center") {
        return pane;
    }

    const candidates = [];
    let current = pane;

    while (current && current !== layout) {
        if (current.hasAttribute("data-dock-node-id")) {
            candidates.push(current);
        }

        current = current.parentElement;
    }

    for (const candidate of candidates) {
        if (candidate === pane) {
            continue;
        }

        if (isNearNodeEdge(candidate.getBoundingClientRect(), zone, clientX, clientY)) {
            return candidate;
        }
    }

    return pane;
}

function getDropRect(layoutRect, targetRect, zone, deepTarget) {
    const result = {
        dropLeft: Math.round(targetRect.left - layoutRect.left),
        dropTop: Math.round(targetRect.top - layoutRect.top),
        dropWidth: Math.round(targetRect.width),
        dropHeight: Math.round(targetRect.height)
    };

    if (!zone || zone === "Center") {
        return result;
    }

    const ratio = deepTarget ? 0.3 : 0.5;

    switch (zone) {
        case "Left":
            result.dropWidth = Math.round(targetRect.width * ratio);
            break;
        case "Right":
            result.dropWidth = Math.round(targetRect.width * ratio);
            result.dropLeft = Math.round(targetRect.right - layoutRect.left - result.dropWidth);
            break;
        case "Top":
            result.dropHeight = Math.round(targetRect.height * ratio);
            break;
        case "Bottom":
            result.dropHeight = Math.round(targetRect.height * ratio);
            result.dropTop = Math.round(targetRect.bottom - layoutRect.top - result.dropHeight);
            break;
    }

    return result;
}

function isNearNodeEdge(rect, zone, clientX, clientY) {
    const depthSize = Math.max(24, Math.min(56, Math.min(rect.width, rect.height) * 0.18));

    switch (zone) {
        case "Left":
            return clientX - rect.left <= depthSize;
        case "Right":
            return rect.right - clientX <= depthSize;
        case "Top":
            return clientY - rect.top <= depthSize;
        case "Bottom":
            return rect.bottom - clientY <= depthSize;
        default:
            return false;
    }
}

function findCompassZone(rect, clientX, clientY, centerEnabled) {
    if (clientX < rect.left || clientX > rect.right || clientY < rect.top || clientY > rect.bottom) {
        return null;
    }

    const deltaX = clientX - (rect.left + rect.width / 2);
    const deltaY = clientY - (rect.top + rect.height / 2);
    const centerSize = 32;

    if (centerEnabled && Math.abs(deltaX) <= centerSize && Math.abs(deltaY) <= centerSize) {
        return "Center";
    }

    if (Math.abs(deltaX) > Math.abs(deltaY)) {
        return deltaX < 0 ? "Left" : "Right";
    }

    return deltaY < 0 ? "Top" : "Bottom";
}

function emptyDockTarget() {
    return {
        targetName: null,
        targetNodeId: null,
        zone: null,
        dropLeft: 0,
        dropTop: 0,
        dropWidth: 0,
        dropHeight: 0,
        compassX: 0,
        compassY: 0
    };
}

function ensureDragPreview(clientX, clientY) {
    if (operation.dragPreview) {
        return;
    }

    const preview = document.createElement("div");

    preview.className = "dock-drag-preview";
    preview.textContent = getPaneCaption(operation.layout, operation.paneName);
    operation.layout.appendChild(preview);
    operation.dragPreview = preview;
    updateDragPreview(clientX, clientY);
}

function updateDragPreview(clientX, clientY) {
    if (!operation.dragPreview) {
        return;
    }

    const layoutRect = operation.layout.getBoundingClientRect();
    const left = Math.round(clientX - layoutRect.left + 12);
    const top = Math.round(clientY - layoutRect.top + 12);

    operation.dragPreview.style.left = `${left}px`;
    operation.dragPreview.style.top = `${top}px`;
}

function updateDropPreview(target) {
    if (!target.zone || !target.dropWidth || !target.dropHeight) {
        removeDropPreview();
        return;
    }

    if (!operation.dropPreview) {
        const preview = document.createElement("div");

        preview.className = "dock-drop-preview";
        operation.layout.appendChild(preview);
        operation.dropPreview = preview;
    }

    operation.dropPreview.style.left = `${target.dropLeft}px`;
    operation.dropPreview.style.top = `${target.dropTop}px`;
    operation.dropPreview.style.width = `${target.dropWidth}px`;
    operation.dropPreview.style.height = `${target.dropHeight}px`;
}

function removeDragVisuals() {
    removeDropPreview();

    if (operation.dragPreview) {
        operation.dragPreview.remove();
        operation.dragPreview = null;
    }
}

function removeDropPreview() {
    if (operation.dropPreview) {
        operation.dropPreview.remove();
        operation.dropPreview = null;
    }
}

function getPaneCaption(layout, paneName) {
    const tab = layout.querySelector(`[data-dock-tab-name="${cssEscape(paneName)}"] .dock-pane-tab-label`);

    if (tab) {
        return tab.textContent?.trim() || paneName;
    }

    const pane = layout.querySelector(`[data-dock-pane-name="${cssEscape(paneName)}"]`);

    return pane?.getAttribute("data-dock-pane-caption") || paneName;
}

function cssEscape(value) {
    if (globalThis.CSS?.escape) {
        return CSS.escape(value);
    }

    return String(value).replace(/["\\]/g, "\\$&");
}

function scheduleCallback(callback) {
    if (animationFrame) {
        cancelAnimationFrame(animationFrame);
    }

    animationFrame = requestAnimationFrame(() => {
        animationFrame = null;
        callback();
    });
}

function clamp(value, min, max) {
    let result = value;

    if (!Number.isNaN(min)) {
        result = Math.max(result, min);
    }

    if (!Number.isNaN(max)) {
        result = Math.min(result, max);
    }

    return result;
}

function parseSize(value, context, horizontal) {
    if (!value || value === "none") {
        return Number.NaN;
    }

    if (value.endsWith("px")) {
        const parsed = Number.parseFloat(value);

        return Number.isNaN(parsed) ? Number.NaN : parsed;
    }

    const probe = document.createElement("div");

    probe.style.position = "absolute";
    probe.style.visibility = "hidden";
    probe.style.pointerEvents = "none";

    if (horizontal) {
        probe.style.width = value;
        probe.style.height = "0";
    } else {
        probe.style.width = "0";
        probe.style.height = value;
    }

    context.parentElement.appendChild(probe);

    const rect = probe.getBoundingClientRect();
    const result = horizontal ? rect.width : rect.height;

    probe.remove();

    return result || Number.NaN;
}