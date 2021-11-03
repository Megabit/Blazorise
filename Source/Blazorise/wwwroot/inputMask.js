import Inputmask from "./vendors/inputmask.js";
import { getRequiredElement } from "./utilities.js";

let _instances = [];

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    var instances = _instances = _instances || {};

    var inputMask = new Inputmask({
        mask: options.mask,
        regex: options.regex,
        placeholder: options.placeholder || "_",
        nullable: false
    });

    inputMask.mask(element);

    _instances[elementId] = {
        element: element,
        elementId: elementId,
        inputMask: inputMask
    };
}

export function destroy(element, elementId) {
    var instances = _instances || {};
    delete instances[elementId];
}