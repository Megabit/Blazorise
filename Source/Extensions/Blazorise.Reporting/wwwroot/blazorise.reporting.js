let sectionResize;
const designerKeyboardShortcuts = new WeakMap();
let activeDesignerKeyboardShortcut;
const treeDragImageSuppressors = new WeakMap();
const textTokenEditors = new WeakMap();
const designerInteractionOverlays = new WeakMap();
const designerSectionResizePreviews = new WeakMap();
let nextSubscriptionId = 0;

const designerControlShortcuts = {
    x: "Cut",
    c: "Copy",
    v: "Paste",
    z: "Undo",
    y: "Redo",
};

const designerControlShiftShortcuts = {
    z: "Redo",
};

const designerPlainShortcuts = {
    Delete: "Delete",
    F2: "EditText",
    ArrowLeft: "MoveLeft",
    ArrowUp: "MoveUp",
    ArrowRight: "MoveRight",
    ArrowDown: "MoveDown",
};

export function startSectionResize(dotNetReference, startClientY, pointerId) {
    stopSectionResize();

    const observer = getDocumentObserver();

    if (!observer) {
        return;
    }

    sectionResize = {
        dotNetReference,
        ownerId: createDocumentObserverOwnerId("section-resize"),
        pointerId,
        ended: false,
        lastClientY: startClientY ?? 0,
        move: event => {
            const resize = sectionResize;

            if (!resize || resize.ended) {
                return;
            }

            event.preventDefault();
            resize.lastClientY = getClientY(event, resize.lastClientY);
            resize.dotNetReference.invokeMethodAsync("OnDocumentSectionResizeMove", resize.lastClientY);
        },
        end: event => {
            const resize = sectionResize;

            if (!resize || resize.ended) {
                return;
            }

            event.preventDefault();
            completeSectionResize(resize, getClientY(event, resize.lastClientY));
        },
        cancel: event => cancelSectionResize(event),
    };

    addSectionResizeListeners(sectionResize);
    observer.capturePointer(sectionResize.ownerId, sectionResize.pointerId);
}

export function stopSectionResize() {
    const resize = sectionResize;

    if (!resize) {
        return;
    }

    clearSectionResize(resize);
}

export function startDesignerKeyboardShortcuts(element, dotNetReference) {
    if (!element || typeof element.contains !== "function" || !dotNetReference) {
        return;
    }

    stopDesignerKeyboardShortcuts(element);

    const observer = getDocumentObserver();

    if (!observer) {
        return;
    }

    const shortcuts = {
        dotNetReference,
        ownerId: createDocumentObserverOwnerId("keyboard"),
        pointerDown: event => {
            if (element.contains(event.target)) {
                activeDesignerKeyboardShortcut = shortcuts;
            }
            else if (activeDesignerKeyboardShortcut === shortcuts) {
                activeDesignerKeyboardShortcut = null;
            }
        },
        focusIn: event => {
            if (element.contains(event.target)) {
                activeDesignerKeyboardShortcut = shortcuts;
            }
        },
        keyDown: event => {
            if (activeDesignerKeyboardShortcut !== shortcuts || shouldIgnoreDesignerShortcut(event)) {
                return;
            }

            const shortcut = resolveDesignerShortcut(event);

            if (!shortcut) {
                return;
            }

            event.preventDefault();
            event.stopPropagation();
            shortcuts.dotNetReference.invokeMethodAsync("OnDesignerShortcut", shortcut);
        },
    };

    shortcuts.subscriptions = [
        createDocumentObserverSubscriptionId("keyboard-pointer"),
        createDocumentObserverSubscriptionId("keyboard-focus"),
        createDocumentObserverSubscriptionId("keyboard-key"),
    ];

    observer.subscribe({
        id: shortcuts.subscriptions[0],
        ownerId: shortcuts.ownerId,
        eventNames: ["pointerdown", "mousedown"],
        capture: true,
        ignorePointerCapture: true,
        handler: shortcuts.pointerDown,
    });

    observer.subscribe({
        id: shortcuts.subscriptions[1],
        ownerId: shortcuts.ownerId,
        eventNames: ["focusin"],
        capture: true,
        handler: shortcuts.focusIn,
    });

    observer.subscribe({
        id: shortcuts.subscriptions[2],
        ownerId: shortcuts.ownerId,
        eventNames: ["keydown"],
        capture: true,
        handler: shortcuts.keyDown,
    });

    designerKeyboardShortcuts.set(element, shortcuts);
}

