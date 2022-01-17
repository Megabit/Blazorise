"use strict";

function _typeof(obj) { "@babel/helpers - typeof"; return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function (obj) { return typeof obj; } : function (obj) { return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }, _typeof(obj); }

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.close = close;
exports.destroy = destroy;
exports.focus = focus;
exports.initialize = initialize;
exports.open = open;
exports.select = select;
exports.toggle = toggle;
exports.updateLocalization = updateLocalization;
exports.updateOptions = updateOptions;
exports.updateValue = updateValue;

require("./vendors/flatpickr.js");

var utilities = _interopRequireWildcard(require("./utilities.js"));

function _getRequireWildcardCache(nodeInterop) { if (typeof WeakMap !== "function") return null; var cacheBabelInterop = new WeakMap(); var cacheNodeInterop = new WeakMap(); return (_getRequireWildcardCache = function _getRequireWildcardCache(nodeInterop) { return nodeInterop ? cacheNodeInterop : cacheBabelInterop; })(nodeInterop); }

function _interopRequireWildcard(obj, nodeInterop) { if (!nodeInterop && obj && obj.__esModule) { return obj; } if (obj === null || _typeof(obj) !== "object" && typeof obj !== "function") { return { "default": obj }; } var cache = _getRequireWildcardCache(nodeInterop); if (cache && cache.has(obj)) { return cache.get(obj); } var newObj = {}; var hasPropertyDescriptor = Object.defineProperty && Object.getOwnPropertyDescriptor; for (var key in obj) { if (key !== "default" && Object.prototype.hasOwnProperty.call(obj, key)) { var desc = hasPropertyDescriptor ? Object.getOwnPropertyDescriptor(obj, key) : null; if (desc && (desc.get || desc.set)) { Object.defineProperty(newObj, key, desc); } else { newObj[key] = obj[key]; } } } newObj["default"] = obj; if (cache) { cache.set(obj, newObj); } return newObj; }

function ownKeys(object, enumerableOnly) { var keys = Object.keys(object); if (Object.getOwnPropertySymbols) { var symbols = Object.getOwnPropertySymbols(object); enumerableOnly && (symbols = symbols.filter(function (sym) { return Object.getOwnPropertyDescriptor(object, sym).enumerable; })), keys.push.apply(keys, symbols); } return keys; }

function _objectSpread(target) { for (var i = 1; i < arguments.length; i++) { var source = null != arguments[i] ? arguments[i] : {}; i % 2 ? ownKeys(Object(source), !0).forEach(function (key) { _defineProperty(target, key, source[key]); }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(target, Object.getOwnPropertyDescriptors(source)) : ownKeys(Object(source)).forEach(function (key) { Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key)); }); } return target; }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

function _toConsumableArray(arr) { return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread(); }

function _nonIterableSpread() { throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); }

function _unsupportedIterableToArray(o, minLen) { if (!o) return; if (typeof o === "string") return _arrayLikeToArray(o, minLen); var n = Object.prototype.toString.call(o).slice(8, -1); if (n === "Object" && o.constructor) n = o.constructor.name; if (n === "Map" || n === "Set") return Array.from(o); if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return _arrayLikeToArray(o, minLen); }

function _iterableToArray(iter) { if (typeof Symbol !== "undefined" && iter[Symbol.iterator] != null || iter["@@iterator"] != null) return Array.from(iter); }

function _arrayWithoutHoles(arr) { if (Array.isArray(arr)) return _arrayLikeToArray(arr); }

function _arrayLikeToArray(arr, len) { if (len == null || len > arr.length) len = arr.length; for (var i = 0, arr2 = new Array(len); i < len; i++) { arr2[i] = arr[i]; } return arr2; }

var _pickers = [];

