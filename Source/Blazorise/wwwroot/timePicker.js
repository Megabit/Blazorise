import * as utilities from "./utilities.js?v=2.2.2.0";

const pickers = new Map();

export function initialize(dotnetAdapter, element, elementId, options) {
    element = utilities.getRequiredElement(element, elementId);

    if (!element)
        return;

    pickers.set(elementId, element);
    applyOptions(element, options);
}

export function destroy(element, elementId) {
    pickers.delete(elementId);
}

export function activate() {
}

export function updateValue(element, elementId, value) {
    updateTextValue(element, elementId, value);
}

export function updateTextValue(element, elementId, value) {
    element = pickers.get(elementId) || utilities.getRequiredElement(element, elementId);

    if (element)
        element.value = value || "";
}

export function updateOptions(element, elementId, options) {
    element = pickers.get(elementId) || utilities.getRequiredElement(element, elementId);

    if (element)
        applyChangedOptions(element, options);
}

export function open() {
}

export function close() {
}

export function toggle() {
}

export function updateLocalization() {
}

export function focus(element, elementId, scrollToElement) {
    element = pickers.get(elementId) || utilities.getRequiredElement(element, elementId);

    if (element)
        utilities.focus(element, null, scrollToElement);
}

export function select(element, elementId, focusElement) {
    element = pickers.get(elementId) || utilities.getRequiredElement(element, elementId);

    if (!element)
        return;

    if (focusElement)
        element.focus();

    element.select();
}

function applyOptions(element, options) {
    if (!options)
        return;

    element.disabled = options.disabled || false;
    element.readOnly = options.readOnly || false;
    element.placeholder = options.placeholder || "";
}

function applyChangedOptions(element, options) {
    if (!options)
        return;

    if (options.disabled && options.disabled.changed)
        element.disabled = options.disabled.value;

    if (options.readOnly && options.readOnly.changed)
        element.readOnly = options.readOnly.value;

    if (options.placeholder && options.placeholder.changed)
        element.placeholder = options.placeholder.value || "";
}