export function stopDesignerKeyboardShortcuts(element) {
    if (!element) {
        return;
    }

    const shortcuts = designerKeyboardShortcuts.get(element);

    if (!shortcuts) {
        return;
    }

    const observer = getDocumentObserver();

    if (observer && shortcuts.subscriptions) {
        for (const subscriptionId of shortcuts.subscriptions) {
            observer.unsubscribe(subscriptionId);
        }
    }

    if (activeDesignerKeyboardShortcut === shortcuts) {
        activeDesignerKeyboardShortcut = null;
    }

    designerKeyboardShortcuts.delete(element);
}

export function getElementOffset(element, clientX, clientY) {
    if (!element || typeof element.getBoundingClientRect !== "function") {
        return [0, 0];
    }

    const rectangle = element.getBoundingClientRect();

    return [
        Math.max(0, (clientX ?? 0) - rectangle.left),
        Math.max(0, (clientY ?? 0) - rectangle.top),
    ];
}

export function suppressTreeNativeDragImage(element) {
    if (!element || typeof element.addEventListener !== "function") {
        return;
    }

    clearTreeNativeDragImage(element);

    const suppressor = {
        dragImage: createTransparentDragImage(),
        dragStart: event => {
            if (!event.target?.closest?.(".b-report-treeview-row.draggable") || !event.dataTransfer?.setDragImage) {
                return;
            }

            event.dataTransfer.setDragImage(suppressor.dragImage, 0, 0);
        },
    };

    element.addEventListener("dragstart", suppressor.dragStart, true);
    treeDragImageSuppressors.set(element, suppressor);
}

export function clearTreeNativeDragImage(element) {
    if (!element || typeof element.removeEventListener !== "function") {
        return;
    }

    const suppressor = treeDragImageSuppressors.get(element);

    if (!suppressor) {
        return;
    }

    element.removeEventListener("dragstart", suppressor.dragStart, true);
    treeDragImageSuppressors.delete(element);
}

export function scrollTreeNodeIntoView(element, nodeKey) {
    if (!element || !nodeKey || typeof element.querySelectorAll !== "function") {
        return;
    }

    const rows = element.querySelectorAll("[data-report-tree-node-key]");

    for (const row of rows) {
        if (row.getAttribute("data-report-tree-node-key") === nodeKey) {
            row.scrollIntoView({ block: "nearest", inline: "nearest" });
            return;
        }
    }
}

export function protectTextExpressionTokens(element) {
    if (!element || typeof element.addEventListener !== "function") {
        return;
    }

    clearTextExpressionTokenProtection(element);

    const editor = {
        keyDown: event => handleTextExpressionTokenKeyDown(element, event),
        replace: () => prepareTextExpressionTokenReplacement(element),
    };

    element.addEventListener("keydown", editor.keyDown, true);
    element.addEventListener("paste", editor.replace, true);
    element.addEventListener("cut", editor.replace, true);
    textTokenEditors.set(element, editor);
}

export function clearTextExpressionTokenProtection(element) {
    if (!element || typeof element.removeEventListener !== "function") {
        return;
    }

    const editor = textTokenEditors.get(element);

    if (!editor) {
        return;
    }

    element.removeEventListener("keydown", editor.keyDown, true);
    element.removeEventListener("paste", editor.replace, true);
    element.removeEventListener("cut", editor.replace, true);
    textTokenEditors.delete(element);
}

