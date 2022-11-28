import { getRequiredElement } from "./utilities.js?v=1.1.3.0";

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
        element.addEventListener("drop", async (e) => await onDrop(e, element), false);
        element.addEventListener('paste', (e) => onPaste(e, element));
    }
}

function onDragHover(e) {
    e.preventDefault();
}

function onDragLeave(e) {
    e.preventDefault();
}

async function onDrop(e, element) {
    e.preventDefault();
    console.log(element);
    let fileInput = getFileInput(element);

    let _files = await getFilesAsync(e.dataTransfer);
    var dt = new DataTransfer();

    for (var i = 0; i < _files.length; i++) {
        dt.items.add(_files[i]);
    }
    fileInput.files = dt.files;

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

async function getFilesAsync(dataTransfer) {
    const files = [];
    const queue = [];
    for (let i = 0; i < dataTransfer.items.length; i++) {
        const item = dataTransfer.items[i];
        if (item.kind === "file") {
            if (typeof item.webkitGetAsEntry === "function") {
                const entry = item.webkitGetAsEntry();
                queue.push(readEntryContentAsync(entry).then(x => files.push(...x)));
                continue;
            }

            const file = item.getAsFile();
            if (file) {
                files.push(file);
            }
        }
    }
    await Promise.all(queue);
    return files;
}

// Returns a promise with all the files of the directory hierarchy
function readEntryContentAsync(entry) {
    return new Promise((resolve, reject) => {
        let reading = 0;
        const contents = [];

        readEntry(entry);

        function readEntry(entry) {
            if (entry.isFile) {
                reading++;
                entry.file(file => {
                    reading--;
                    contents.push(file);

                    if (reading === 0) {
                        resolve(contents);
                    }
                });
            } else if (entry.isDirectory) {
                readReaderContent(entry.createReader());
            }
        }

        function readReaderContent(reader) {
            reading++;

            reader.readEntries(function (entries) {
                reading--;
                for (const entry of entries) {
                    readEntry(entry);
                }

                if (reading === 0) {
                    resolve(contents);
                }
            });
        }
    });
}
