"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.NoValidator = NoValidator;

function NoValidator() {
  this.isValid = function (currentValue) {
    return true;
  };
}