export function downloadFile(fileName, contentType, content) {
    const blob = new Blob([content], { type: contentType || "application/octet-stream" });
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement("a");

    anchor.href = url;
    anchor.download = fileName || "download";
    anchor.style.display = "none";
    document.body.appendChild(anchor);
    anchor.click();
    anchor.remove();
    setTimeout(() => URL.revokeObjectURL(url), 0);
}

export function updateDesignerSelectionOverlay(pageElement, x, y, width, height) {
    const overlays = getDesignerInteractionOverlays(pageElement);

    if (!overlays) {
        return;
    }

    positionOverlay(overlays.selection, x, y, width, height);
    overlays.selection.hidden = false;
    updateDesignerRulerMarker(pageElement, x, y, width, height, true);
}

export function updateDesignerDragOverlay(pageElement, elementType, text, x, y, width, height) {
    const overlays = getDesignerInteractionOverlays(pageElement);

    if (!overlays) {
        return;
    }

    overlays.drag.className = `b-report-drag-preview b-report-element-${(elementType || "text").toLowerCase()}`;
    overlays.drag.textContent = text || "";
    positionOverlay(overlays.drag, x, y, width, height);
    overlays.drag.hidden = false;
    updateDesignerRulerMarker(pageElement, x, y, width, height, true);
}

export function clearDesignerInteractionOverlays(pageElement) {
    const overlays = designerInteractionOverlays.get(pageElement);

    if (!overlays) {
        return;
    }

    overlays.selection.hidden = true;
    overlays.drag.hidden = true;
    clearDesignerRulerMarker(pageElement);
}

function updateDesignerRulerMarker(pageElement, x, y, width, height, active) {
    const root = pageElement?.closest?.(".b-report-designer-rulers");

    if (!root) {
        return;
    }

    const horizontal = root.querySelector(".b-report-designer-ruler-horizontal");
    const vertical = root.querySelector(".b-report-designer-ruler-vertical");

    updateHorizontalRulerMarker(horizontal, x, width, active);
    updateVerticalRulerMarker(vertical, y, height, active);
}

function updateHorizontalRulerMarker(ruler, x, width, active) {
    if (!ruler) {
        return;
    }

    const markers = ruler.querySelectorAll(".b-report-designer-ruler-marker");
    const range = ruler.querySelector(".b-report-designer-ruler-marker-range");

    if (range) {
        range.style.left = `calc(var(--b-report-designer-ruler-surface-offset) + ${Math.max(0, x || 0)}px)`;
        range.style.width = `${Math.max(0, width || 0)}px`;
        range.hidden = false;
        range.classList.toggle("b-report-designer-ruler-marker-range-active", !!active);
    }

    if (markers[0]) {
        markers[0].style.left = `calc(var(--b-report-designer-ruler-surface-offset) + ${Math.max(0, x || 0)}px)`;
        markers[0].hidden = false;
        markers[0].classList.toggle("b-report-designer-ruler-marker-active", !!active);
    }

    if (markers[1]) {
        markers[1].style.left = `calc(var(--b-report-designer-ruler-surface-offset) + ${Math.max(0, (x || 0) + (width || 0))}px)`;
        markers[1].hidden = false;
        markers[1].classList.toggle("b-report-designer-ruler-marker-active", !!active);
    }
}