function initialize(element, elementId, options) {
  element = utilities.getRequiredElement(element, elementId);
  if (!element) return;

  function mutationObserverCallback(mutationsList, observer) {
    mutationsList.forEach(function (mutation) {
      if (mutation.attributeName === 'class') {
        var _picker = _pickers[mutation.target.id];

        if (_picker && _picker.altInput) {
          var altInputClassListToRemove = _toConsumableArray(_picker.altInput.classList).filter(function (cn) {
            return !["input", "active"].includes(cn);
          });

          var inputClassListToAdd = _toConsumableArray(_picker.input.classList).filter(function (cn) {
            return !["flatpickr-input"].includes(cn);
          });

          altInputClassListToRemove.forEach(function (name) {
            _picker.altInput.classList.remove(name);
          });
          inputClassListToAdd.forEach(function (name) {
            _picker.altInput.classList.add(name);
          });
        }
      }
    });
  } // When flatpickr is defined with altInput=true, it will create a second input
  // element while the original input element will be hidden. With MutationObserver
  // we can copy classnames from hidden to the visible element.


  var mutationObserver = new MutationObserver(mutationObserverCallback);
  mutationObserver.observe(document.getElementById(elementId), {
    attributes: true
  });
  var defaultOptions = {
    enableTime: options.inputMode === 1,
    dateFormat: options.inputMode === 1 ? 'Y-m-d H:i' : 'Y-m-d',
    allowInput: true,
    altInput: true,
    altFormat: options.displayFormat ? options.displayFormat : options.inputMode === 1 ? 'Y-m-d H:i' : 'Y-m-d',
    defaultValue: options["default"],
    minDate: options.min,
    maxDate: options.max,
    locale: options.localization || {
      firstDayOfWeek: options.firstDayOfWeek
    },
    time_24hr: options.timeAs24hr ? options.timeAs24hr : false,
    clickOpens: !(options.readOnly || false),
    disable: options.disabledDates || []
  };
  var pluginOptions = options.inputMode === 2 ? {
    plugins: [new monthSelectPlugin({
      shorthand: false,
      dateFormat: "Y-m-d",
      altFormat: "M Y"
    })]
  } : {};
  var picker = flatpickr(element, _objectSpread(_objectSpread({}, defaultOptions), pluginOptions));

  if (options) {
    picker.altInput.disabled = options.disabled || false;
    picker.altInput.readOnly = options.readOnly || false;
  }

  _pickers[elementId] = picker;
}

function destroy(element, elementId) {
  var instances = _pickers || {};
  delete instances[elementId];
}

function updateValue(element, elementId, value) {
  var picker = _pickers[elementId];

  if (picker) {
    picker.setDate(value);
  }
}

function updateOptions(element, elementId, options) {
  var picker = _pickers[elementId];

  if (picker) {
    if (options.firstDayOfWeek.changed) {
      picker.set("firstDayOfWeek", options.firstDayOfWeek.value);
    }

    if (options.displayFormat.changed) {
      picker.set("altFormat", options.displayFormat.value);
    }

    if (options.timeAs24hr.changed) {
      picker.set("time_24hr", options.timeAs24hr.value);
    }

    if (options.min.changed) {
      picker.set("minDate", options.min.value);
    }

    if (options.max.changed) {
      picker.set("maxDate", options.max.value);
    }

    if (options.disabled.changed) {
      picker.altInput.disabled = options.disabled.value;
    }

    if (options.readOnly.changed) {
      picker.altInput.readOnly = options.readOnly.value;
      picker.set("clickOpens", !options.readOnly.value);
    }

    if (options.disabledDates.changed) {
      picker.set("disable", options.disabledDates.value || []);
    }
  }
}

function open(element, elementId) {
  var picker = _pickers[elementId];

  if (picker) {
    picker.open();
  }
}

function close(element, elementId) {
  var picker = _pickers[elementId];

  if (picker) {
    picker.close();
  }
}

function toggle(element, elementId) {
  var picker = _pickers[elementId];

  if (picker) {
    picker.toggle();
  }
}

function updateLocalization(element, elementId, localization) {
  var picker = _pickers[elementId];

  if (picker) {
    picker.config.locale = localization;

    if (picker.l10n) {
      picker.l10n.months = localization.months;
      picker.l10n.weekdays = localization.weekdays;
      picker.l10n.amPM = localization.amPM;
    }

    if (picker.weekdayContainer) {
      for (var i = 0; i < 7; ++i) {
        picker.weekdayContainer.children[0].children[i].innerHtml = localization.weekdays.shorthand[i];
        picker.weekdayContainer.children[0].children[i].innerText = localization.weekdays.shorthand[i];
      }
    }

    if (picker.amPM) {
      var selectedDate = picker.selectedDates && picker.selectedDates.length > 0 ? picker.selectedDates[0] : null;
      var index = selectedDate && selectedDate.getHours() >= 12 ? 1 : 0;
      picker.amPM.innerHtml = localization.amPM[index];
      picker.amPM.innerText = localization.amPM[index];
    }

    picker.redraw();
  }
}

function focus(element, elementId, scrollToElement) {
  var picker = _pickers[elementId];

  if (picker && picker.altInput) {
    utilities.focus(picker.altInput, null, scrollToElement);
  }
}

function select(element, elementId, focus) {
  var picker = _pickers[elementId];

  if (picker && picker.altInput) {
    utilities.select(picker.altInput, null, focus);
  }
}