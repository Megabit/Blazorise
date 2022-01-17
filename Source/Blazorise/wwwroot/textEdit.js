"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.destroy = destroy;
exports.initialize = initialize;

var _NumericMaskValidator = require("./validators/NumericMaskValidator.js");

var _DateTimeMaskValidator = require("./validators/DateTimeMaskValidator.js");

var _RegExMaskValidator = require("./validators/RegExMaskValidator.js");

var _NoValidator = require("./validators/NoValidator.js");

var _utilities = require("./utilities.js");

var _instances = [];

function initialize(element, elementId, maskType, editMask) {
  element = (0, _utilities.getRequiredElement)(element, elementId);
  if (!element) return;
  var instances = _instances = _instances || {};

  if (maskType === "numeric") {
    instances[elementId] = new _NumericMaskValidator.NumericMaskValidator(null, element, elementId);
  } else if (maskType === "datetime") {
    instances[elementId] = new _DateTimeMaskValidator.DateTimeMaskValidator(element, elementId);
  } else if (maskType === "regex") {
    instances[elementId] = new _RegExMaskValidator.RegExMaskValidator(element, elementId, editMask);
  } else {
    instances[elementId] = new _NoValidator.NoValidator();
  }

  element.addEventListener("keypress", function (e) {
    keyPress(instances[elementId], e);
  });
  element.addEventListener("paste", function (e) {
    paste(instances[elementId], e);
  });
}

function destroy(element, elementId) {
  var instances = _instances || {};
  delete instances[elementId];
}

function keyPress(validator, e) {
  var currentValue = String.fromCharCode(e.which);
  return validator.isValid(currentValue) || e.preventDefault();
}

function paste(validator, e) {
  return validator.isValid(e.clipboardData.getData("text/plain")) || e.preventDefault();
}