function updateVerticalRulerMarker(ruler, y, height, active) {
    if (!ruler) {
        return;
    }

    const markers = ruler.querySelectorAll(".b-report-designer-ruler-marker");
    const range = ruler.querySelector(".b-report-designer-ruler-marker-range");

    if (range) {
        range.style.top = `calc(var(--b-report-designer-ruler-surface-offset) + ${Math.max(0, y || 0)}px)`;
        range.style.height = `${Math.max(0, height || 0)}px`;
        range.hidden = false;
        range.classList.toggle("b-report-designer-ruler-marker-range-active", !!active);
    }

    if (markers[0]) {
        markers[0].style.top = `calc(var(--b-report-designer-ruler-surface-offset) + ${Math.max(0, y || 0)}px)`;
        markers[0].hidden = false;
        markers[0].classList.toggle("b-report-designer-ruler-marker-active", !!active);
    }

    if (markers[1]) {
        markers[1].style.top = `calc(var(--b-report-designer-ruler-surface-offset) + ${Math.max(0, (y || 0) + (height || 0))}px)`;
        markers[1].hidden = false;
        markers[1].classList.toggle("b-report-designer-ruler-marker-active", !!active);
    }
}

function clearDesignerRulerMarker(pageElement) {
    const root = pageElement?.closest?.(".b-report-designer-rulers");

    if (!root) {
        return;
    }

    const markers = root.querySelectorAll(".b-report-designer-ruler-marker, .b-report-designer-ruler-marker-range");

    for (const marker of markers) {
        marker.hidden = true;
        marker.classList.remove("b-report-designer-ruler-marker-active", "b-report-designer-ruler-marker-range-active");
    }
}

export function updateDesignerSectionResizePreview(pageElement, sectionId, height) {
    if (!pageElement || !sectionId) {
        return;
    }

    const section = findReportSection(pageElement, sectionId);

    if (!section) {
        return;
    }

    let preview = designerSectionResizePreviews.get(pageElement);

    if (!preview || preview.section !== section) {
        preview = {
            section,
            originalHeight: section.style.height || "",
        };
        designerSectionResizePreviews.set(pageElement, preview);
    }

    section.style.height = `${Math.max(0, height || 0)}px`;
}

export function clearDesignerSectionResizePreview(pageElement) {
    const preview = designerSectionResizePreviews.get(pageElement);

    if (!preview) {
        return;
    }

    preview.section.style.height = preview.originalHeight;
    designerSectionResizePreviews.delete(pageElement);
}

export function commitDesignerSectionResizePreview(pageElement) {
    designerSectionResizePreviews.delete(pageElement);
}

function findReportSection(pageElement, sectionId) {
    const sections = pageElement.querySelectorAll("[data-report-section-id]");

    for (const section of sections) {
        if (section.getAttribute("data-report-section-id") === sectionId) {
            return section;
        }
    }

    return null;
}

function getDesignerInteractionOverlays(pageElement) {
    if (!pageElement || typeof pageElement.appendChild !== "function") {
        return null;
    }

    let overlays = designerInteractionOverlays.get(pageElement);

    if (overlays) {
        return overlays;
    }

    const selection = document.createElement("div");
    selection.className = "b-report-selection-box";
    selection.hidden = true;

    const drag = document.createElement("div");
    drag.className = "b-report-drag-preview";
    drag.hidden = true;

    pageElement.appendChild(selection);
    pageElement.appendChild(drag);

    overlays = { selection, drag };
    designerInteractionOverlays.set(pageElement, overlays);

    return overlays;
}

function positionOverlay(element, x, y, width, height) {
    element.style.left = `${Math.max(0, x || 0)}px`;
    element.style.top = `${Math.max(0, y || 0)}px`;
    element.style.width = `${Math.max(0, width || 0)}px`;
    element.style.height = `${Math.max(0, height || 0)}px`;
}

function clearSectionResize(resize) {
    removeSectionResizeListeners(resize);

    if (sectionResize === resize) {
        sectionResize = null;
    }
}

