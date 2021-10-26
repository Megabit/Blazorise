const _instances = [];

export function initialize(adapter, element, elementId) {
    var nextFileId = 0;

    // save an instance of adapter
    _instances[elementId] = new FileEditInfo(adapter, element, elementId);

    element.addEventListener('change', function handleInputFileChange(event) {
        // Reduce to purely serializable data, plus build an index by ID
        element._blazorFilesById = {};

        var fileList = Array.prototype.map.call(element.files, function (file) {
            var fileEntry = {
                id: ++nextFileId,
                lastModified: new Date(file.lastModified).toISOString(),
                name: file.name,
                size: file.size,
                type: file.type
            };
            element._blazorFilesById[fileEntry.id] = fileEntry;

            // Attach the blob data itself as a non-enumerable property so it doesn't appear in the JSON
            Object.defineProperty(fileEntry, 'blob', { value: file });

            return fileEntry;
        });

        adapter.invokeMethodAsync('NotifyChange', fileList).then(null, function (err) {
            throw new Error(err);
        });
    });
}

export function destroy(element, elementId) {
    var instances = _instances || {};
    delete instances[elementId];
}

export function reset(element, elementId) {
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

class FileEditInfo {
    adapter;
    element;
    elementId;

    constructor(adapter, element, elementId) {
        this.adapter = adapter;
        this.element = element;
        this.elementId = elementId;
    }
}