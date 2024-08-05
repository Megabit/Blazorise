import "./vendors/pdf.min.mjs?v=1.6.0.0";

import { getRequiredElement } from "../Blazorise/utilities.js?v=1.6.0.0";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"_content/Blazorise.PdfViewer/vendors/pdf_viewer.min.css\" />");

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

    var url = 'https://raw.githubusercontent.com/mozilla/pdf.js/ba2edeae/web/compressed.tracemonkey-pldi-09.pdf';

    const { pdfjsLib } = globalThis;

    pdfjsLib.GlobalWorkerOptions.workerSrc = "./pdf.worker.min.mjs?v=1.6.0.0";

    // Asynchronous download of PDF
    var loadingTask = pdfjsLib.getDocument(url);
    loadingTask.promise.then(function (pdf) {
        instance.pdf = pdf;
        instance.totalPages = pdf.numPages;
        renderPage(instance, instance.pageNumber);

        NotifyDocumentLoaded(instance);
    }, function (reason) {
        // PDF loading error
        console.error(reason);
    });

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {


        delete instances[elementId];
    }
}

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (instance && instance.player && options) {
        if (options.source.changed) {
            // TODO
        }
    }
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

        NotifyPageNumberChanged(instance);
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

        NotifyPageNumberChanged(instance);
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

        NotifyPageNumberChanged(instance);
    }
}

function NotifyDocumentLoaded(instance) {
    instance.dotNetAdapter.invokeMethodAsync('NotifyDocumentLoaded', {
        pageNumber: instance.pageNumber,
        totalPages: instance.totalPages,
    });
}

function NotifyPageNumberChanged(instance) {
    instance.dotNetAdapter.invokeMethodAsync('NotifyPageNumberChanged', {
        pageNumber: instance.pageNumber,
        totalPages: instance.totalPages,
    });
}