function addSectionResizeListeners(resize) {
    const observer = getDocumentObserver();

    if (!observer) {
        return;
    }

    resize.subscriptions = [
        createDocumentObserverSubscriptionId("section-resize-move"),
        createDocumentObserverSubscriptionId("section-resize-end"),
        createDocumentObserverSubscriptionId("section-resize-cancel"),
    ];

    observer.subscribe({
        id: resize.subscriptions[0],
        ownerId: resize.ownerId,
        eventNames: ["pointermove", "mousemove", "touchmove"],
        capture: true,
        preventDefault: true,
        throttle: true,
        handler: resize.move,
    });

    observer.subscribe({
        id: resize.subscriptions[1],
        ownerId: resize.ownerId,
        eventNames: ["pointerup", "mouseup", "touchend", "dragend"],
        capture: true,
        preventDefault: true,
        handler: resize.end,
    });

    observer.subscribe({
        id: resize.subscriptions[2],
        ownerId: resize.ownerId,
        eventNames: ["pointercancel", "touchcancel", "blur"],
        capture: true,
        preventDefault: true,
        handler: resize.cancel,
    });
}

function removeSectionResizeListeners(resize) {
    const observer = getDocumentObserver();

    if (!observer) {
        return;
    }

    if (resize.subscriptions) {
        for (const subscriptionId of resize.subscriptions) {
            observer.unsubscribe(subscriptionId);
        }
    }

    observer.releasePointer(resize.ownerId, resize.pointerId);
}

function getDocumentObserver() {
    return globalThis.Blazorise?.documentObserver ?? null;
}

function createDocumentObserverOwnerId(scope) {
    return createDocumentObserverSubscriptionId(`${scope}-owner`);
}

function createDocumentObserverSubscriptionId(scope) {
    nextSubscriptionId++;

    return `reporting-${scope}-${nextSubscriptionId}`;
}

async function completeSectionResize(resize, clientY) {
    if (!resize || resize.ended) {
        return;
    }

    resize.ended = true;
    clearSectionResize(resize);

    await resize.dotNetReference.invokeMethodAsync("OnDocumentSectionResizeEnd", clientY);
}

async function cancelSectionResize(event) {
    if (event) {
        event.preventDefault();
    }

    const resize = sectionResize;

    if (!resize || resize.ended) {
        return;
    }

    resize.ended = true;
    clearSectionResize(resize);

    await resize.dotNetReference.invokeMethodAsync("OnDocumentSectionResizeCancel");
}

function getClientY(event, fallback) {
    if (typeof event?.clientY === "number") {
        return event.clientY;
    }

    const touch = event?.changedTouches?.[0] ?? event?.touches?.[0];

    return typeof touch?.clientY === "number"
        ? touch.clientY
        : fallback;
}

function createTransparentDragImage() {
    const canvas = document.createElement("canvas");
    canvas.width = 1;
    canvas.height = 1;

    return canvas;
}

function resolveDesignerShortcut(event) {
    if (isDesignerControlShortcut(event)) {
        return designerControlShortcuts[event.key?.toLowerCase?.()] ?? null;
    }

    if (isDesignerControlShiftShortcut(event)) {
        return designerControlShiftShortcuts[event.key?.toLowerCase?.()] ?? null;
    }

    return isDesignerPlainShortcut(event)
        ? designerPlainShortcuts[event.key] ?? null
        : null;
}

function isDesignerControlShortcut(event) {
    return event.ctrlKey && !event.shiftKey && !event.altKey && !event.metaKey;
}

function isDesignerControlShiftShortcut(event) {
    return event.ctrlKey && event.shiftKey && !event.altKey && !event.metaKey;
}

function isDesignerPlainShortcut(event) {
    return !event.ctrlKey && !event.shiftKey && !event.altKey && !event.metaKey;
}

function shouldIgnoreDesignerShortcut(event) {
    const target = event.target;

    if (!target) {
        return false;
    }

    const tagName = target.tagName?.toLowerCase?.();

    return tagName === "input"
        || tagName === "textarea"
        || tagName === "select"
        || target.isContentEditable
        || !!target.closest?.(".b-report-element-text-editor");
}

function handleTextExpressionTokenKeyDown(element, event) {
    if (event.key === "Backspace" || event.key === "Delete") {
        handleTextExpressionTokenDelete(element, event);
        return;
    }

    if (isTextInputKey(event)) {
        prepareTextExpressionTokenReplacement(element);
    }
}

