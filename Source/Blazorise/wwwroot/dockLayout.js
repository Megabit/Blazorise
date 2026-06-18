let operation = null;
let animationFrame = null;

export function beginResize(dotNetObjectRef, pane, paneName, position, clientX, clientY, minSize, maxSize) {
    cancel();

    const rect = pane.getBoundingClientRect();
    const horizontal = position === "Left" || position === "Right";

    operation = {
        type: "resize",
        dotNetObjectRef,
        pane,
        paneName,
        position,
        startX: clientX,
        startY: clientY,
        startSize: horizontal ? rect.width : rect.height,
        minSize: parseSize(minSize, pane, horizontal),
        maxSize: parseSize(maxSize, pane, horizontal)
    };

    addDocumentListeners(onResizeMove, onResizeEnd);
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
        zone: null,
        compassX: 0,
        compassY: 0
    };

    addDocumentListeners(onDragMove, onDragEnd);
}

export function cancel() {
    if (!operation) {
        return;
    }

    removeDocumentListeners();

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
    const directionalDelta = operation.position === "Right" || operation.position === "Bottom" ? -pointerDelta : pointerDelta;
    const size = clamp(operation.startSize + directionalDelta, operation.minSize, operation.maxSize);

    scheduleCallback(() => operation.dotNetObjectRef.invokeMethodAsync("NotifyDockPaneResized", operation.paneName, `${Math.round(size)}px`));
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

    const target = findDockTarget(operation.layout, operation.paneName, event.clientX, event.clientY);

    if (operation.targetName !== target.targetName || operation.zone !== target.zone || operation.compassX !== target.compassX || operation.compassY !== target.compassY) {
        operation.targetName = target.targetName;
        operation.zone = target.zone;
        operation.compassX = target.compassX;
        operation.compassY = target.compassY;
        scheduleCallback(() => operation.dotNetObjectRef.invokeMethodAsync("NotifyDockPaneDrag", operation.paneName, target.targetName, target.zone, target.compassX, target.compassY));
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

    currentOperation.dotNetObjectRef.invokeMethodAsync("NotifyDockPaneDropped", currentOperation.paneName, currentOperation.targetName, currentOperation.zone);
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

        return {
            targetName: pane.getAttribute("data-dock-pane-name"),
            zone,
            compassX: Math.round(paneRect.left - layoutRect.left + paneRect.width / 2),
            compassY: Math.round(paneRect.top - layoutRect.top + paneRect.height / 2)
        };
    }

    const zone = findCompassZone(layoutRect, clientX, clientY, false);

    return {
        targetName: null,
        zone,
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
        zone: null,
        compassX: 0,
        compassY: 0
    };
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