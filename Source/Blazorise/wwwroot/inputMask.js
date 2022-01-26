import Inputmask from "./vendors/inputmask.js";
import { getRequiredElement } from "./utilities.js";

let _instances = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const maskOptions = options.mask ? { mask: options.mask } : {};
    const regexOptions = options.mask ? { regex: options.regex } : {};
    const aliasOptions = options.alias ? { alias: options.alias, inputFormat: options.inputFormat, outputFormat: options.outputFormat } : {};
    const otherOptions = {
        placeholder: options.placeholder || "_",
        showMaskOnFocus: options.showMaskOnFocus,
        showMaskOnHover: options.showMaskOnHover,
        numericInput: options.numericInput || false,
        rightAlign: options.rightAlign || false,
        radixPoint: options.decimalSeparator || "",
        groupSeparator: options.groupSeparator || "",
        nullable: options.nullable || false,
        positionCaretOnClick: options.positionCaretOnClick || "lvp",
        clearMaskOnLostFocus: options.clearMaskOnLostFocus || true,
        clearIncomplete: options.clearIncomplete || false,
        autoUnmask: options.autoUnmask || false,
        oncomplete: function (e) {
            dotnetAdapter.invokeMethodAsync('NotifyCompleted', e.target.value);
        },
        onincomplete: function (e) {
            dotnetAdapter.invokeMethodAsync('NotifyIncompleted', e.target.value);
        },
        oncleared: function () {
            dotnetAdapter.invokeMethodAsync('NotifyCleared');
        }
    };

    const finalOptions = options.alias
        ? Object.assign({}, aliasOptions, otherOptions)
        : Object.assign({}, maskOptions, regexOptions, otherOptions);

    var inputMask = new Inputmask(finalOptions);

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

export function extendAliases(element, elementId, aliasOptions) {
    const instance = _instances[elementId];

    if (instance && instance.inputMask) {
        instance.inputMask.extendAliases(aliasOptions);
    }
}