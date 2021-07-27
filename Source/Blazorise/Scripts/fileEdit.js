window.blazorise.FileEditInfo = function (adapter, element, elementId) {
    this.adapter = adapter;
    this.element = element;
    this.elementId = elementId;
};

window.blazorise.fileEdit = {
    _instances: [],

    initialize: (adapter, element, elementId) => {
        var nextFileId = 0;

        // save an instance of adapter
        window.blazorise.fileEdit._instances[elementId] = new window.blazorise.FileEditInfo(adapter, element, elementId);

        element.addEventListener('change', function handleInputFileChange(event) {
            // Reduce to purely serializable data, plus build an index by ID
            element._blazorFilesById = {};
            var fileList = Array.prototype.map.call(element.files, function (file) {
                var result = {
                    id: ++nextFileId,
                    lastModified: new Date(file.lastModified).toISOString(),
                    name: file.name,
                    size: file.size,
                    type: file.type
                };
                element._blazorFilesById[result.id] = result;

                // Attach the blob data itself as a non-enumerable property so it doesn't appear in the JSON
                Object.defineProperty(result, 'blob', { value: file });

                return result;
            });

            adapter.invokeMethodAsync('NotifyChange', fileList).then(null, function (err) {
                throw new Error(err);
            });
        });
    },
    destroy: (element, elementId) => {
        var instances = window.blazorise.fileEdit._instances || {};
        delete instances[elementId];
    },

    reset: (element, elementId) => {
        if (element) {
            element.value = '';

            var fileEditInfo = window.blazorise.fileEdit._instances[elementId];

            if (fileEditInfo) {
                fileEditInfo.adapter.invokeMethodAsync('NotifyChange', []).then(null, function (err) {
                    throw new Error(err);
                });
            }
        }
    },

    readFileData: function readFileData(element, fileEntryId, position, length) {
        var readPromise = getArrayBufferFromFileAsync(element, fileEntryId);

        return readPromise.then(function (arrayBuffer) {
            var uint8Array = new Uint8Array(arrayBuffer, position, length);
            var base64 = uint8ToBase64(uint8Array);
            return base64;
        });
    },

    ensureArrayBufferReadyForSharedMemoryInterop: function ensureArrayBufferReadyForSharedMemoryInterop(elem, fileId) {
        return getArrayBufferFromFileAsync(elem, fileId).then(function (arrayBuffer) {
            getFileById(elem, fileId).arrayBuffer = arrayBuffer;
        });
    },

    readFileDataSharedMemory: function readFileDataSharedMemory(readRequest) {
        // This uses various unsupported internal APIs. Beware that if you also use them,
        // your code could become broken by any update.
        var inputFileElementReferenceId = Blazor.platform.readStringField(readRequest, 0);
        var inputFileElement = document.querySelector('[_bl_' + inputFileElementReferenceId + ']');
        var fileId = Blazor.platform.readInt32Field(readRequest, 4);
        var sourceOffset = Blazor.platform.readUint64Field(readRequest, 8);
        var destination = Blazor.platform.readInt32Field(readRequest, 16);
        var destinationOffset = Blazor.platform.readInt32Field(readRequest, 20);
        var maxBytes = Blazor.platform.readInt32Field(readRequest, 24);

        var sourceArrayBuffer = getFileById(inputFileElement, fileId).arrayBuffer;
        var bytesToRead = Math.min(maxBytes, sourceArrayBuffer.byteLength - sourceOffset);
        var sourceUint8Array = new Uint8Array(sourceArrayBuffer, sourceOffset, bytesToRead);

        var destinationUint8Array = Blazor.platform.toUint8Array(destination);
        destinationUint8Array.set(sourceUint8Array, destinationOffset);

        return bytesToRead;
    },
    open: (element, elementId) => {
        if (!element && elementId) {
            element = document.getElementById(elementId);
        }

        if (element) {
            element.click();
        }
    }
};

