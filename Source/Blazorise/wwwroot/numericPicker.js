import { getRequiredElement, fromExponential } from "./utilities.js";

import './vendors/autoNumeric.js';

let _instances = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = new AutoNumeric(element, {
        decimalPlaces: options.decimals || AutoNumeric.options.decimalPlaces.two,
        decimalCharacter: options.separator || AutoNumeric.options.decimalCharacter.dot,
        decimalCharacterAlternative: options.alternativeSeparator || AutoNumeric.options.decimalCharacter.comma,

        digitGroupSeparator: options.groupSeparator || AutoNumeric.options.digitGroupSeparator.noSeparator,
        digitalGroupSpacing: options.groupSpacing || AutoNumeric.options.digitalGroupSpacing.three,

        wheelStep: options.step || 1,
        minimumValue: fromExponential(options.min || options.typeMin) || AutoNumeric.options.minimumValue.tenTrillions,
        maximumValue: fromExponential(options.max || options.typeMax) || AutoNumeric.options.maximumValue.tenTrillions,
        roundingMethod: options.roundingMethod || AutoNumeric.options.roundingMethod.halfUpSymmetric,

        currencySymbol: options.currencySymbol || AutoNumeric.options.currencySymbol.none,
        currencySymbolPlacement: options.currencySymbolPlacement || AutoNumeric.options.currencySymbolPlacement.suffix,

        selectOnFocus: options.selectAllOnFocus || AutoNumeric.options.selectOnFocus.doNotSelect,

        allowDecimalPadding: options.allowDecimalPadding || AutoNumeric.options.allowDecimalPadding.always,
        alwaysAllowDecimalCharacter: options.alwaysAllowDecimalSeparator || AutoNumeric.options.alwaysAllowDecimalCharacter.doNotAllow,

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

    if (instance && options) {
        if (options.decimals.changed) {
            instance.options.decimalPlacesRawValue(options.decimals.value || AutoNumeric.options.decimalPlaces.two);
            instance.options.decimalPlacesShownOnFocus(options.decimals.value || AutoNumeric.options.decimalPlaces.two);
            instance.options.decimalPlacesShownOnBlur(options.decimals.value || AutoNumeric.options.decimalPlaces.two);
        }

        if (options.separator.changed) {
            instance.options.decimalCharacter(options.separator.value || AutoNumeric.options.decimalCharacter.dot);
        }

        if (options.alternativeSeparator.changed) {
            instance.options.decimalCharacterAlternative(options.alternativeSeparator.value || AutoNumeric.options.decimalCharacter.comma);
        }

        if (options.groupSeparator.changed) {
            instance.options.digitGroupSeparator(options.groupSeparator.value || AutoNumeric.options.digitGroupSeparator.noSeparator);
        }

        if (options.groupSpacing.changed) {
            instance.options.digitalGroupSpacing(options.groupSpacing.value || AutoNumeric.options.digitalGroupSpacing.three);
        }

        if (options.currencySymbol.changed) {
            instance.options.currencySymbol(options.currencySymbol.value || AutoNumeric.options.currencySymbol.none);
        }

        if (options.currencySymbolPlacement.changed) {
            instance.options.currencySymbolPlacement(options.currencySymbolPlacement.value || AutoNumeric.options.currencySymbolPlacement.suffix);
        }

        if (options.roundingMethod.changed) {
            instance.options.roundingMethod(options.roundingMethod.value || AutoNumeric.options.roundingMethod.halfUpSymmetric);
        }

        if (options.min.changed) {
            instance.options.minimumValue(options.min.value || AutoNumeric.options.minimumValue.tenTrillions);
        }

        if (options.max.changed) {
            instance.options.maximumValue(options.max.value || AutoNumeric.options.maximumValue.tenTrillions);
        }

        if (options.selectAllOnFocus.changed) {
            instance.options.selectOnFocus(options.selectAllOnFocus.value || AutoNumeric.options.selectOnFocus.doNotSelect);
        }

        if (options.allowDecimalPadding.changed) {
            instance.options.allowDecimalPadding(options.allowDecimalPadding.value || AutoNumeric.options.allowDecimalPadding.always);
        }

        if (options.alwaysAllowDecimalSeparator.changed) {
            instance.options.alwaysAllowDecimalCharacter(options.alwaysAllowDecimalSeparator.value || AutoNumeric.options.alwaysAllowDecimalCharacter.doNotAllow);
        }
    }
}

export function updateValue(element, elementId, value) {
    const instance = _instances[elementId];

    if (instance) {
        instance.set(value);
    }
}