import "./vendors/Pickr.js?v=1.6.1.0";
import * as utilities from "./utilities.js?v=1.6.1.0";

const _instancesInfos = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = utilities.getRequiredElement(element, elementId);

    if (!element)
        return;

    const picker = Pickr.create({
        el: element,
        theme: 'monolith', // 'monolith' or 'nano'

        useAsButton: element,

        comparison: false,
        default: options.default || "#000000",
        position: 'bottom-start',
        silent: true,

        swatches: options.showPalette ? options.palette : null,
        components: {
            //palette: false,

            // Main components
            preview: true,
            opacity: options.showOpacitySlider === true ? true : false,
            hue: options.showHueSlider === true ? true : false,

            // Input / output Options
            interaction: {
                hex: true,
                rgba: true,
                hsla: false,
                hsva: false,
                cmyk: false,
                input: options.showInputField === true ? true : false,
                save: false,
                clear: options.showClearButton === true ? true : false,
                cancel: options.showCancelButton === true ? true : false
            }
        },

        // Translations, these are the default values.
        i18n: options.localization || {
            // Strings visible in the UI
            'ui:dialog': 'color picker dialog',
            'btn:toggle': 'toggle color picker dialog',
            'btn:swatch': 'color swatch',
            'btn:last-color': 'use previous color',
            'btn:save': 'Save',
            'btn:cancel': 'Cancel',
            'btn:clear': 'Clear',

            // Strings used for aria-labels
            'aria:btn:save': 'save and close',
            'aria:btn:cancel': 'cancel and close',
            'aria:btn:clear': 'clear and close',
            'aria:input': 'color input field',
            'aria:palette': 'color selection area',
            'aria:hue': 'hue selection slider',
            'aria:opacity': 'selection slider'
        }
    });

    const hexColor = options.default ? options.default : "#000000";

    const colorPreviewElement = element.querySelector(options.colorPreviewElementSelector || ":scope > .b-input-color-picker-preview > .b-input-color-picker-curent-color");
    const colorValueElement = element.querySelector(options.colorValueElementSelector || ":scope > .b-input-color-picker-preview > .b-input-color-picker-curent-value");

    const instanceInfo = {
        picker: picker,
        dotnetAdapter: dotnetAdapter,
        element: element,
        elementId: elementId,
        colorPreviewElement: colorPreviewElement,
        colorValueElement: colorValueElement,
        hexColor: hexColor,
        palette: options.palette || [],
        showPalette: options.showPalette || true,
        hideAfterPaletteSelect: options.hideAfterPaletteSelect || true,
        showButtons: options.showButtons || true
    };

    applyHexColor(instanceInfo, hexColor, true);

    let hexColorShow = picker.getColor() ? picker.getColor().toHEXA().toString() : null;

    if (options.disabled) {
        picker.disable();
    }

    picker
        .on('show', (color, instance) => {
            hexColorShow = color ? color.toHEXA().toString() : null;
        })
        .on("cancel", (instance) => {
            applyHexColor(instanceInfo, hexColorShow);
            instanceInfo.picker.setColor(hexColorShow, true);
            instanceInfo.picker.hide()
        })
        .on("clear", (instance) => {
            hexColorShow = null;
            applyHexColor(instanceInfo, null);
        })
        .on("changestop", (source, instance) => {
            const hexColor = instance.getColor() ? instance.getColor().toHEXA().toString() : null;
            applyHexColor(instanceInfo, hexColor);
        })
        .on("swatchselect", (color, instance) => {
            const hexColor = color ? color.toHEXA().toString() : null;
            applyHexColor(instanceInfo, hexColor);

            if (instanceInfo.hideAfterPaletteSelect) {
                instanceInfo.picker.hide();
            }
        });

    _instancesInfos[elementId] = instanceInfo;
}

export function destroy(element, elementId) {
    const instanceInfo = _instancesInfos || {};
    delete instanceInfo[elementId];
}

export function updateValue(element, elementId, hexColor) {
    const instanceInfo = _instancesInfos[elementId];

    if (instanceInfo) {
        applyHexColor(instanceInfo, hexColor);
    }
}

export function updateOptions(element, elementId, options) {
    const instanceInfo = _instancesInfos[elementId];

    if (instanceInfo) {
        if (options.palette.changed) {
            instanceInfo.palette = options.palette.value || [];
            instanceInfo.picker.setSwatches(instanceInfo.palette);
        }

        if (options.showPalette.changed) {
            if (options.showPalette.value) {
                instanceInfo.picker.setSwatches(instanceInfo.palette);
            } else {
                instanceInfo.picker.setSwatches([]);
            }
        }

        if (options.hideAfterPaletteSelect.changed) {
            instanceInfo.hideAfterPaletteSelect = options.hideAfterPaletteSelect.value;
        }

        if (options.disabled.changed || options.readOnly.changed) {
            if (options.disabled.value || options.readOnly.value) {
                instanceInfo.picker.disable();
            } else {
                instanceInfo.picker.enable();
            }
        }
    }
}

export function updateLocalization(element, elementId, localization) {
    const instanceInfo = _instancesInfos[elementId];

    if (instanceInfo) {
        instanceInfo.picker.options.i18n = localization;

        instanceInfo.picker._root.interaction.save.value = localization["btn:save"];
        instanceInfo.picker._root.interaction.cancel.value = localization["btn:cancel"];
        instanceInfo.picker._root.interaction.clear.value = localization["btn:clear"];
    }
}

export function focus(element, elementId, scrollToElement) {
    const instanceInfo = _instancesInfos[elementId];

    if (instanceInfo) {
        utilities.focus(picker.element, null, scrollToElement);
    }
}

export function select(element, elementId, focus) {
    const instanceInfo = _instancesInfos[elementId];

    if (instanceInfo) {
        utilities.select(picker.element, null, focus);
    }
}

export function applyHexColor(instanceInfo, hexColor, force = false) {
    if (instanceInfo.hexColor !== hexColor || force) {
        instanceInfo.hexColor = hexColor;

        if (instanceInfo.colorPreviewElement) {
            instanceInfo.colorPreviewElement.style.backgroundColor = hexColor;
        }

        if (instanceInfo.colorValueElement) {
            instanceInfo.colorValueElement.innerText = hexColor;
        }

        if (instanceInfo.element) {
            instanceInfo.element.setAttribute('data-color', hexColor);
        }

        if (instanceInfo.dotnetAdapter) {
            instanceInfo.dotnetAdapter.invokeMethodAsync('SetValue', hexColor);
        }
    }
}