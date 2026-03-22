import { Behave } from "./vendors/Behave.js?v=2.0.3.0";
import { getRequiredElement } from "./utilities.js?v=2.0.3.0";

const _instances = [];

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const replaceTab = options.replaceTab || false;
    const tabSize = options.tabSize || 4;
    const softTabs = options.tabSize || true;

    let behave = replaceTab ? new Behave({
        textarea: element,
        replaceTab: replaceTab,
        softTabs: softTabs,
        tabSize: tabSize,
        autoOpen: true,
        overwrite: true,
        autoStrip: true,
        autoIndent: true,
        fence: false
    }) : null;

    if (options.autoSize) {
        element.oninput = onInputChanged;

        // fire oninput immediatelly to trigger autosize in case the text is long
        if ("createEvent" in document) {
            let event = document.createEvent("HTMLEvents");
            event.initEvent("input", false, true);
            element.dispatchEvent(event);
        }
        else {
            element.fireEvent("oninput");
        }
    }

    _instances[elementId] = {
        element: element,
        elementId: elementId,
        replaceTab: replaceTab,
        tabSize: tabSize,
        softTabs: softTabs,
        autoSize: options.autoSize || false,
        behave: behave
    };
}

export function destroy(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.behave) {
        instance.behave.destroy();
        instance.behave = null;
    }

    delete _instances[elementId];
}

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (instance) {
        if (options.replaceTab.changed || options.tabSize.changed || options.softTabs.changed) {
            instance.replaceTab = options.replaceTab.value;
            instance.tabSize = options.tabSize.value;
            instance.softTabs = options.softTabs.value;

            if (instance.behave) {
                instance.behave.destroy();
                instance.behave = null;
            }

            if (instance.replaceTab) {
                instance.behave = new Behave({
                    textarea: element,
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
        }

        if (options.autoSize.changed) {
            instance.autoSize = options.autoSize.value;
            element.oninput = options.autoSize.value
                ? onInputChanged
                : function() { };
        }
    };
}

function onInputChanged(e) {
    if (e && e.target) {
        const textarea = e.target;
        const computedStyle = window.getComputedStyle(textarea);

        const borderTop = parseFloat(computedStyle.borderTopWidth) || 0;
        const borderBottom = parseFloat(computedStyle.borderBottomWidth) || 0;

        textarea.style.height = 'auto';
        textarea.style.overflowY = 'hidden';

        const totalExtraSpace = borderTop + borderBottom;
        const minimumRowsHeight = getMinimumRowsHeight(textarea, computedStyle);

        textarea.style.height = `${Math.max(textarea.scrollHeight + totalExtraSpace, minimumRowsHeight)}px`;
        textarea.dataset.blazoriseMemoAutoSized = 'true';
    }
}

export function recalculateAutoHeight(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    // fire input to trigger autosize i n case the text is long
    if ("createEvent" in document) {
        let event = document.createEvent("HTMLEvents");
        event.initEvent("input", false, true);
        element.dispatchEvent(event);
    }
    else {
        element.fireEvent("oninput");
    }
}

export function refreshDisplay(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    window.requestAnimationFrame(() => {
        const instance = _instances[elementId];

        if (instance?.autoSize) {
            recalculateAutoHeight(element, elementId);
            return;
        }

        const rows = element.rows;

        if (element.dataset.blazoriseMemoAutoSized === 'true') {
            element.style.height = '';
            element.style.overflowY = '';
            delete element.dataset.blazoriseMemoAutoSized;
        }

        if (rows > 1) {
            element.rows = 1;
            element.offsetHeight;
            element.rows = rows;
        }
    });
}

function getMinimumRowsHeight(textarea, computedStyle) {
    const rows = textarea.rows || Number.parseInt(textarea.getAttribute('rows') || '0', 10);

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