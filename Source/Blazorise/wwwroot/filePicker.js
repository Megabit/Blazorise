import { getRequiredElement } from "./utilities.js?v=1.1.2.0";

const _instances = [];
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
    let fileInput = setFileInput(element);
    if (fileInput) {
        element.addEventListener("dragenter", onDragHover);
        element.addEventListener("dragover", onDragHover);
        element.addEventListener("dragleave", onDragLeave);
        element.addEventListener("drop", (e) => onDrop(e, element));
        element.addEventListener('paste', (e) => onPaste(e, element));
    }
}

function onDragHover(e) {
    e.preventDefault();
}

function onDragLeave(e) {
    e.preventDefault();
}

function onDrop(e, element) {
    e.preventDefault();
    console.log(element);
    let fileInput = getFileInput(element);

    fileInput.files = getOnlyTrueFiles(e.dataTransfer.files);
    const event = new Event('change', { bubbles: true });
    fileInput.dispatchEvent(event);
}

function onPaste(e, element) {
    let fileInput = getFileInput(element);

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

function setFileInput(element) {
    let fileInput = element.querySelector("input[type=file]");
    _instances[element.id].fileInput = fileInput;
    return fileInput;
}

function getFileInput(element) {
    return _instances[element.id].fileInput;
}
