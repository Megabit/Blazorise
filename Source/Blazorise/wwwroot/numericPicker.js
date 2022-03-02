import { getRequiredElement, fromExponential, firstNonNull } from "./utilities.js";

import './vendors/autoNumeric.js';

let _instances = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = new AutoNumeric(element, {
        decimalPlaces: firstNonNull(options.decimals, AutoNumeric.options.decimalPlaces.two),
        decimalPlacesRawValue: firstNonNull(options.decimals, AutoNumeric.options.decimalPlaces.two),
        decimalPlacesShownOnBlur: firstNonNull(options.decimals, AutoNumeric.options.decimalPlaces.two),
        decimalPlacesShownOnFocus: firstNonNull(options.decimals, AutoNumeric.options.decimalPlaces.two),
        decimalCharacter: firstNonNull(options.decimalSeparator, AutoNumeric.options.decimalCharacter.dot),
        decimalCharacterAlternative: firstNonNull(options.alternativeDecimalSeparator, AutoNumeric.options.decimalCharacter.comma),

        digitGroupSeparator: firstNonNull(options.groupSeparator, AutoNumeric.options.digitGroupSeparator.noSeparator),
        digitalGroupSpacing: firstNonNull(options.groupSpacing, AutoNumeric.options.digitalGroupSpacing.three),

        wheelStep: firstNonNull(options.step, 1),
        minimumValue: firstNonNull(fromExponential(firstNonNull(options.min, options.typeMin)), AutoNumeric.options.minimumValue.tenTrillions),
        maximumValue: firstNonNull(fromExponential(firstNonNull(options.max, options.typeMax)), AutoNumeric.options.maximumValue.tenTrillions),
        overrideMinMaxLimits: firstNonNull(options.minMaxLimitsOverride, AutoNumeric.options.overrideMinMaxLimits.doNotOverride),
        roundingMethod: firstNonNull(options.roundingMethod, AutoNumeric.options.roundingMethod.halfUpSymmetric),

        currencySymbol: firstNonNull(options.currencySymbol, AutoNumeric.options.currencySymbol.none),
        currencySymbolPlacement: firstNonNull(options.currencySymbolPlacement, AutoNumeric.options.currencySymbolPlacement.suffix),

        selectOnFocus: firstNonNull(options.selectAllOnFocus, AutoNumeric.options.selectOnFocus.doNotSelect),

        allowDecimalPadding: firstNonNull(options.allowDecimalPadding, AutoNumeric.options.allowDecimalPadding.always),
        alwaysAllowDecimalCharacter: firstNonNull(options.alwaysAllowDecimalSeparator, AutoNumeric.options.alwaysAllowDecimalCharacter.doNotAllow),

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
            //instance.options.decimalPlaces(options.decimals.value || AutoNumeric.options.decimalPlaces.two);
            instance.options.decimalPlacesRawValue(firstNonNull(options.decimals.value, AutoNumeric.options.decimalPlaces.two));
            instance.options.decimalPlacesShownOnFocus(firstNonNull(options.decimals.value, AutoNumeric.options.decimalPlaces.two));
            instance.options.decimalPlacesShownOnBlur(firstNonNull(options.decimals.value, AutoNumeric.options.decimalPlaces.two));
        }

        if (options.decimalSeparator.changed) {
            instance.options.decimalCharacter(firstNonNull(options.decimalSeparator.value, AutoNumeric.options.decimalCharacter.dot));
        }

        if (options.alternativeDecimalSeparator.changed) {
            instance.options.decimalCharacterAlternative(firstNonNull(options.alternativeDecimalSeparator.value, AutoNumeric.options.decimalCharacter.comma));
        }

        if (options.groupSeparator.changed) {
            instance.options.digitGroupSeparator(firstNonNull(options.groupSeparator.value, AutoNumeric.options.digitGroupSeparator.noSeparator));
        }

        if (options.groupSpacing.changed) {
            instance.options.digitalGroupSpacing(firstNonNull(options.groupSpacing.value, AutoNumeric.options.digitalGroupSpacing.three));
        }

        if (options.currencySymbol.changed) {
            instance.options.currencySymbol(firstNonNull(options.currencySymbol.value, AutoNumeric.options.currencySymbol.none));
        }

        if (options.currencySymbolPlacement.changed) {
            instance.options.currencySymbolPlacement(firstNonNull(options.currencySymbolPlacement.value, AutoNumeric.options.currencySymbolPlacement.suffix));
        }

        if (options.roundingMethod.changed) {
            instance.options.roundingMethod(firstNonNull(options.roundingMethod.value, AutoNumeric.options.roundingMethod.halfUpSymmetric));
        }

        if (options.min.changed) {
            instance.options.minimumValue(firstNonNull(options.min.value, AutoNumeric.options.minimumValue.tenTrillions));
        }

        if (options.max.changed) {
            instance.options.maximumValue(firstNonNull(options.max.value, AutoNumeric.options.maximumValue.tenTrillions));
        }

        if (options.minMaxLimitsOverride.changed) {
            instance.options.overrideMinMaxLimits(firstNonNull(options.minMaxLimitsOverride.value, AutoNumeric.options.overrideMinMaxLimits.doNotOverride));
        }

        if (options.selectAllOnFocus.changed) {
            instance.options.selectOnFocus(firstNonNull(options.selectAllOnFocus.value, AutoNumeric.options.selectOnFocus.doNotSelect));
        }

        if (options.allowDecimalPadding.changed) {
            instance.options.allowDecimalPadding(firstNonNull(options.allowDecimalPadding.value, AutoNumeric.options.allowDecimalPadding.always));
        }

        if (options.alwaysAllowDecimalSeparator.changed) {
            instance.options.alwaysAllowDecimalCharacter(firstNonNull(options.alwaysAllowDecimalSeparator.value, AutoNumeric.options.alwaysAllowDecimalCharacter.doNotAllow));
        }
    }
}

export function updateValue(element, elementId, value) {
    const instance = _instances[elementId];

    if (instance) {
        instance.set(value);
    }
}