import "./vendors/pdf.min.mjs?v=2.2.2.0";
import { getRequiredElement, insertCSSIntoDocumentHead, registerDisconnectCleanup, unregisterDisconnectCleanup } from "../Blazorise/utilities.js?v=2.2.2.0";

const { pdfjsLib } = globalThis;

const pdfViewerMode = {
    singlePage: 0,
    continuous: 1,
};

const continuousPageSpacing = 16;
const continuousOverscanCount = 2;
const continuousVirtualizePages = true;

if (pdfjsLib && pdfjsLib.GlobalWorkerOptions) {
    pdfjsLib.GlobalWorkerOptions.workerSrc = "_content/Blazorise.PdfViewer/vendors/pdf.worker.min.mjs?v=2.2.2.0";
}
else {
    console.error("Blazorise.PdfViewer: Could not find pdfjsLib.");
}

insertCSSIntoDocumentHead("_content/Blazorise.PdfViewer/vendors/pdf_viewer.min.css?v=2.2.2.0");

const _instances = [];

export async function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = {
        dotNetAdapter: dotNetAdapter,
        element: element,
        singleCanvas: null,
        source: options.source,
        pageNumber: Math.max(1, options.pageNumber || 1),
        totalPages: 0,
        scale: options.scale || 1,
        rotation: options.rotation || 0,
        mode: normalizeMode(options.mode),
        continuousPageSpacing: continuousPageSpacing,
        overscanCount: continuousOverscanCount,
        virtualizePages: continuousVirtualizePages,
        pageRendering: false,
        pageNumberPending: null,
        password: null,
        documentTitle: null,
        loadingTask: null,
        loadingVersion: 0,
        renderVersion: 0,
        destroyed: false,
        disconnectCleanupId: null,
        pagesContainer: null,
        pageViews: new Map(),
        intersectionObserver: null,
        scrollContainer: null,
        scrollHandler: null,
        scrollAnimationFrame: null,
        currentRenderTask: null,
        estimatedPageHeight: 800,
        estimatedPageWidth: 600,
    };

    _instances[elementId] = instance;
    instance.disconnectCleanupId = registerDisconnectCleanup(element, () => destroy(null, elementId, false));

    loadDocument(instance, instance.source);
}

export function destroy(element, elementId, unregisterCleanup = true) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        instance.destroyed = true;
        instance.loadingVersion++;
        instance.renderVersion++;
        instance.pageNumberPending = null;

        if (unregisterCleanup) {
            unregisterDisconnectCleanup(instance.disconnectCleanupId);
        }

        clearViewer(instance);

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

        instance.loadingTask = null;
        instance.pdf = null;
        instance.disconnectCleanupId = null;

        delete instances[elementId];
    }
}

export async function updateOptions(element, elementId, newOptions) {
    const instance = _instances[elementId];

    if (instance && newOptions) {
        const previousMode = instance.mode;
        const previousScale = instance.scale;
        const previousRotation = instance.rotation;
        const sourceChanged = newOptions.source?.changed === true;
        const pageNumberChanged = newOptions.pageNumber?.changed === true;

        if (newOptions.mode?.changed) {
            instance.mode = normalizeMode(newOptions.mode.value);
        }

        if (newOptions.pageNumber?.changed && newOptions.pageNumber.value >= 1) {
            instance.pageNumber = clampPageNumber(instance, newOptions.pageNumber.value);
        }

        if (newOptions.scale?.changed) {
            instance.scale = newOptions.scale.value || 1;
        }

        if (newOptions.rotation?.changed) {
            instance.rotation = newOptions.rotation.value || 0;
        }

        if (sourceChanged) {
            instance.source = newOptions.source.value;
            loadDocument(instance, instance.source);
            return;
        }

        if (!instance.pdf) {
            return;
        }

        const layoutChanged = previousMode !== instance.mode
            || previousScale !== instance.scale
            || previousRotation !== instance.rotation;

        if (layoutChanged) {
            await renderViewer(instance, ++instance.renderVersion, instance.pageNumber);
            NotifyPdfChanged(instance);
            return;
        }

        if (instance.mode === pdfViewerMode.continuous) {
            if (pageNumberChanged) {
                scrollToPage(instance, instance.pageNumber, true);
            }
            else {
                renderVisiblePages(instance);
            }
        }
        else if (pageNumberChanged) {
            queueRenderPage(instance, instance.pageNumber);

            NotifyPdfChanged(instance);
        }
    }
}

