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
    for (let i = 0; i < dataTransfer.items.length; i++) {
        const item = dataTransfer.items[i];
        if (item.kind === "file") {
            const entry = item.webkitGetAsEntry();
            const entryContent = await readEntryContentAsync(entry);
            files.push(...entryContent);
            continue;

            const file = item.getAsFile();
            if (file) {
                files.push(file);
            }
        }
    }

    return files;
}

// Returns a promise with all the files of the directory hierarchy
function readEntryContentAsync(entry) {
    return new Promise ((resolve, reject) => {
        let reading = 0;
        const contents = [];

        readEntry(entry, "");

        function readEntry(entry, path) {
            if (entry.isFile) {
                reading++;
                entry.file(file => {
                    file.webkitRelativePath = path + file.name;
                    reading--;
                    contents.push(file);

                    if (reading === 0) {
                        resolve(contents);
                    }
                });
            } else if (entry.isDirectory) {
                readReaderContent(entry.createReader(), path);
            }
        }

        function readReaderContent(reader, path) {
            reading++;

            reader.readEntries(function (entries) {
                reading--;
                for (const entry of entries) {
                    readEntry(entry, path + entry.name);
                }

                if (reading === 0) {
                    resolve(contents);
                }
            });
        }
    });
}
