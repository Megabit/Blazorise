import * as utilities from "./utilities.js?v=2.3.3.0";

export function createPickerInput() {
    const pickers = new Map();

    function initialize(dotnetAdapter, element, elementId, options) {
        element = utilities.getRequiredElement(element, elementId);

        if (!element) {
            return;
        }

        pickers.set(elementId, element);
        applyOptions(element, options);
    }

    function destroy(element, elementId) {
        pickers.delete(elementId);
    }

    function activate() {
    }

    function updateValue(element, elementId, value) {
        updateTextValue(element, elementId, value);
    }

    function updateTextValue(element, elementId, value) {
        element = pickers.get(elementId) || utilities.getRequiredElement(element, elementId);

        if (element) {
            element.value = value || "";
        }
    }

    function updateOptions(element, elementId, options) {
        element = pickers.get(elementId) || utilities.getRequiredElement(element, elementId);

        if (element) {
            applyChangedOptions(element, options);
        }
    }

    function open() {
    }

    function close() {
    }

    function toggle() {
    }

    function updateLocalization() {
    }

    function focus(element, elementId, scrollToElement) {
        element = pickers.get(elementId) || utilities.getRequiredElement(element, elementId);

        if (element) {
            utilities.focus(element, null, scrollToElement);
        }
    }

    function select(element, elementId, focusElement) {
        element = pickers.get(elementId) || utilities.getRequiredElement(element, elementId);

        if (!element) {
            return;
        }

        if (focusElement) {
            element.focus();
        }

        element.select();
    }

    function applyOptions(element, options) {
        if (!options) {
            return;
        }

        element.disabled = options.disabled || false;
        element.readOnly = options.readOnly || false;
        element.placeholder = options.placeholder || "";
    }

    function applyChangedOptions(element, options) {
        if (!options) {
            return;
        }

        if (options.disabled && options.disabled.changed) {
            element.disabled = options.disabled.value;
        }

        if (options.readOnly && options.readOnly.changed) {
            element.readOnly = options.readOnly.value;
        }

        if (options.placeholder && options.placeholder.changed) {
            element.placeholder = options.placeholder.value || "";
        }
    }

    return { initialize, destroy, activate, updateValue, updateTextValue, updateOptions, open, close, toggle, updateLocalization, focus, select };
}