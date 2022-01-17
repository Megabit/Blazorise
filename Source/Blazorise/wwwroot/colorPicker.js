"use strict";

function _typeof(obj) { "@babel/helpers - typeof"; return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function (obj) { return typeof obj; } : function (obj) { return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }, _typeof(obj); }

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.applyHexColor = applyHexColor;
exports.destroy = destroy;
exports.focus = focus;
exports.initialize = initialize;
exports.select = select;
exports.updateLocalization = updateLocalization;
exports.updateOptions = updateOptions;
exports.updateValue = updateValue;

require("./vendors/Pickr.js");

var utilities = _interopRequireWildcard(require("./utilities.js"));

function _getRequireWildcardCache(nodeInterop) { if (typeof WeakMap !== "function") return null; var cacheBabelInterop = new WeakMap(); var cacheNodeInterop = new WeakMap(); return (_getRequireWildcardCache = function _getRequireWildcardCache(nodeInterop) { return nodeInterop ? cacheNodeInterop : cacheBabelInterop; })(nodeInterop); }

function _interopRequireWildcard(obj, nodeInterop) { if (!nodeInterop && obj && obj.__esModule) { return obj; } if (obj === null || _typeof(obj) !== "object" && typeof obj !== "function") { return { "default": obj }; } var cache = _getRequireWildcardCache(nodeInterop); if (cache && cache.has(obj)) { return cache.get(obj); } var newObj = {}; var hasPropertyDescriptor = Object.defineProperty && Object.getOwnPropertyDescriptor; for (var key in obj) { if (key !== "default" && Object.prototype.hasOwnProperty.call(obj, key)) { var desc = hasPropertyDescriptor ? Object.getOwnPropertyDescriptor(obj, key) : null; if (desc && (desc.get || desc.set)) { Object.defineProperty(newObj, key, desc); } else { newObj[key] = obj[key]; } } } newObj["default"] = obj; if (cache) { cache.set(obj, newObj); } return newObj; }

var _instancesInfos = [];

function initialize(dotnetAdapter, element, elementId, options) {
  element = utilities.getRequiredElement(element, elementId);
  if (!element) return;
  var picker = Pickr.create({
    el: element,
    theme: 'monolith',
    // or 'monolith', or 'nano'
    useAsButton: element,
    comparison: false,
    "default": options["default"] || "#000000",
    position: 'bottom-start',
    silent: true,
    swatches: options.showPalette ? options.palette : null,
    components: {
      //palette: false,
      // Main components
      preview: true,
      opacity: true,
      hue: false,
      // Input / output Options
      interaction: {
        hex: true,
        rgba: true,
        hsla: false,
        hsva: false,
        cmyk: false,
        input: true,
        save: false,
        clear: options.showClearButton || true,
        cancel: options.showCancelButton || true
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
  var hexColor = options["default"] ? options["default"] : "#000000";
  var previewElement = element.querySelector(":scope > .b-input-color-picker-preview > .b-input-color-picker-curent-color");
  var instanceInfo = {
    picker: picker,
    dotnetAdapter: dotnetAdapter,
    element: element,
    elementId: elementId,
    previewElement: previewElement,
    hexColor: hexColor,
    palette: options.palette || [],
    showPalette: options.showPalette || true,
    hideAfterPaletteSelect: options.hideAfterPaletteSelect || true,
    showButtons: options.showButtons || true
  };
  applyHexColor(instanceInfo, hexColor, true);
  var hexColorShow = picker.getColor() ? picker.getColor().toHEXA().toString() : null;

  if (options.disabled) {
    picker.disable();
  }

  picker.on('show', function (color, instance) {
    hexColorShow = color ? color.toHEXA().toString() : null;
  }).on("cancel", function (instance) {
    applyHexColor(instanceInfo, hexColorShow);
    instanceInfo.picker.setColor(hexColorShow, true);
    instanceInfo.picker.hide();
  }).on("clear", function (instance) {
    hexColorShow = null;
    applyHexColor(instanceInfo, null);
  }).on("changestop", function (source, instance) {
    var hexColor = instance.getColor() ? instance.getColor().toHEXA().toString() : null;
    applyHexColor(instanceInfo, hexColor);
  }).on("swatchselect", function (color, instance) {
    var hexColor = color ? color.toHEXA().toString() : null;
    applyHexColor(instanceInfo, hexColor);

    if (instanceInfo.hideAfterPaletteSelect) {
      instanceInfo.picker.hide();
    }
  });
  _instancesInfos[elementId] = instanceInfo;
}

function destroy(element, elementId) {
  var instanceInfo = _instancesInfos || {};
  delete instanceInfo[elementId];
}

function updateValue(element, elementId, hexColor) {
  var instanceInfo = _instancesInfos[elementId];

  if (instanceInfo) {
    applyHexColor(instanceInfo, hexColor);
  }
}

function updateOptions(element, elementId, options) {
  var instanceInfo = _instancesInfos[elementId];

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

function updateLocalization(element, elementId, localization) {
  var instanceInfo = _instancesInfos[elementId];

  if (instanceInfo) {
    instanceInfo.picker.options.i18n = localization;
    instanceInfo.picker._root.interaction.save.value = localization["btn:save"];
    instanceInfo.picker._root.interaction.cancel.value = localization["btn:cancel"];
    instanceInfo.picker._root.interaction.clear.value = localization["btn:clear"];
  }
}

function focus(element, elementId, scrollToElement) {
  var instanceInfo = _instancesInfos[elementId];

  if (instanceInfo) {
    utilities.focus(picker.element, null, scrollToElement);
  }
}

function select(element, elementId, focus) {
  var instanceInfo = _instancesInfos[elementId];

  if (instanceInfo) {
    utilities.select(picker.element, null, focus);
  }
}

function applyHexColor(instanceInfo, hexColor) {
  var force = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : false;

  if (instanceInfo.hexColor !== hexColor || force) {
    instanceInfo.hexColor = hexColor;

    if (instanceInfo.previewElement) {
      instanceInfo.previewElement.style.backgroundColor = hexColor;
    }

    if (instanceInfo.element) {
      instanceInfo.element.setAttribute('data-color', hexColor);
    }

    if (instanceInfo.dotnetAdapter) {
      instanceInfo.dotnetAdapter.invokeMethodAsync('SetValue', hexColor);
    }
  }
}