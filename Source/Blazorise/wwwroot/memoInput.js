import { getRequiredElement } from "./utilities.js?v=2.3.1.0";

const _instances = [];
const supportsNativeFieldSizing = typeof CSS !== "undefined"
    && typeof CSS.supports === "function"
    && CSS.supports("field-sizing", "content");

let behaveModulePromise;

export async function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = {
        element: element,
        elementId: elementId,
        replaceTab: options.replaceTab ?? false,
        tabSize: options.tabSize ?? 4,
        softTabs: options.softTabs ?? true,
        autoSize: false,
        behave: null,
        behaveRevision: 0,
        hasFallbackInputListener: false,
        originalHeight: "",
        originalOverflowY: ""
    };

    _instances[elementId] = instance;

    setAutoSize(instance, options.autoSize ?? false);
    await updateBehave(instance);
}

export function destroy(element, elementId) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    destroyBehave(instance);
    disableAutoSizeFallback(instance);
    delete _instances[elementId];
}

export async function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    if (options.replaceTab.changed || options.tabSize.changed || options.softTabs.changed) {
        instance.replaceTab = options.replaceTab.value;
        instance.tabSize = options.tabSize.value;
        instance.softTabs = options.softTabs.value;

        await updateBehave(instance);
    }

    if (options.autoSize.changed)
        setAutoSize(instance, options.autoSize.value);
}

export function recalculateAutoHeight(element, elementId) {
    if (supportsNativeFieldSizing)
        return;

    element = getRequiredElement(element, elementId);

    const instance = _instances[elementId];

    if (!element || !instance?.autoSize)
        return;

    autoSizeElement(element);
}

export function refreshDisplay(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    window.requestAnimationFrame(() => {
        const instance = _instances[elementId];

        if (instance?.autoSize) {
            if (!supportsNativeFieldSizing)
                autoSizeElement(element);

            return;
        }

        refreshFixedRows(element);
    });
}

async function updateBehave(instance) {
    const revision = ++instance.behaveRevision;

    destroyBehave(instance);

    if (!instance.replaceTab)
        return;

    behaveModulePromise ??= import("./vendors/Behave.js?v=2.3.1.0");

    const { Behave } = await behaveModulePromise;

    if (_instances[instance.elementId] !== instance
        || instance.behaveRevision !== revision
        || !instance.replaceTab)
        return;

    instance.behave = new Behave({
        textarea: instance.element,
        replaceTab: instance.replaceTab,
        softTabs: instance.softTabs,
        tabSize: instance.tabSize,
        autoOpen: true,
        overwrite: true,
        autoStrip: true,
        autoIndent: true,
        fence: false
    });
}

function destroyBehave(instance) {
    if (!instance.behave)
        return;

    instance.behave.destroy();
    instance.behave = null;
}

function setAutoSize(instance, autoSize) {
    instance.autoSize = autoSize;

    if (supportsNativeFieldSizing)
        return;

    if (autoSize) {
        if (!instance.hasFallbackInputListener) {
            instance.originalHeight = instance.element.style.height;
            instance.originalOverflowY = instance.element.style.overflowY;
            instance.element.addEventListener("input", onInputChanged);
            instance.hasFallbackInputListener = true;
        }

        autoSizeElement(instance.element);
    }
    else {
        disableAutoSizeFallback(instance);
    }
}

function disableAutoSizeFallback(instance) {
    if (instance.hasFallbackInputListener) {
        instance.element.removeEventListener("input", onInputChanged);
        instance.hasFallbackInputListener = false;
    }

    if (instance.element.dataset.blazoriseMemoAutoSized === "true") {
        instance.element.style.height = instance.originalHeight;
        instance.element.style.overflowY = instance.originalOverflowY;
        delete instance.element.dataset.blazoriseMemoAutoSized;
    }
}

function onInputChanged(event) {
    if (event?.target)
        autoSizeElement(event.target);
}

function autoSizeElement(textarea) {
    const computedStyle = window.getComputedStyle(textarea);
    const borderTop = Number.parseFloat(computedStyle.borderTopWidth) || 0;
    const borderBottom = Number.parseFloat(computedStyle.borderBottomWidth) || 0;

    textarea.style.height = "auto";
    textarea.style.overflowY = "hidden";

    const minimumRowsHeight = getMinimumRowsHeight(textarea, computedStyle);

    textarea.style.height = `${Math.max(textarea.scrollHeight + borderTop + borderBottom, minimumRowsHeight)}px`;
    textarea.style.overflowY = textarea.scrollHeight > textarea.clientHeight ? "auto" : "hidden";
    textarea.dataset.blazoriseMemoAutoSized = "true";
}

function refreshFixedRows(element) {
    const rows = element.rows;

    if (rows > 1) {
        element.rows = 1;
        element.offsetHeight;
        element.rows = rows;
    }
}

function getMinimumRowsHeight(textarea, computedStyle) {
    const rows = textarea.rows || Number.parseInt(textarea.getAttribute("rows") || "0", 10);

    if (!(rows > 1))
        return 0;

    let lineHeight = Number.parseFloat(computedStyle.lineHeight);

    if (!Number.isFinite(lineHeight)) {
        const fontSize = Number.parseFloat(computedStyle.fontSize) || 0;
        lineHeight = fontSize * 1.5;
    }

    const paddingTop = Number.parseFloat(computedStyle.paddingTop) || 0;
    const paddingBottom = Number.parseFloat(computedStyle.paddingBottom) || 0;
    const borderTop = Number.parseFloat(computedStyle.borderTopWidth) || 0;
    const borderBottom = Number.parseFloat(computedStyle.borderBottomWidth) || 0;

    return (lineHeight * rows) + paddingTop + paddingBottom + borderTop + borderBottom;
}