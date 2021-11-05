import Inputmask from "./vendors/inputmask.js";
import { getRequiredElement } from "./utilities.js";

let _instances = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    var inputMask = new Inputmask({
        mask: options.mask,
        regex: options.regex,
        placeholder: options.placeholder || "_",
        showMaskOnFocus: options.showMaskOnFocus,
        showMaskOnHover: options.showMaskOnHover,
        numericInput: options.numericInput || false,
        rightAlign: options.rightAlign || false,
        radixPoint: options.decimalSeparator || "",
        groupSeparator: options.groupSeparator || "",
        nullable: options.nullable || false,
        positionCaretOnClick: options.positionCaretOnClick || "lvp",
        oncomplete: function (e) {
            dotnetAdapter.invokeMethodAsync('NotifyCompleted', e.target.value);
        },
        onincomplete: function (e) {
            dotnetAdapter.invokeMethodAsync('NotifyIncompleted', e.target.value);
        },
    });

    inputMask.mask(element);

    _instances[elementId] = {
        dotnetAdapter: dotnetAdapter,
        element: element,
        elementId: elementId,
        inputMask: inputMask
    };
}

export function destroy(element, elementId) {
    var instances = _instances || {};
    delete instances[elementId];
}