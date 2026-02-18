import "./vendors/pdf.min.mjs?v=2.0.0.0";
import { getRequiredElement, insertCSSIntoDocumentHead } from "../Blazorise/utilities.js?v=2.0.0.0";

const { pdfjsLib } = globalThis;

if (pdfjsLib && pdfjsLib.GlobalWorkerOptions) {
    pdfjsLib.GlobalWorkerOptions.workerSrc = "_content/Blazorise.PdfViewer/vendors/pdf.worker.min.mjs?v=2.0.0.0";
}
else {
    console.error("Blazorise.PdfViewer: Could not find pdfjsLib.");
}

insertCSSIntoDocumentHead("_content/Blazorise.PdfViewer/vendors/pdf_viewer.min.css?v=2.0.0.0");

const _instances = [];

export async function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = {
        dotNetAdapter: dotNetAdapter,
        canvas: element,
        source: options.source,
        pageNumber: options.pageNumber,
        totalPages: 0,
        scale: options.scale,
        rotation: options.rotation,
        pageRendering: false,
        pageNumberPending: null,
        password: null,
        loadingTask: null,
        loadingVersion: 0,
    };

    loadDocument(instance, instance.source);

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        if (instance.loadingTask) {
            try {
                instance.loadingTask.destroy();
            }
            catch {
            }
        }

        if (instance.pdf) {
            instance.pdf.destroy();
        }

        delete instances[elementId];
    }
}

export function updateOptions(element, elementId, newOptions) {
    const instance = _instances[elementId];

    if (instance && newOptions) {
        let queueNeeded = false;

        if (instance.pdf && newOptions.pageNumber.changed && newOptions.pageNumber.value >= 1 && newOptions.pageNumber.value <= instance.totalPages) {
            instance.pageNumber = newOptions.pageNumber.value;

            queueNeeded = true;
        }

        if (instance.pdf && newOptions.scale.changed) {
            instance.scale = newOptions.scale.value;

            queueNeeded = true;
        }

        if (instance.pdf && newOptions.rotation.changed) {
            instance.rotation = newOptions.rotation.value;

            queueNeeded = true;
        }

        if (queueNeeded && !newOptions.source.changed) {
            queueRenderPage(instance, instance.pageNumber);

            NotifyPdfChanged(instance);
        }
        else if (newOptions.source.changed) {
            instance.source = newOptions.source.value;
            loadDocument(instance, instance.source);
        }
    }
}

function loadDocument(instance, source) {
    if (!instance || !source) {
        return;
    }

    instance.loadingVersion++;
    const currentLoadingVersion = instance.loadingVersion;
    instance.password = null;

    if (instance.loadingTask) {
        try {
            instance.loadingTask.destroy();
        }
        catch {
        }
    }

    if (instance.pdf) {
        try {
            instance.pdf.destroy();
        }
        catch {
        }
    }

    const loadingTask = pdfjsLib.getDocument(source);
    instance.loadingTask = loadingTask;

    let attempt = 0;

    loadingTask.onPassword = async (updatePassword, reason) => {
        if (currentLoadingVersion !== instance.loadingVersion) {
            return;
        }

        attempt++;

        try {
            const password = await requestPasswordFromDotNet(instance, reason, attempt);

            if (password !== null && password !== undefined) {
                const normalizedPassword = typeof password === "string" ? password : String(password);
                instance.password = normalizedPassword;
                updatePassword(normalizedPassword);
            }
            else {
                try {
                    loadingTask.destroy();
                }
                catch {
                }
            }
        }
        catch (error) {
            console.error(error);

            try {
                loadingTask.destroy();
            }
            catch {
            }
        }
    };

    loadingTask.promise.then(function (pdf) {
        if (currentLoadingVersion !== instance.loadingVersion) {
            if (pdf) {
                pdf.destroy();
            }

            return;
        }

        instance.loadingTask = null;
        instance.pdf = pdf;
        instance.totalPages = pdf.numPages;

        if (instance.pageNumber > instance.totalPages) {
            instance.pageNumber = instance.totalPages;
        }

        if (instance.pageNumber < 1) {
            instance.pageNumber = 1;
        }

        renderPage(instance, instance.pageNumber);

        NotifyPdfInitialized(instance);
    }, function (reason) {
        if (currentLoadingVersion !== instance.loadingVersion) {
            return;
        }

        instance.loadingTask = null;
        console.error(reason);
    });
}

async function requestPasswordFromDotNet(instance, reason, attempt) {
    if (!instance || !instance.dotNetAdapter) {
        return null;
    }

    return await instance.dotNetAdapter.invokeMethodAsync('RequestPdfPassword', {
        reason: reason,
        attempt: attempt,
        source: instance.source,
    });
}

function getDocumentSource(instance, source, includePassword) {
    const currentSource = source ?? instance?.source;

    if (!includePassword || !instance?.password) {
        return currentSource;
    }

    return applyPasswordToSource(currentSource, instance.password);
}

function applyPasswordToSource(source, password) {
    if (!password) {
        return source;
    }

    if (typeof source === 'string') {
        return { url: source, password: password };
    }

    if (source instanceof Uint8Array) {
        return { data: source, password: password };
    }

    if (source instanceof ArrayBuffer) {
        return { data: new Uint8Array(source), password: password };
    }

    if (source && typeof source === 'object') {
        return Object.assign({}, source, { password: password });
    }

    return source;
}

