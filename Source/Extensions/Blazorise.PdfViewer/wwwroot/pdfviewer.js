import "./vendors/pdf.min.mjs?v=1.6.1.0";
import { getRequiredElement, insertCSSIntoDocumentHead } from "../Blazorise/utilities.js?v=1.6.1.0";

const { pdfjsLib } = globalThis;

if (pdfjsLib && pdfjsLib.GlobalWorkerOptions) {
    pdfjsLib.GlobalWorkerOptions.workerSrc = "./pdf.worker.min.mjs?v=1.6.1.0";
}
else {
    console.error("Blazorise.PdfViewer: Could not find pdfjsLib.");
}

insertCSSIntoDocumentHead("_content/Blazorise.PdfViewer/vendors/pdf_viewer.min.css?v=1.6.1.0");

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
    };

    loadDocument(instance, instance.source);

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        if (instance.pdf) {
            instance.pdf.destroy();
        }

        delete instances[elementId];
    }
}

export function updateOptions(element, elementId, newOptions) {
    const instance = _instances[elementId];

    if (instance && instance.pdf && newOptions) {
        let queueNeeded = false;

        if (newOptions.pageNumber.changed && newOptions.pageNumber.value >= 1 && newOptions.pageNumber.value <= instance.totalPages) {
            instance.pageNumber = newOptions.pageNumber.value;

            queueNeeded = true;
        }

        if (newOptions.scale.changed) {
            instance.scale = newOptions.scale.value;

            queueNeeded = true;
        }

        if (newOptions.rotation.changed) {
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
    var loadingTask = pdfjsLib.getDocument(source);

    loadingTask.promise.then(function (pdf) {
        instance.pdf = pdf;
        instance.totalPages = pdf.numPages;
        renderPage(instance, instance.pageNumber);

        NotifyPdfInitialized(instance);
    }, function (reason) {
        console.error(reason);
    });
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

export async function print(source) {
    const iframe = document.createElement('iframe');
    iframe.style = 'display: none';
    document.body.appendChild(iframe);

    const canvasContainer = document.createElement('div');

    try {
        var pdf = await pdfjsLib.getDocument(source).promise;

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

        const iframeDoc = iframe.contentWindow.document;
        iframeDoc.body.appendChild(canvasContainer);
    }
    finally {
        if (iframe) {
            if (iframe.contentWindow) {
                iframe.contentWindow.print();
            }

            document.body.removeChild(iframe);

            if (iframe.contentWindow && iframe.contentWindow.document && canvasContainer) {
                iframeDoc.body.removeChild(canvasContainer);
            }
        }
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