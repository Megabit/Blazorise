import { NumericMaskValidator } from "./validators/NumericMaskValidator.js";
import { getRequiredElement } from "./utilities.js";

let _instances = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = new NumericMaskValidator(dotnetAdapter, element, elementId, options);

    _instances[elementId] = instance;

    element.addEventListener("keypress", (e) => {
        keyPress(_instances[elementId], e);
    });

    element.addEventListener("paste", (e) => {
        paste(_instances[elementId], e);
    });

    element.addEventListener("focus", (e) => {
        selectAll(_instances[elementId], e);
    });

    if (instance.decimals && instance.decimals !== 2) {
        instance.truncate();
    }
}

export function destroy(element, elementId) {
    var instances = _instances || {};
    delete instances[elementId];
}

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (instance) {
        instance.update(options);
    }
}

function keyPress(validator, e) {
    var currentValue = String.fromCharCode(e.which);

    return e.which === 13 // still need to allow ENTER key so that we don't preventDefault on form submit
        || validator.isValid(currentValue)
        || e.preventDefault();
}

function paste(validator, e) {
    return validator.isValid(e.clipboardData.getData("text/plain")) || e.preventDefault();
}

function selectAll(validator, e) {
    if (validator.selectAllOnFocus && validator.element) {
        const element = validator.element;

        if (element.value && element.value.length > 0) {
            element.setSelectionRange(0, element.value.length);
        }
    }
}