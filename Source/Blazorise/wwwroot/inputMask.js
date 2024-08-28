import Inputmask from "./vendors/inputmask.js?v=1.6.1.0";
import { getRequiredElement } from "./utilities.js?v=1.6.1.0";

let _instances = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const maskOptions = options.mask ? { mask: options.mask } : {};
    const regexOptions = options.mask ? { regex: options.regex } : {};
    const aliasOptions = options.alias ? { alias: options.alias, inputFormat: options.inputFormat, outputFormat: options.outputFormat } : {};
    const eventOptions = dotnetAdapter
        ? {
            oncomplete: function (e) {
                dotnetAdapter.invokeMethodAsync('NotifyCompleted', e.target.value);
            },
            onincomplete: function (e) {
                dotnetAdapter.invokeMethodAsync('NotifyIncompleted', e.target.value);
            },
            oncleared: function () {
                dotnetAdapter.invokeMethodAsync('NotifyCleared');
            }
        } : {};

    const otherOptions = {
        placeholder: options.maskPlaceholder || "_",
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
        autoUnmask: options.autoUnmask || false
    };

    const finalOptions = options.alias
        ? Object.assign({}, aliasOptions, eventOptions, otherOptions)
        : Object.assign({}, maskOptions, regexOptions, eventOptions, otherOptions);

    let inputMask = new Inputmask(finalOptions);

    inputMask.mask(element);

    _instances[elementId] = {
        dotnetAdapter: dotnetAdapter,
        element: element,
        elementId: elementId,
        inputMask: inputMask
    };

    return inputMask;
}

export function destroy(element, elementId) {
    let instances = _instances || {};
    delete instances[elementId];
}

export function extendAliases(element, elementId, aliasOptions) {
    const instance = _instances[elementId];

    if (instance && instance.inputMask) {
        instance.inputMask.extendAliases(aliasOptions);
    }
}