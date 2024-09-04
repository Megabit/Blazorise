import { getRequiredElement, fromExponential, firstNonNull } from "./utilities.js?v=1.6.1.0";

import './vendors/autoNumeric.js?v=1.6.1.0';

let _instances = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = new AutoNumeric(element, options.value, {
        decimalPlaces: firstNonNull(options.decimals, AutoNumeric.options.decimalPlaces.two),
        decimalPlacesRawValue: firstNonNull(options.decimals, AutoNumeric.options.decimalPlaces.two),
        decimalPlacesShownOnBlur: firstNonNull(options.decimals, AutoNumeric.options.decimalPlaces.two),
        decimalPlacesShownOnFocus: firstNonNull(options.decimals, AutoNumeric.options.decimalPlaces.two),
        decimalCharacter: firstNonNull(options.decimalSeparator, AutoNumeric.options.decimalCharacter.dot),
        decimalCharacterAlternative: firstNonNull(options.alternativeDecimalSeparator, AutoNumeric.options.decimalCharacter.comma),

        digitGroupSeparator: firstNonNull(options.groupSeparator, AutoNumeric.options.digitGroupSeparator.noSeparator),
        digitalGroupSpacing: firstNonNull(options.groupSpacing, AutoNumeric.options.digitalGroupSpacing.three),

        modifyValueOnWheel: firstNonNull(options.modifyValueOnWheel, AutoNumeric.options.modifyValueOnWheel.doNothing),
        wheelOn: firstNonNull(options.wheelOn, AutoNumeric.options.wheelOn.focus),
        wheelStep: firstNonNull(options.step, 1),
        minimumValue: firstNonNull(fromExponential(firstNonNull(options.min, options.typeMin)), AutoNumeric.options.minimumValue.tenTrillions),
        maximumValue: firstNonNull(fromExponential(firstNonNull(options.max, options.typeMax)), AutoNumeric.options.maximumValue.tenTrillions),
        overrideMinMaxLimits: firstNonNull(options.minMaxLimitsOverride, AutoNumeric.options.overrideMinMaxLimits.doNotOverride),
        roundingMethod: firstNonNull(options.roundingMethod, AutoNumeric.options.roundingMethod.halfUpSymmetric),

        currencySymbol: firstNonNull(options.currencySymbol, AutoNumeric.options.currencySymbol.none),
        currencySymbolPlacement: firstNonNull(options.currencySymbolPlacement, AutoNumeric.options.currencySymbolPlacement.suffix),

        selectOnFocus: firstNonNull(options.selectAllOnFocus, AutoNumeric.options.selectOnFocus.doNotSelect),
        caretPositionOnFocus: AutoNumeric.options.caretPositionOnFocus.doNoForceCaretPosition,

        allowDecimalPadding: firstNonNull(options.allowDecimalPadding, AutoNumeric.options.allowDecimalPadding.always),
        alwaysAllowDecimalCharacter: firstNonNull(options.alwaysAllowDecimalSeparator, AutoNumeric.options.alwaysAllowDecimalCharacter.doNotAllow),

        onInvalidPaste: 'ignore',
        emptyInputBehavior: AutoNumeric.options.emptyInputBehavior.null
    });

    element.addEventListener('autoNumeric:rawValueModified', e => {
        if (typeof e.detail.newRawValue !== "undefined") {
            dotnetAdapter.invokeMethodAsync('SetValue', e.detail.newRawValue);
        }
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
        const newOptions = {};

        if (options.decimals.changed) {
            //newOptions.decimalPlaces = options.decimals.value || AutoNumeric.options.decimalPlaces.two;
            newOptions.decimalPlacesRawValue = firstNonNull(options.decimals.value, AutoNumeric.options.decimalPlaces.two);
            newOptions.decimalPlacesShownOnFocus = firstNonNull(options.decimals.value, AutoNumeric.options.decimalPlaces.two);
            newOptions.decimalPlacesShownOnBlur = firstNonNull(options.decimals.value, AutoNumeric.options.decimalPlaces.two);
        }

        if (options.decimalSeparator.changed) {
            newOptions.decimalCharacter = firstNonNull(options.decimalSeparator.value, AutoNumeric.options.decimalCharacter.dot);
        }

        if (options.alternativeDecimalSeparator.changed) {
            newOptions.decimalCharacterAlternative = firstNonNull(options.alternativeDecimalSeparator.value, AutoNumeric.options.decimalCharacter.comma);
        }

        if (options.groupSeparator.changed) {
            newOptions.digitGroupSeparator = firstNonNull(options.groupSeparator.value, AutoNumeric.options.digitGroupSeparator.noSeparator);
        }

        if (options.groupSpacing.changed) {
            newOptions.digitalGroupSpacing = firstNonNull(options.groupSpacing.value, AutoNumeric.options.digitalGroupSpacing.three);
        }

        if (options.currencySymbol.changed) {
            newOptions.currencySymbol = firstNonNull(options.currencySymbol.value, AutoNumeric.options.currencySymbol.none);
        }

        if (options.currencySymbolPlacement.changed) {
            newOptions.currencySymbolPlacement = firstNonNull(options.currencySymbolPlacement.value, AutoNumeric.options.currencySymbolPlacement.suffix);
        }

        if (options.roundingMethod.changed) {
            newOptions.roundingMethod = firstNonNull(options.roundingMethod.value, AutoNumeric.options.roundingMethod.halfUpSymmetric);
        }

        if (options.min.changed) {
            newOptions.minimumValue = fromExponential(firstNonNull(options.min.value, AutoNumeric.options.minimumValue.tenTrillions));
        }
        if (options.max.changed) {
            newOptions.maximumValue = fromExponential(firstNonNull(options.max.value, AutoNumeric.options.maximumValue.tenTrillions));
        }

        if (options.minMaxLimitsOverride.changed) {
            newOptions.overrideMinMaxLimits = firstNonNull(options.minMaxLimitsOverride.value, AutoNumeric.options.overrideMinMaxLimits.doNotOverride);
        }

        if (options.selectAllOnFocus.changed) {
            newOptions.selectOnFocus = firstNonNull(options.selectAllOnFocus.value, AutoNumeric.options.selectOnFocus.doNotSelect);
        }

        if (options.allowDecimalPadding.changed) {
            newOptions.allowDecimalPadding = firstNonNull(options.allowDecimalPadding.value, AutoNumeric.options.allowDecimalPadding.always);
        }

        if (options.alwaysAllowDecimalSeparator.changed) {
            newOptions.alwaysAllowDecimalCharacter = firstNonNull(options.alwaysAllowDecimalSeparator.value, AutoNumeric.options.alwaysAllowDecimalCharacter.doNotAllow);
        }

        if (options.modifyValueOnWheel.changed) {
            newOptions.modifyValueOnWheel = firstNonNull(options.modifyValueOnWheel.value, AutoNumeric.options.modifyValueOnWheel.doNothing);
        }

        if (options.wheelOn.changed) {
            newOptions.wheelOn = firstNonNull(options.wheelOn.value, AutoNumeric.options.wheelOn.focus);
        }

        instance.update(newOptions);
    }
}

export function updateValue(element, elementId, value) {
    const instance = _instances[elementId];

    if (instance) {
        instance.set(value);
    }
}