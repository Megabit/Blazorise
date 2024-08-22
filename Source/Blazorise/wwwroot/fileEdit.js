import { getRequiredElement } from "./utilities.js?v=1.6.1.0";

const _instances = [];
let nextFileId = 0;
export function initialize(adapter, element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    // save an instance of adapter
    _instances[elementId] = new FileEditInfo(adapter, element, elementId);
    element.addEventListener("drop", async (e) => await onDrop(e, element), false);
    element.addEventListener('change', function handleInputFileChange(event) {

        var fileList = mapElementFilesToFileEntries(element);

        adapter.invokeMethodAsync('NotifyChange', fileList).then(null, function (err) {
            throw new Error(err);
        });
    });
}

export function removeFile(element, elementId, fileId) {
    element = getRequiredElement(element, elementId);

    if (element && element.files && element.files.length > 0) {
        const dt = new DataTransfer();

        for (let i = 0; i < element.files.length; i++) {
            const file = element.files[i];
            if (file.id != fileId)
                dt.items.add(file);
        }

        element.files = dt.files;
        element.dispatchEvent(new Event("change"));
    }
}

export function destroy(element, elementId) {
    var instances = _instances || {};
    delete instances[elementId];
}

export function reset(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (element) {
        element.value = '';

        var fileEditInfo = _instances[elementId];

        if (fileEditInfo) {
            fileEditInfo.adapter.invokeMethodAsync('NotifyChange', []).then(null, function (err) {
                throw new Error(err);
            });
        }
    }
}

export function open(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    if ('showPicker' in HTMLInputElement.prototype) {
        element.showPicker();
    }
    else {
        element.click();
    }
}

// Reduce to purely serializable data, plus build an index by ID
function mapElementFilesToFileEntries(element) {
    element._blazorFilesById = {};

    let fileList = Array.prototype.map.call(element.files, function (file) {
        file.id = file.id ?? ++nextFileId;
        var fileEntry = {
            id: file.id,
            lastModified: new Date(file.lastModified).toISOString(),
            name: file.name,
            size: file.size,
            type: file.type,
            relativePath: file.webkitRelativePath
        };
        element._blazorFilesById[fileEntry.id] = fileEntry;

        // Attach the blob data itself as a non-enumerable property so it doesn't appear in the JSON
        Object.defineProperty(fileEntry, 'blob', { value: file });

        return fileEntry;
    });
    return fileList;
}

async function onDrop(e, element) {
    e.preventDefault();
    let fileInput = element;

    if (fileInput.disabled)
        return;

    let _files = await getFilesAsync(e.dataTransfer, fileInput.webkitdirectory, fileInput.multiple);
    fileInput.files = _files;

    const event = new Event('change', { bubbles: true });
    fileInput.dispatchEvent(event);
}

export async function getFilesAsync(dataTransfer, directory, multiple) {
    const files = [];
    const queue = [];

    let fileCount = 1
    if (multiple) {
        fileCount = dataTransfer.items.length
    }

    for (let i = 0; i < fileCount; i++) {
        const item = dataTransfer.items[i];
        if (item.kind === "file") {
            if (typeof item.webkitGetAsEntry === "function") {
                const entry = item.webkitGetAsEntry();
                if (entry.isDirectory) {
                    if (!directory) {
                        continue;
                    }
                }
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

    var dt = new DataTransfer();

    for (var i = 0; i < files.length; i++) {
        dt.items.add(files[i]);
    }

    return dt.files;
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


class FileEditInfo {
    constructor(adapter, element, elementId) {
        this.adapter = adapter;
        this.element = element;
        this.elementId = elementId;
    }
}