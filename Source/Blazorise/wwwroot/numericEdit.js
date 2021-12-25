import { getRequiredElement, fromExponential } from "./utilities.js";

import './vendors/autoNumeric.js';

let _instances = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = new AutoNumeric(element, {
        digitGroupSeparator: options.groupSeparator || "",
        decimalCharacter: options.separator || ".",
        decimalCharacterAlternative: options.separator || ",",
        decimalPlaces: options.decimals || 2,
        wheelStep: options.step || 1,
        minimumValue: fromExponential(options.min || options.typeMin) || '-10000000000000',
        maximumValue: fromExponential(options.max || options.typeMax) || '10000000000000',

        currencySymbol: options.currencySymbol || "",
        currencySymbolPlacement: AutoNumeric.options.currencySymbolPlacement.suffix,
        roundingMethod: AutoNumeric.options.roundingMethod.halfUpSymmetric,
        onInvalidPaste: 'ignore',
        selectOnFocus: options.selectAllOnFocus || false
    });

    _instances[elementId] = instance;
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

export function updateValue(element, elementId, value) {
    const instance = _instances[elementId];

    if (instance) {
        instance.set(value);
    }
}