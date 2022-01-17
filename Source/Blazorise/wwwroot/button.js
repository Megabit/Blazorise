"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.click = click;
exports.destroy = destroy;
exports.initialize = initialize;

var _utilities = require("./utilities.js");

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); Object.defineProperty(Constructor, "prototype", { writable: false }); return Constructor; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

var _instances = [];

function initialize(element, elementId, options) {
  element = (0, _utilities.getRequiredElement)(element, elementId);
  if (!element) return;
  _instances[elementId] = new ButtonInfo(element, elementId, options);

  if (element && element.type === "submit") {
    element.addEventListener("click", function (e) {
      click(_instances[elementId], e);
    });
  }
}

function destroy(element, elementId) {
  var instances = _instances || {};
  delete instances[elementId];
}

function click(buttonInfo, e) {
  if (buttonInfo.options.preventDefaultOnSubmit) {
    return e.preventDefault();
  }
}

var ButtonInfo = /*#__PURE__*/_createClass(function ButtonInfo(element, elementId, options) {
  _classCallCheck(this, ButtonInfo);

  _defineProperty(this, "elementId", null);

  _defineProperty(this, "element", null);

  _defineProperty(this, "options", {});

  this.elementId = elementId;
  this.element = element;
  this.options = options || {};
});