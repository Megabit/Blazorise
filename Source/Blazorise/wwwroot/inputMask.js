"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.destroy = destroy;
exports.extendAliases = extendAliases;
exports.initialize = initialize;

var _inputmask = _interopRequireDefault(require("./vendors/inputmask.js"));

var _utilities = require("./utilities.js");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { "default": obj }; }

function ownKeys(object, enumerableOnly) { var keys = Object.keys(object); if (Object.getOwnPropertySymbols) { var symbols = Object.getOwnPropertySymbols(object); enumerableOnly && (symbols = symbols.filter(function (sym) { return Object.getOwnPropertyDescriptor(object, sym).enumerable; })), keys.push.apply(keys, symbols); } return keys; }

function _objectSpread(target) { for (var i = 1; i < arguments.length; i++) { var source = null != arguments[i] ? arguments[i] : {}; i % 2 ? ownKeys(Object(source), !0).forEach(function (key) { _defineProperty(target, key, source[key]); }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(target, Object.getOwnPropertyDescriptors(source)) : ownKeys(Object(source)).forEach(function (key) { Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key)); }); } return target; }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

var _instances = [];

function initialize(dotnetAdapter, element, elementId, options) {
  element = (0, _utilities.getRequiredElement)(element, elementId);
  if (!element) return;
  var maskOptions = options.mask ? {
    mask: options.mask
  } : {};
  var regexOptions = options.mask ? {
    regex: options.regex
  } : {};
  var aliasOptions = options.alias ? {
    alias: options.alias,
    inputFormat: options.inputFormat,
    outputFormat: options.outputFormat
  } : {};
  var otherOptions = {
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
    oncomplete: function oncomplete(e) {
      dotnetAdapter.invokeMethodAsync('NotifyCompleted', e.target.value);
    },
    onincomplete: function onincomplete(e) {
      dotnetAdapter.invokeMethodAsync('NotifyIncompleted', e.target.value);
    },
    oncleared: function oncleared() {
      dotnetAdapter.invokeMethodAsync('NotifyCleared');
    }
  };
  var finalOptions = options.alias ? _objectSpread(_objectSpread({}, aliasOptions), otherOptions) : _objectSpread(_objectSpread(_objectSpread({}, maskOptions), regexOptions), otherOptions);
  var inputMask = new _inputmask["default"](finalOptions);
  inputMask.mask(element);
  _instances[elementId] = {
    dotnetAdapter: dotnetAdapter,
    element: element,
    elementId: elementId,
    inputMask: inputMask
  };
}

function destroy(element, elementId) {
  var instances = _instances || {};
  delete instances[elementId];
}

function extendAliases(element, elementId, aliasOptions) {
  var instance = _instances[elementId];

  if (instance && instance.inputMask) {
    instance.inputMask.extendAliases(aliasOptions);
  }
}