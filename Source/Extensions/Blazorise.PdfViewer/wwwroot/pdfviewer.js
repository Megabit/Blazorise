import "./vendors/pdf.min.mjs?v=1.6.0.0";
import { getRequiredElement, insertCSSIntoDocument } from "../Blazorise/utilities.js?v=1.6.0.0";

const { pdfjsLib } = globalThis;

if (pdfjsLib && pdfjsLib.GlobalWorkerOptions) {
    pdfjsLib.GlobalWorkerOptions.workerSrc = "./pdf.worker.min.mjs?v=1.6.0.0";
}
else {
    console.error("Blazorise.PdfViewer: Could not find pdfjsLib.");
}

insertCSSIntoDocument("_content/Blazorise.PdfViewer/vendors/pdf_viewer.min.css?v=1.6.0.0");

const _instances = [];

export async function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = {
        dotNetAdapter: dotNetAdapter,
        canvas: element,
        options: options,
        pageNumber: 1,
        totalPages: 0,
        pageRendering: false,
        pageNumberPending: null,
    };

    loadDocument(instance, options.source);

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

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (instance && instance.player && options) {
        if (options.source.changed) {
            loadDocument(instance, options.source.value);
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
        const viewport = page.getViewport({ scale: instance.options.scale, rotation: instance.options.rotation });

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

        instance.options.scale = scale;
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
        scale: instance.options.scale,
    });
}