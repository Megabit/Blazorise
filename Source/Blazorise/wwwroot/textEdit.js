import { NumericMaskValidator } from "./validators/NumericMaskValidator.js?v=1.6.1.0";
import { DateTimeMaskValidator } from "./validators/DateTimeMaskValidator.js?v=1.6.1.0";
import { RegExMaskValidator } from "./validators/RegExMaskValidator.js?v=1.6.1.0";
import { NoValidator } from "./validators/NoValidator.js?v=1.6.1.0";
import { getRequiredElement } from "./utilities.js?v=1.6.1.0";

let _instances = [];

export function initialize(element, elementId, maskType, editMask) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    var instances = _instances = _instances || {};

    if (maskType === "numeric") {
        instances[elementId] = new NumericMaskValidator(null, element, elementId);
    }
    else if (maskType === "datetime") {
        instances[elementId] = new DateTimeMaskValidator(element, elementId);
    }
    else if (maskType === "regex") {
        instances[elementId] = new RegExMaskValidator(element, elementId, editMask);
    }
    else {
        instances[elementId] = new NoValidator();
    }

    element.addEventListener("keypress", (e) => {
        keyPress(instances[elementId], e);
    });

    element.addEventListener("paste", (e) => {
        paste(instances[elementId], e);
    });
}

export function destroy(element, elementId) {
    var instances = _instances || {};
    delete instances[elementId];
}

function keyPress(validator, e) {
    var currentValue = String.fromCharCode(e.which);

    return validator.isValid(currentValue) || e.preventDefault();
}

function paste(validator, e) {
    return validator.isValid(e.clipboardData.getData("text/plain")) || e.preventDefault();
}