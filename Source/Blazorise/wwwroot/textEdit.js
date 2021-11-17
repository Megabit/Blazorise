﻿import { NumericMaskValidator } from "./validators/NumericMaskValidator.js";
import { DateTimeMaskValidator } from "./validators/DateTimeMaskValidator.js";
import { RegExMaskValidator } from "./validators/RegExMaskValidator.js";
import { NoValidator } from "./validators/NoValidator.js";

let _instances = [];

export function initialize(element, elementId, maskType, editMask) {
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