function renderPage(instance, pageNumber) {
    instance.pageRendering = true;

    instance.pdf.getPage(pageNumber).then(function (page) {
        const viewport = page.getViewport({ scale: instance.scale, rotation: instance.rotation });

        const canvas = instance.canvas;
        const context = canvas.getContext('2d');
        canvas.height = viewport.height;
        canvas.width = viewport.width;

        const renderContext = {
            canvasContext: context,
            viewport: viewport
        };

        const renderTask = page.render(renderContext);

        renderTask.promise.then(function () {
            instance.pageRendering = false;

            if (instance.pageNumberPending !== null) {
                renderPage(instance, instance.pageNumberPending);
                instance.pageNumberPending = null;
            }
        });
    });
}

export async function print(element, elementId, source) {
    const instance = _instances[elementId];
    const documentSource = getDocumentSource(instance, source, true);
    const iframe = document.createElement('iframe');
    iframe.style = 'display: none';
    document.body.appendChild(iframe);

    const canvasContainer = document.createElement('div');
    let pdf = null;

    try {
        pdf = await pdfjsLib.getDocument(documentSource).promise;

        if (pdf) {
            for (let pageNumber = 1; pageNumber <= pdf.numPages; pageNumber++) {
                const page = await pdf.getPage(pageNumber);

                if (!page) {
                    continue;
                }

                const viewport = page.getViewport({ scale: 1.5, rotation: 0 });
                const canvas = document.createElement("canvas");
                const context = canvas.getContext('2d');
                canvas.width = viewport.width;
                canvas.height = viewport.height;

                const renderContext = {
                    canvasContext: context,
                    viewport: viewport
                };

                await page.render(renderContext).promise;

                canvasContainer.appendChild(canvas);
            }
        }

        if (iframe.contentWindow && iframe.contentWindow.document) {
            iframe.contentWindow.document.body.appendChild(canvasContainer);
            iframe.contentWindow.print();
        }
    }
    finally {
        if (pdf && typeof pdf.destroy === "function") {
            try {
                pdf.destroy();
            }
            catch {
            }
        }

        if (iframe && iframe.parentNode) {
            iframe.parentNode.removeChild(iframe);
        }
    }
}

export async function download(element, elementId, source) {
    const instance = _instances[elementId];
    const documentSource = getDocumentSource(instance, source, true);

    try {
        const loadingTask = pdfjsLib.getDocument(documentSource);
        const pdf = await loadingTask.promise;

        if (!pdf || !pdf._transport || !pdf._transport._params) {
            console.error("Unable to access raw PDF data.");
            return;
        }

        const src = source?.url || source || instance?.source;

        // Case 1: source is a URL
        if (typeof src === 'string' && !src.startsWith('data:')) {
            // Simply trigger a download link
            const link = document.createElement('a');
            link.href = src;
            link.download = getFileNameFromUrl(src);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
        // Case 2: source is base64 data
        else if (src.startsWith('data:application/pdf;base64,')) {
            const link = document.createElement('a');
            link.href = src;
            link.download = 'document.pdf';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
        else {
            // Case 3: not yet supported in Blazorise
            throw new Error(
                "The provided source format is not currently supported for downloading.\n\n" +
                "Blazorise PdfViewer currently supports:\n" +
                " - direct file URLs\n" +
                " - base64-encoded data URIs\n\n" +
                "Support for advanced sources like streamed binary data (e.g., fetched Uint8Array or blob) " +
                "may be added in the future to allow download of dynamically generated or protected PDFs."
            );
        }
    } catch (error) {
        console.error("PDF Save failed:", error);
    }
}

export function queueRenderPage(instance, pageNumber) {
    if (instance && pageNumber) {
        if (instance.pageRendering) {
            instance.pageNumberPending = pageNumber;
        } else {
            renderPage(instance, pageNumber);
        }
    }
}

export function prevPage(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        if (instance.pageNumber <= 1) {
            return;
        }

        instance.pageNumber--;
        queueRenderPage(instance, instance.pageNumber);

        NotifyPdfChanged(instance);
    }
}

export function nextPage(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        if (instance.pageNumber >= instance.totalPages) {
            return;
        }

        instance.pageNumber++;
        queueRenderPage(instance, instance.pageNumber);

        NotifyPdfChanged(instance);
    }
}

export function goToPage(element, elementId, pageNumber) {
    const instance = _instances[elementId];

    if (instance) {
        if (pageNumber <= 1 || pageNumber >= instance.totalPages) {
            return;
        }

        instance.pageNumber = pageNumber;
        queueRenderPage(instance, instance.pageNumber);

        NotifyPdfChanged(instance);
    }
}

export function setScale(element, elementId, scale) {
    const instance = _instances[elementId];

    if (instance) {

        instance.scale = scale;
        queueRenderPage(instance, instance.pageNumber);

        NotifyPdfChanged(instance);
    }
}

function NotifyPdfInitialized(instance) {
    instance.dotNetAdapter.invokeMethodAsync('NotifyPdfInitialized', {
        pageNumber: instance.pageNumber,
        totalPages: instance.totalPages,
    });
}

function NotifyPdfChanged(instance) {
    instance.dotNetAdapter.invokeMethodAsync('NotifyPdfChanged', {
        pageNumber: instance.pageNumber,
        totalPages: instance.totalPages,
        scale: instance.scale,
    });
}

function getFileNameFromUrl(url) {
    try {
        return url.split('/').pop().split('?')[0] || 'document.pdf';
    } catch {
        return 'document.pdf';
    }
}