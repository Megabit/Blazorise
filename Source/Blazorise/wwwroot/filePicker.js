import { getRequiredElement } from "./utilities.js?v=1.6.1.0";
import { getFilesAsync } from "./fileEdit.js?v=1.6.1.0";

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
        element.addEventListener("dragenter", async (e) => await onDragHover(e, element), false);
        element.addEventListener("dragover", async (e) => await onDragHover(e, element), false);
        element.addEventListener("dragleave", onDragLeave);
        element.addEventListener("drop", async (e) => await onDrop(e, element), false);
        element.addEventListener('paste', (e) => onPaste(e, element));
    }
}

function onDragHover(e, element) {
    if (element.fileInput.disabled) {
        e.dataTransfer.dropEffect = "none";
        if (element.fileInput == e.target){
            //Fallback to FileEdit behavior
            return;
        }

    }
    e.preventDefault();
}

function onDragLeave(e) {
    e.preventDefault();
}

async function onDrop(e, element) {
    e.preventDefault();
    let fileInput = getFileInput(element);

    if (fileInput.disabled)
        return;

    let _files = await getFilesAsync(e.dataTransfer, fileInput.webkitdirectory, fileInput.multiple);
    fileInput.files = _files;

    const event = new Event('change', { bubbles: true });
    fileInput.dispatchEvent(event);
}

function onPaste(e, element) {
    let fileInput = getFileInput(element);

    fileInput.files = e.clipboardData.files;
    const event = new Event('change', { bubbles: true });
    fileInput.dispatchEvent(event);
}

function setFileInput(element) {
    let fileInput = element.querySelector("input[type=file]");
    _instances[element.id].fileInput = fileInput;
    return fileInput;
}

function getFileInput(element) {
    return _instances[element.id].fileInput;
}