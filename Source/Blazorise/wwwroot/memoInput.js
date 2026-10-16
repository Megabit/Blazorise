import { getRequiredElement } from "./utilities.js?v=__BLAZORISE_VERSION__";

const _instances = [];
const supportsNativeFieldSizing = typeof CSS !== "undefined"
    && typeof CSS.supports === "function"
    && CSS.supports("field-sizing", "content");

const characterPairs = {
    '"': '"',
    "'": "'",
    "(": ")",
    "[": "]",
    "{": "}"
};

export async function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = {
        element: element,
        replaceTab: options.replaceTab ?? false,
        tabSize: options.tabSize ?? 4,
        softTabs: options.softTabs ?? true,
        autoSize: false,
        keyHandler: null,
        hasFallbackInputListener: false,
        originalHeight: "",
        originalOverflowY: ""
    };

    _instances[elementId] = instance;

    setAutoSize(instance, options.autoSize ?? false);
    updateEditing(instance);
}

export function destroy(element, elementId) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    destroyEditing(instance);
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

        updateEditing(instance);
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

function updateEditing(instance) {
    destroyEditing(instance);

    if (!instance.replaceTab) {
        return;
    }

    if (!instance.softTabs) {
        instance.element.style.tabSize = instance.tabSize;
    }

    instance.keyHandler = event => onEditingKey(event, instance);
    instance.element.addEventListener("keydown", instance.keyHandler);
    instance.element.addEventListener("keypress", instance.keyHandler);
    instance.element.addEventListener("blur", onEditingBlur);
}

function destroyEditing(instance) {
    if (!instance.keyHandler) {
        return;
    }

    instance.element.removeEventListener("keydown", instance.keyHandler);
    instance.element.removeEventListener("keypress", instance.keyHandler);
    instance.element.removeEventListener("blur", onEditingBlur);
    instance.keyHandler = null;
}

function onEditingBlur(event) {
    // Scripted edits do not produce the native change event used by deferred binding.
    event.currentTarget.dispatchEvent(new Event("change", { bubbles: true }));
}

function onEditingKey(event, instance) {
    const element = instance.element;

    if (event.defaultPrevented || event.isComposing
        || event.ctrlKey || event.metaKey || event.altKey
        || element.readOnly || element.disabled) {
        return;
    }

    const start = element.selectionStart;
    const end = element.selectionEnd;
    const value = element.value;

    if (event.type === "keydown") {
        if (event.key === "Tab") {
            event.preventDefault();
            replaceTab(instance, event.shiftKey);
        }
        else if (event.key === "Enter") {
            event.preventDefault();
            insertNewLine(instance);
        }
        else if (event.key === "Backspace" && start === end && start > 0
            && start < value.length && characterPairs[value[start - 1]] === value[start]) {
            event.preventDefault();
            replaceText(element, start - 1, start + 1, "");
        }

        return;
    }

    if (start === end && Object.values(characterPairs).includes(event.key) && value[start] === event.key) {
        event.preventDefault();
        element.setSelectionRange(start + 1, start + 1);
        return;
    }

    const closingCharacter = characterPairs[event.key];

    if (closingCharacter) {
        event.preventDefault();
        replaceText(element, start, end, event.key + closingCharacter, start + 1);
    }
}

function getTabString(instance) {
    return instance.softTabs ? " ".repeat(Math.max(0, instance.tabSize)) : "\t";
}

function replaceTab(instance, unindent) {
    const element = instance.element;
    const start = element.selectionStart;
    const end = element.selectionEnd;
    const value = element.value;
    const tab = getTabString(instance);

    if (!tab.length) {
        return;
    }

    if (start !== end) {
        const lineStart = value.slice(0, start).lastIndexOf("\n") + 1;
        const replacement = value.slice(lineStart, end).split("\n")
            .map(line => unindent ? (line.startsWith(tab) ? line.slice(tab.length) : line) : tab + line)
            .join("\n");

        replaceText(element, lineStart, end, replacement, lineStart, lineStart + replacement.length);
        return;
    }

    if (!unindent) {
        replaceText(element, start, end, tab);
        return;
    }

    if (start >= tab.length && value.slice(start - tab.length, start) === tab) {
        replaceText(element, start - tab.length, start, "");
    }
}

function insertNewLine(instance) {
    const element = instance.element;
    const start = element.selectionStart;
    const end = element.selectionEnd;
    const value = element.value;
    const tab = getTabString(instance);
    const depth = getIndentationDepth(value.slice(0, start));
    const indentation = tab.repeat(depth);
    let replacement = "\n" + indentation;

    if (depth > 0 && end < value.length && characterPairs[value[start - 1]] === value[end]) {
        replacement += "\n" + tab.repeat(depth - 1);
    }

    replaceText(element, start, end, replacement, start + 1 + indentation.length);
}

function getIndentationDepth(text) {
    let depth = 0;
    let quote = "";

    for (let index = 0; index < text.length; index++) {
        const character = text[index];

        if (quote) {
            if (character === "\\") {
                index++;
            }
            else if (character === quote) {
                quote = "";
            }
        }
        else if (character === '"' || character === "'") {
            quote = character;
        }
        else if (character === "[" || character === "{") {
            depth++;
        }
        else if (character === "]" || character === "}") {
            depth--;
        }
    }

    return Math.max(0, depth);
}

function replaceText(element, start, end, text, selectionStart = start + text.length, selectionEnd = selectionStart) {
    const previousValue = element.value;
    const value = previousValue.slice(0, start) + text + previousValue.slice(end);

    if (value !== previousValue) {
        element.value = value;
    }

    element.setSelectionRange(selectionStart, selectionEnd);

    if (value !== previousValue) {
        element.dispatchEvent(new Event("input", { bubbles: true }));
    }
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