function loadDocument(instance, source) {
    if (!instance || instance.destroyed || !source) {
        return;
    }

    instance.loadingVersion++;
    instance.renderVersion++;
    const currentLoadingVersion = instance.loadingVersion;
    instance.password = null;
    instance.documentTitle = null;

    clearViewer(instance);

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
        if (instance.destroyed || currentLoadingVersion !== instance.loadingVersion) {
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

    loadingTask.promise.then(async function (pdf) {
        if (instance.destroyed || currentLoadingVersion !== instance.loadingVersion) {
            if (pdf) {
                pdf.destroy();
            }

            return;
        }

        instance.documentTitle = await getDocumentTitle(pdf);

        if (instance.destroyed || currentLoadingVersion !== instance.loadingVersion) {
            if (pdf) {
                pdf.destroy();
            }

            return;
        }

        instance.loadingTask = null;
        instance.pdf = pdf;
        instance.totalPages = pdf.numPages;
        instance.pageNumber = clampPageNumber(instance, instance.pageNumber);

        await renderViewer(instance, ++instance.renderVersion, instance.pageNumber);

        if (instance.destroyed || currentLoadingVersion !== instance.loadingVersion) {
            return;
        }

        NotifyPdfInitialized(instance);
    }, function (reason) {
        if (instance.destroyed || currentLoadingVersion !== instance.loadingVersion) {
            return;
        }

        instance.loadingTask = null;
        console.error(reason);
    });
}

async function requestPasswordFromDotNet(instance, reason, attempt) {
    if (!instance || instance.destroyed || !instance.dotNetAdapter) {
        return null;
    }

    return await instance.dotNetAdapter.invokeMethodAsync('RequestPdfPassword', {
        reason: reason,
        attempt: attempt,
        source: instance.source,
    });
}

async function getDocumentTitle(pdf) {
    if (!pdf || typeof pdf.getMetadata !== "function") {
        return null;
    }

    try {
        const metadata = await pdf.getMetadata();
        const infoTitle = metadata?.info?.Title;

        if (typeof infoTitle === "string" && infoTitle.trim().length > 0) {
            return infoTitle.trim();
        }

        const metadataTitle = metadata?.metadata?.get?.("dc:title")
            ?? metadata?.metadata?.get?.("title");

        if (typeof metadataTitle === "string" && metadataTitle.trim().length > 0) {
            return metadataTitle.trim();
        }
    }
    catch {
    }

    return null;
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

async function renderViewer(instance, renderVersion, pageNumber) {
    if (!instance || instance.destroyed || !instance.pdf) {
        return;
    }

    clearViewer(instance);

    if (instance.mode === pdfViewerMode.continuous) {
        await renderContinuousViewer(instance, renderVersion, pageNumber);
    }
    else {
        renderSinglePageViewer(instance, renderVersion, pageNumber);
    }
}

function renderSinglePageViewer(instance, renderVersion, pageNumber) {
    if (!instance || instance.destroyed || !instance.pdf || renderVersion !== instance.renderVersion) {
        return;
    }

    const canvas = document.createElement("canvas");
    canvas.style.display = "inline-block";
    canvas.style.verticalAlign = "top";

    instance.singleCanvas = canvas;
    instance.element.appendChild(canvas);

    queueRenderPage(instance, pageNumber);
}

function renderPage(instance, pageNumber, renderVersion = instance.renderVersion) {
    if (!instance || instance.destroyed || !instance.pdf || !instance.singleCanvas) {
        return;
    }

    instance.pageRendering = true;

    instance.pdf.getPage(pageNumber).then(function (page) {
        if (instance.destroyed || renderVersion !== instance.renderVersion || !instance.singleCanvas) {
            instance.pageRendering = false;
            return;
        }

        const viewport = page.getViewport({ scale: instance.scale, rotation: instance.rotation });

        const canvas = instance.singleCanvas;
        const context = canvas.getContext('2d');
        canvas.height = viewport.height;
        canvas.width = viewport.width;

        const renderContext = {
            canvasContext: context,
            viewport: viewport
        };

        const renderTask = page.render(renderContext);
        instance.currentRenderTask = renderTask;

        renderTask.promise.then(function () {
            if (instance.destroyed || renderVersion !== instance.renderVersion) {
                return;
            }

            instance.pageRendering = false;
            instance.currentRenderTask = null;

            if (instance.pageNumberPending !== null) {
                const pendingPageNumber = instance.pageNumberPending;
                instance.pageNumberPending = null;
                renderPage(instance, pendingPageNumber, renderVersion);
            }
        }).catch(function (error) {
            if (error?.name !== "RenderingCancelledException") {
                console.error(error);
            }

            instance.pageRendering = false;
            instance.currentRenderTask = null;
        });
    }).catch(function (error) {
        console.error(error);
        instance.pageRendering = false;
    });
}

async function renderContinuousViewer(instance, renderVersion, pageNumber) {
    if (!instance || instance.destroyed || !instance.pdf || renderVersion !== instance.renderVersion) {
        return;
    }

    const pagesContainer = document.createElement("div");
    pagesContainer.className = "b-pdfviewer-pages";
    pagesContainer.style.display = "flex";
    pagesContainer.style.flexDirection = "column";
    pagesContainer.style.alignItems = "center";
    pagesContainer.style.width = "100%";

    instance.pagesContainer = pagesContainer;
    instance.element.appendChild(pagesContainer);

    const estimatedViewport = await getEstimatedViewport(instance);

    if (instance.destroyed || renderVersion !== instance.renderVersion) {
        return;
    }

    instance.estimatedPageWidth = estimatedViewport.width;
    instance.estimatedPageHeight = estimatedViewport.height;

    for (let currentPageNumber = 1; currentPageNumber <= instance.totalPages; currentPageNumber++) {
        const pageElement = document.createElement("div");
        pageElement.className = "b-pdfviewer-page";
        pageElement.dataset.pageNumber = String(currentPageNumber);
        pageElement.style.position = "relative";
        pageElement.style.display = "block";
        pageElement.style.boxSizing = "content-box";
        pageElement.style.width = `${estimatedViewport.width}px`;
        pageElement.style.height = `${estimatedViewport.height}px`;
        pageElement.style.margin = `0 auto ${currentPageNumber === instance.totalPages ? 0 : instance.continuousPageSpacing}px`;
        pageElement.style.background = "white";

        const pageView = {
            pageNumber: currentPageNumber,
            element: pageElement,
            canvas: null,
            rendered: false,
            rendering: false,
            renderTask: null,
            width: estimatedViewport.width,
            height: estimatedViewport.height,
        };

        instance.pageViews.set(currentPageNumber, pageView);
        pagesContainer.appendChild(pageElement);
    }

    setupContinuousObservers(instance);

    requestAnimationFrame(() => {
        if (instance.destroyed || renderVersion !== instance.renderVersion) {
            return;
        }

        scrollToPage(instance, pageNumber, false);
        renderVisiblePages(instance);
        updateCurrentPageFromScroll(instance);
    });
}

async function getEstimatedViewport(instance) {
    try {
        const page = await instance.pdf.getPage(clampPageNumber(instance, instance.pageNumber || 1));
        return page.getViewport({ scale: instance.scale, rotation: instance.rotation });
    }
    catch {
        return {
            width: instance.estimatedPageWidth || 600,
            height: instance.estimatedPageHeight || 800,
        };
    }
}

function setupContinuousObservers(instance) {
    cleanupContinuousObservers(instance);

    if (!instance || !instance.pagesContainer) {
        return;
    }

    instance.scrollContainer = findScrollContainer(instance.element);

    const rootMargin = `${Math.max(0, instance.estimatedPageHeight * instance.overscanCount)}px 0px`;
    instance.intersectionObserver = new IntersectionObserver(entries => {
        for (const entry of entries) {
            const pageNumber = Number(entry.target.dataset.pageNumber);
            const pageView = instance.pageViews.get(pageNumber);

            if (!pageView) {
                continue;
            }

            if (entry.isIntersecting) {
                renderPageView(instance, pageView, instance.renderVersion);
            }
            else if (instance.virtualizePages) {
                clearPageView(pageView);
            }
        }

        updateCurrentPageFromScroll(instance);
    }, {
        root: instance.scrollContainer,
        rootMargin: rootMargin,
        threshold: [0, 0.1, 0.5, 1],
    });

    for (const pageView of instance.pageViews.values()) {
        instance.intersectionObserver.observe(pageView.element);
    }

    instance.scrollHandler = () => {
        if (instance.scrollAnimationFrame) {
            cancelAnimationFrame(instance.scrollAnimationFrame);
        }

        instance.scrollAnimationFrame = requestAnimationFrame(() => {
            instance.scrollAnimationFrame = null;
            updateCurrentPageFromScroll(instance);
            renderVisiblePages(instance);
        });
    };

    const scrollTarget = instance.scrollContainer || window;
    scrollTarget.addEventListener("scroll", instance.scrollHandler, { passive: true });
}

function cleanupContinuousObservers(instance) {
    if (!instance) {
        return;
    }

    if (instance.intersectionObserver) {
        instance.intersectionObserver.disconnect();
        instance.intersectionObserver = null;
    }

    if (instance.scrollHandler) {
        const scrollTarget = instance.scrollContainer || window;
        scrollTarget.removeEventListener("scroll", instance.scrollHandler);
        instance.scrollHandler = null;
    }

    if (instance.scrollAnimationFrame) {
        cancelAnimationFrame(instance.scrollAnimationFrame);
        instance.scrollAnimationFrame = null;
    }

    instance.scrollContainer = null;
}

function renderVisiblePages(instance) {
    if (!instance || instance.mode !== pdfViewerMode.continuous || !instance.pageViews.size) {
        return;
    }

    const viewport = getViewportBounds(instance);
    const overscanPixels = Math.max(0, instance.estimatedPageHeight * instance.overscanCount);
    const minTop = viewport.top - overscanPixels;
    const maxBottom = viewport.bottom + overscanPixels;

    for (const pageView of instance.pageViews.values()) {
        const rect = pageView.element.getBoundingClientRect();
        const isNearViewport = rect.bottom >= minTop && rect.top <= maxBottom;

        if (isNearViewport) {
            renderPageView(instance, pageView, instance.renderVersion);
        }
        else if (instance.virtualizePages) {
            clearPageView(pageView);
        }
    }
}

async function renderPageView(instance, pageView, renderVersion) {
    if (!instance || instance.destroyed || !instance.pdf || !pageView || pageView.rendered || pageView.rendering) {
        return;
    }

    pageView.rendering = true;

    try {
        const page = await instance.pdf.getPage(pageView.pageNumber);

        if (instance.destroyed || renderVersion !== instance.renderVersion) {
            pageView.rendering = false;
            return;
        }

        const viewport = page.getViewport({ scale: instance.scale, rotation: instance.rotation });
        const canvas = document.createElement("canvas");
        const context = canvas.getContext('2d');

        canvas.width = viewport.width;
        canvas.height = viewport.height;
        canvas.style.display = "block";
        canvas.style.width = `${viewport.width}px`;
        canvas.style.height = `${viewport.height}px`;

        pageView.element.style.width = `${viewport.width}px`;
        pageView.element.style.height = `${viewport.height}px`;
        pageView.width = viewport.width;
        pageView.height = viewport.height;
        pageView.canvas = canvas;
        pageView.element.appendChild(canvas);

        const renderTask = page.render({
            canvasContext: context,
            viewport: viewport,
        });

        pageView.renderTask = renderTask;
        await renderTask.promise;

        if (instance.destroyed || renderVersion !== instance.renderVersion) {
            clearPageView(pageView);
            return;
        }

        pageView.rendered = true;
        pageView.rendering = false;
        pageView.renderTask = null;
    }
    catch (error) {
        if (error?.name !== "RenderingCancelledException") {
            console.error(error);
        }

        pageView.rendering = false;
        pageView.renderTask = null;
    }
}

function clearPageView(pageView) {
    if (!pageView) {
        return;
    }

    if (pageView.renderTask) {
        try {
            pageView.renderTask.cancel();
        }
        catch {
        }
    }

    if (pageView.canvas && pageView.canvas.parentNode) {
        pageView.canvas.parentNode.removeChild(pageView.canvas);
    }

    pageView.canvas = null;
    pageView.renderTask = null;
    pageView.rendered = false;
    pageView.rendering = false;
}

function updateCurrentPageFromScroll(instance) {
    if (!instance || instance.mode !== pdfViewerMode.continuous || !instance.pageViews.size) {
        return;
    }

    const viewport = getViewportBounds(instance);
    let bestPageNumber = instance.pageNumber;
    let bestVisibleArea = -1;
    let bestDistance = Number.MAX_SAFE_INTEGER;
    const viewportCenter = (viewport.top + viewport.bottom) / 2;

    for (const pageView of instance.pageViews.values()) {
        const rect = pageView.element.getBoundingClientRect();
        const visibleArea = Math.max(0, Math.min(rect.bottom, viewport.bottom) - Math.max(rect.top, viewport.top));
        const pageCenter = (rect.top + rect.bottom) / 2;
        const distance = Math.abs(pageCenter - viewportCenter);

        if (visibleArea > bestVisibleArea || (visibleArea === bestVisibleArea && distance < bestDistance)) {
            bestVisibleArea = visibleArea;
            bestDistance = distance;
            bestPageNumber = pageView.pageNumber;
        }
    }

    setCurrentPage(instance, bestPageNumber, false);
}

function scrollToPage(instance, pageNumber, notify) {
    if (!instance || instance.mode !== pdfViewerMode.continuous) {
        return;
    }

    const targetPageNumber = clampPageNumber(instance, pageNumber);
    const pageView = instance.pageViews.get(targetPageNumber);

    if (!pageView) {
        return;
    }

    if (instance.scrollContainer) {
        const containerRect = instance.scrollContainer.getBoundingClientRect();
        const pageRect = pageView.element.getBoundingClientRect();
        instance.scrollContainer.scrollTop += pageRect.top - containerRect.top;
    }
    else {
        const pageRect = pageView.element.getBoundingClientRect();
        window.scrollBy(0, pageRect.top);
    }

    renderPageRange(instance, targetPageNumber);
    setCurrentPage(instance, targetPageNumber, notify);
}

function renderPageRange(instance, pageNumber) {
    const firstPage = Math.max(1, pageNumber - instance.overscanCount);
    const lastPage = Math.min(instance.totalPages, pageNumber + instance.overscanCount);

    for (let currentPageNumber = firstPage; currentPageNumber <= lastPage; currentPageNumber++) {
        const pageView = instance.pageViews.get(currentPageNumber);

        if (pageView) {
            renderPageView(instance, pageView, instance.renderVersion);
        }
    }
}

function setCurrentPage(instance, pageNumber, notify) {
    const targetPageNumber = clampPageNumber(instance, pageNumber);
    const changed = instance.pageNumber !== targetPageNumber;

    instance.pageNumber = targetPageNumber;

    if (changed || notify) {
        NotifyPdfChanged(instance);
    }
}

function updateContinuousSpacing(instance) {
    if (!instance || instance.mode !== pdfViewerMode.continuous || !instance.pageViews.size) {
        return;
    }

    for (const pageView of instance.pageViews.values()) {
        pageView.element.style.marginBottom = pageView.pageNumber === instance.totalPages
            ? "0"
            : `${instance.continuousPageSpacing}px`;
    }
}

function clearViewer(instance) {
    if (!instance) {
        return;
    }

    cleanupContinuousObservers(instance);

    if (instance.currentRenderTask) {
        try {
            instance.currentRenderTask.cancel();
        }
        catch {
        }
    }

    for (const pageView of instance.pageViews?.values?.() || []) {
        clearPageView(pageView);
    }

    instance.currentRenderTask = null;
    instance.singleCanvas = null;
    instance.pagesContainer = null;
    instance.pageViews = new Map();
    instance.pageRendering = false;
    instance.pageNumberPending = null;

    if (instance.element) {
        instance.element.innerHTML = "";
    }
}

function findScrollContainer(element) {
    let parent = element?.parentElement;
    let fallback = null;

    while (parent && parent !== document.body && parent !== document.documentElement) {
        const style = getComputedStyle(parent);
        const overflowY = style.overflowY || style.overflow;

        if (overflowY === "auto" || overflowY === "scroll" || overflowY === "overlay") {
            fallback ??= parent;

            if (parent.scrollHeight > parent.clientHeight) {
                return parent;
            }
        }

        parent = parent.parentElement;
    }

    return fallback;
}

function getViewportBounds(instance) {
    if (instance.scrollContainer) {
        const rect = instance.scrollContainer.getBoundingClientRect();

        return {
            top: rect.top,
            bottom: rect.bottom,
        };
    }

    return {
        top: 0,
        bottom: window.innerHeight || document.documentElement.clientHeight,
    };
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

export async function download(element, elementId, source, fileName) {
    const instance = _instances[elementId];
    const src = source?.url || source || instance?.source;
    const resolvedFileName = resolveDownloadFileName(fileName, instance?.documentTitle, src);

    try {
        // Case 1: source is a URL
        if (typeof src === 'string' && !src.startsWith('data:')) {
            // Simply trigger a download link
            const link = document.createElement('a');
            link.href = src;
            link.download = resolvedFileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
        // Case 2: source is base64 data
        else if (typeof src === 'string' && src.startsWith('data:application/pdf;base64,')) {
            const link = document.createElement('a');
            link.href = src;
            link.download = resolvedFileName;
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

        const pageNumber = instance.pageNumber - 1;

        if (instance.mode === pdfViewerMode.continuous) {
            scrollToPage(instance, pageNumber, true);
        }
        else {
            instance.pageNumber = pageNumber;
            queueRenderPage(instance, instance.pageNumber);

            NotifyPdfChanged(instance);
        }
    }
}

export function nextPage(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        if (instance.pageNumber >= instance.totalPages) {
            return;
        }

        const pageNumber = instance.pageNumber + 1;

        if (instance.mode === pdfViewerMode.continuous) {
            scrollToPage(instance, pageNumber, true);
        }
        else {
            instance.pageNumber = pageNumber;
            queueRenderPage(instance, instance.pageNumber);

            NotifyPdfChanged(instance);
        }
    }
}

export function goToPage(element, elementId, pageNumber) {
    const instance = _instances[elementId];

    if (instance) {
        if (pageNumber < 1 || pageNumber > instance.totalPages) {
            return;
        }

        if (instance.mode === pdfViewerMode.continuous) {
            scrollToPage(instance, pageNumber, true);
        }
        else {
            instance.pageNumber = pageNumber;
            queueRenderPage(instance, instance.pageNumber);

            NotifyPdfChanged(instance);
        }
    }
}

export async function setScale(element, elementId, scale) {
    const instance = _instances[elementId];

    if (instance) {

        instance.scale = scale || 1;

        if (instance.mode === pdfViewerMode.continuous) {
            await renderViewer(instance, ++instance.renderVersion, instance.pageNumber);
        }
        else {
            queueRenderPage(instance, instance.pageNumber);
        }

        NotifyPdfChanged(instance);
    }
}

function NotifyPdfInitialized(instance) {
    if (!instance || instance.destroyed || !instance.dotNetAdapter) {
        return;
    }

    instance.dotNetAdapter.invokeMethodAsync('NotifyPdfInitialized', {
        pageNumber: instance.pageNumber,
        totalPages: instance.totalPages,
        scale: instance.scale,
        title: instance.documentTitle,
    });
}

function NotifyPdfChanged(instance) {
    if (!instance || instance.destroyed || !instance.dotNetAdapter) {
        return;
    }

    instance.dotNetAdapter.invokeMethodAsync('NotifyPdfChanged', {
        pageNumber: instance.pageNumber,
        totalPages: instance.totalPages,
        scale: instance.scale,
        title: instance.documentTitle,
    });
}

function resolveDownloadFileName(fileName, documentTitle, source) {
    const explicitFileName = normalizeDownloadFileName(fileName);

    if (explicitFileName) {
        return explicitFileName;
    }

    const metadataFileName = normalizeDownloadFileName(documentTitle);

    if (metadataFileName) {
        return metadataFileName;
    }

    if (typeof source === "string" && !source.startsWith("data:")) {
        const sourceFileName = normalizeDownloadFileName(getFileNameFromUrl(source));

        if (sourceFileName) {
            return sourceFileName;
        }
    }

    return "document.pdf";
}

function normalizeDownloadFileName(fileName) {
    if (typeof fileName !== "string") {
        return null;
    }

    let normalizedFileName = fileName.trim();

    if (!normalizedFileName) {
        return null;
    }

    const slashIndex = Math.max(normalizedFileName.lastIndexOf("/"), normalizedFileName.lastIndexOf("\\"));
    if (slashIndex >= 0) {
        normalizedFileName = normalizedFileName.substring(slashIndex + 1);
    }

    normalizedFileName = normalizedFileName.replace(/[<>:"/\\|?*\u0000-\u001F]/g, "_").trim();
    normalizedFileName = normalizedFileName.replace(/[. ]+$/, "");

    if (!normalizedFileName) {
        return null;
    }

    if (!normalizedFileName.toLowerCase().endsWith(".pdf")) {
        normalizedFileName = `${normalizedFileName}.pdf`;
    }

    return normalizedFileName;
}

function getFileNameFromUrl(url) {
    try {
        const [withoutFragment] = url.split("#");
        const [withoutQuery] = withoutFragment.split("?");
        const rawFileName = withoutQuery.split("/").pop();

        if (!rawFileName) {
            return null;
        }

        return decodeURIComponent(rawFileName);
    } catch {
        return null;
    }
}

function normalizeMode(mode) {
    if (mode === pdfViewerMode.continuous || mode === "Continuous" || mode === "continuous") {
        return pdfViewerMode.continuous;
    }

    return pdfViewerMode.singlePage;
}

function clampPageNumber(instance, pageNumber) {
    const parsedPageNumber = Number(pageNumber);

    if (!Number.isFinite(parsedPageNumber)) {
        return 1;
    }

    if (!instance.totalPages) {
        return Math.max(1, Math.trunc(parsedPageNumber));
    }

    return Math.min(instance.totalPages, Math.max(1, Math.trunc(parsedPageNumber)));
}