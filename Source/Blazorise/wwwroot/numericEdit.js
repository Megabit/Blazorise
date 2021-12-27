import { getRequiredElement, fromExponential } from "./utilities.js";

import './vendors/autoNumeric.js';

let _instances = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = new AutoNumeric(element, {
        digitGroupSeparator: options.groupSeparator || AutoNumeric.options.digitGroupSeparator.noSeparator,
        digitalGroupSpacing: options.groupSpacing || AutoNumeric.options.digitalGroupSpacing.three,
        decimalCharacter: options.separator || AutoNumeric.options.decimalCharacter.dot,
        decimalCharacterAlternative: options.alternativeSeparator || AutoNumeric.options.decimalCharacter.comma,
        decimalPlaces: options.decimals || AutoNumeric.options.decimalPlaces.two,
        wheelStep: options.step || 1,
        minimumValue: fromExponential(options.min || options.typeMin) || AutoNumeric.options.minimumValue.tenTrillions,
        maximumValue: fromExponential(options.max || options.typeMax) || AutoNumeric.options.maximumValue.tenTrillions,
        roundingMethod: AutoNumeric.options.roundingMethod.halfUpSymmetric,

        currencySymbol: options.currencySymbol || AutoNumeric.options.currencySymbol.none,
        currencySymbolPlacement: options.currencySymbolPlacement || AutoNumeric.options.currencySymbolPlacement.suffix,

        selectOnFocus: options.selectAllOnFocus || AutoNumeric.options.selectOnFocus.doNotSelect,

        onInvalidPaste: 'ignore',
    });

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        instance.remove();
    }

    delete _instances[elementId];
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