function handleTextExpressionTokenDelete(element, event) {
    const selectionStart = element.selectionStart;
    const selectionEnd = element.selectionEnd;

    if (typeof selectionStart !== "number" || typeof selectionEnd !== "number") {
        return;
    }

    const range = getProtectedTokenDeletionRange(element.value ?? "", selectionStart, selectionEnd, event.key);

    if (!range) {
        return;
    }

    event.preventDefault();
    replaceTextRange(element, range.start, range.end, "");
}

function prepareTextExpressionTokenReplacement(element) {
    const selectionStart = element.selectionStart;
    const selectionEnd = element.selectionEnd;

    if (typeof selectionStart !== "number" || typeof selectionEnd !== "number") {
        return;
    }

    const range = getProtectedTokenReplacementRange(element.value ?? "", selectionStart, selectionEnd);

    if (range) {
        element.setSelectionRange(range.start, range.end);
    }
}

function getProtectedTokenDeletionRange(value, selectionStart, selectionEnd, key) {
    const tokenRanges = getExpressionTokenRanges(value);

    if (tokenRanges.length === 0) {
        return null;
    }

    if (selectionStart !== selectionEnd) {
        let start = selectionStart;
        let end = selectionEnd;
        let expanded = false;

        for (const tokenRange of tokenRanges) {
            if (tokenRange.end <= selectionStart || tokenRange.start >= selectionEnd) {
                continue;
            }

            start = Math.min(start, tokenRange.start);
            end = Math.max(end, tokenRange.end);
            expanded = true;
        }

        return expanded ? { start, end } : null;
    }

    for (const tokenRange of tokenRanges) {
        if (key === "Backspace" && selectionStart > tokenRange.start && selectionStart <= tokenRange.end) {
            return tokenRange;
        }

        if (key === "Delete" && selectionStart >= tokenRange.start && selectionStart < tokenRange.end) {
            return tokenRange;
        }
    }

    return null;
}

function getProtectedTokenReplacementRange(value, selectionStart, selectionEnd) {
    const tokenRanges = getExpressionTokenRanges(value);

    if (tokenRanges.length === 0) {
        return null;
    }

    if (selectionStart !== selectionEnd) {
        let start = selectionStart;
        let end = selectionEnd;
        let expanded = false;

        for (const tokenRange of tokenRanges) {
            if (tokenRange.end <= selectionStart || tokenRange.start >= selectionEnd) {
                continue;
            }

            start = Math.min(start, tokenRange.start);
            end = Math.max(end, tokenRange.end);
            expanded = true;
        }

        return expanded ? { start, end } : null;
    }

    for (const tokenRange of tokenRanges) {
        if (selectionStart > tokenRange.start && selectionStart < tokenRange.end) {
            return tokenRange;
        }
    }

    return null;
}

function isTextInputKey(event) {
    return !event.ctrlKey
        && !event.metaKey
        && !event.altKey
        && event.key?.length === 1;
}

function getExpressionTokenRanges(value) {
    const ranges = [];
    const expressionRegex = /\{[^{}\r\n]+\}/g;
    let match;

    while ((match = expressionRegex.exec(value)) !== null) {
        ranges.push({
            start: match.index,
            end: match.index + match[0].length,
        });
    }

    return ranges;
}

function replaceTextRange(element, start, end, replacement) {
    const value = element.value ?? "";
    element.value = value.substring(0, start) + replacement + value.substring(end);
    element.setSelectionRange(start + replacement.length, start + replacement.length);
    dispatchInputEvent(element);
}

function dispatchInputEvent(element) {
    let event;

    try {
        event = new InputEvent("input", { bubbles: true, inputType: "deleteContentBackward" });
    }
    catch {
        event = new Event("input", { bubbles: true });
    }

    element.dispatchEvent(event);
}