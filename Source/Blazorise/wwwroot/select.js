"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.getSelectedOptions = getSelectedOptions;
exports.setSelectedOptions = setSelectedOptions;

function getSelectedOptions(elementId) {
  var element = document.getElementById(elementId);
  var len = element.options.length;
  var opts = [],
      opt;

  for (var i = 0; i < len; i++) {
    opt = element.options[i];

    if (opt.selected) {
      opts.push(opt.value);
    }
  }

  return opts;
}

function setSelectedOptions(elementId, values) {
  var element = document.getElementById(elementId);

  if (element && element.options) {
    var len = element.options.length;

    var _loop = function _loop() {
      var opt = element.options[i];

      if (values && values.find(function (x) {
        return x !== null && x.toString() === opt.value;
      })) {
        opt.selected = true;
      } else {
        opt.selected = false;
      }
    };

    for (var i = 0; i < len; i++) {
      _loop();
    }
  }
}