"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.destroy = destroy;
exports.initialize = initialize;
exports.updateOptions = updateOptions;

var _NumericMaskValidator = require("./validators/NumericMaskValidator.js");

var _utilities = require("./utilities.js");

var _instances = [];

function initialize(dotnetAdapter, element, elementId, options) {
  element = (0, _utilities.getRequiredElement)(element, elementId);
  if (!element) return;
  var instance = new _NumericMaskValidator.NumericMaskValidator(dotnetAdapter, element, elementId, options);
  _instances[elementId] = instance;
  element.addEventListener("keypress", function (e) {
    keyPress(_instances[elementId], e);
  });
  element.addEventListener("paste", function (e) {
    paste(_instances[elementId], e);
  });
  element.addEventListener("focus", function (e) {
    selectAll(_instances[elementId], e);
  });

  if (instance.decimals && instance.decimals !== 2) {
    instance.truncate();
  }
}

function destroy(element, elementId) {
  var instances = _instances || {};
  delete instances[elementId];
}

function updateOptions(element, elementId, options) {
  var instance = _instances[elementId];

  if (instance) {
    instance.update(options);
  }
}

function keyPress(validator, e) {
  var currentValue = String.fromCharCode(e.which);
  return e.which === 13 // still need to allow ENTER key so that we don't preventDefault on form submit
  || validator.isValid(currentValue) || e.preventDefault();
}

function paste(validator, e) {
  return validator.isValid(e.clipboardData.getData("text/plain")) || e.preventDefault();
}

function selectAll(validator, e) {
  if (validator.selectAllOnFocus && validator.element) {
    var element = validator.element;

    if (element.value && element.value.length > 0) {
      element.setSelectionRange(0, element.value.length);
    }
  }
}