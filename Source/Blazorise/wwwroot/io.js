export async function readFileDataStream(elem, fileId) {
    const file = getFileById(elem, fileId);
    return file.blob;
}

export function readFileData(element, fileEntryId, position, length) {
    var readPromise = getArrayBufferFromFileAsync(element, fileEntryId);

    return readPromise.then(function (arrayBuffer) {
        return new Uint8Array(arrayBuffer, position, length);
    });
}

function ensureArrayBufferReadyForSharedMemoryInterop(element, fileId) {
    return getArrayBufferFromFileAsync(element, fileId).then(function (arrayBuffer) {
        getFileById(element, fileId).arrayBuffer = arrayBuffer;
    });
}

function readFileDataSharedMemory(readRequest) {
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
}


function getFileById(element, fileId) {
    var file = element._blazorFilesById[fileId];

    if (!file) {
        throw new Error('There is no file with ID ' + fileId + '. The file list may have changed');
    }

    return file;
}

function getArrayBufferFromFileAsync(element, fileId) {
    var file = getFileById(element, fileId);

    // On the first read, convert the FileReader into a Promise<ArrayBuffer>
    if (!file.readPromise) {
        file.readPromise = new Promise(function (resolve, reject) {
            var reader = new FileReader();
            reader.onload = function () { resolve(reader.result); };
            reader.onerror = function (err) { reject(err); };
            reader.readAsArrayBuffer(file.blob);
        });
    }

    return file.readPromise;
}

var uint8ToBase64 = (function () {
    // Code from https://github.com/beatgammit/base64-js/
    // License: MIT
    var lookup = [];

    var code = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    for (var i = 0, len = code.length; i < len; ++i) {
        lookup[i] = code[i];
    }

    function tripletToBase64(num) {
        return lookup[num >> 18 & 0x3F] +
            lookup[num >> 12 & 0x3F] +
            lookup[num >> 6 & 0x3F] +
            lookup[num & 0x3F];
    }

    function encodeChunk(uint8, start, end) {
        var tmp;
        var output = [];
        for (var i = start; i < end; i += 3) {
            tmp =
                ((uint8[i] << 16) & 0xFF0000) +
                ((uint8[i + 1] << 8) & 0xFF00) +
                (uint8[i + 2] & 0xFF);
            output.push(tripletToBase64(tmp));
        }
        return output.join('');
    }

    return function fromByteArray(uint8) {
        var tmp;
        var len = uint8.length;
        var extraBytes = len % 3; // if we have 1 byte left, pad 2 bytes
        var parts = [];
        var maxChunkLength = 16383; // must be multiple of 3

        // go through the array every three bytes, we'll deal with trailing stuff later
        for (var i = 0, len2 = len - extraBytes; i < len2; i += maxChunkLength) {
            parts.push(encodeChunk(
                uint8, i, (i + maxChunkLength) > len2 ? len2 : (i + maxChunkLength)
            ));
        }

        // pad the end with zeros, but make sure to not forget the extra bytes
        if (extraBytes === 1) {
            tmp = uint8[len - 1];
            parts.push(
                lookup[tmp >> 2] +
                lookup[(tmp << 4) & 0x3F] +
                '=='
            );
        } else if (extraBytes === 2) {
            tmp = (uint8[len - 2] << 8) + uint8[len - 1];
            parts.push(
                lookup[tmp >> 10] +
                lookup[(tmp >> 4) & 0x3F] +
                lookup[(tmp << 2) & 0x3F] +
                '='
            );
        }

        return parts.join('');
    };
})();