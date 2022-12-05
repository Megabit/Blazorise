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
