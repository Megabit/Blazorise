import { getRequiredElement } from "./utilities.js?v=1.0.4.0";

const _instances = [];
let fileInput;
export function initialize(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    _instances[elementId] = element;
    initializeDropZone(element);
}

export function destroy(element, elementId) {
    var instances = _instances || {};
    delete instances[elementId];
}

function initializeDropZone(element) {
    fileInput = element.querySelector("input[type=file]");
    if (fileInput) {
        element.addEventListener("dragenter", onDragHover);
        element.addEventListener("dragover", onDragHover);
        element.addEventListener("dragleave", onDragLeave);
        element.addEventListener("drop", onDrop);
        element.addEventListener('paste', onPaste);
    }
}

function onDragHover(e) {
    e.preventDefault();
}

function onDragLeave(e) {
    e.preventDefault();
}

function onDrop(e) {
    e.preventDefault();
    fileInput.files = getOnlyTrueFiles(e.dataTransfer.files);
    const event = new Event('change', { bubbles: true });
    fileInput.dispatchEvent(event);
}

function onPaste(e) {
    fileInput.files = getOnlyTrueFiles(e.clipboardData.files);
    const event = new Event('change', { bubbles: true });
    fileInput.dispatchEvent(event);
}

function getOnlyTrueFiles(files) {
    const dt = new DataTransfer();
    for (let i = 0; i < files.length; i++) {
        let current = files[i];
        if (current.type != "")
            dt.items.add(current);
    }
    return dt.files;
}


