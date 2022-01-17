"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.destroy = destroy;
exports.initialize = initialize;
exports.updateOptions = updateOptions;

var _Behave = require("./vendors/Behave.js");

var _utilities = require("./utilities.js");

var _instances = [];

function initialize(element, elementId, options) {
  element = (0, _utilities.getRequiredElement)(element, elementId);
  if (!element) return;
  var replaceTab = options.replaceTab || false;
  var tabSize = options.tabSize || 4;
  var softTabs = options.tabSize || true;
  var behave = replaceTab ? new _Behave.Behave({
    textarea: element,
    replaceTab: replaceTab,
    softTabs: softTabs,
    tabSize: tabSize,
    autoOpen: true,
    overwrite: true,
    autoStrip: true,
    autoIndent: true,
    fence: false
  }) : null;

  if (options.autoSize) {
    element.oninput = calculateAutoHeight;
  }

  _instances[elementId] = {
    element: element,
    elementId: elementId,
    replaceTab: replaceTab,
    tabSize: tabSize,
    softTabs: softTabs,
    behave: behave
  };
}

function destroy(element, elementId) {
  var instance = _instances[elementId];

  if (instance && instance.behave) {
    instance.behave.destroy();
    instance.behave = null;
  }

  delete _instances[elementId];
}

function updateOptions(element, elementId, options) {
  var instance = _instances[elementId];

  if (instance) {
    if (options.replaceTab.changed || options.tabSize.changed || options.softTabs.changed) {
      instance.replaceTab = options.replaceTab.value;
      instance.tabSize = options.tabSize.value;
      instance.softTabs = options.softTabs.value;

      if (instance.behave) {
        instance.behave.destroy();
        instance.behave = null;
      }

      if (instance.replaceTab) {
        instance.behave = new _Behave.Behave({
          textarea: element,
          replaceTab: instance.replaceTab,
          softTabs: instance.softTabs,
          tabSize: instance.tabSize,
          autoOpen: true,
          overwrite: true,
          autoStrip: true,
          autoIndent: true,
          fence: false
        });
      }
    }

    if (options.autoSize.changed) {
      element.oninput = options.autoSize.value ? calculateAutoHeight : function () {};
    }
  }

  ;
}

function calculateAutoHeight(e) {
  if (e && e.target) {
    e.target.style.height = 'auto';
    e.target.style.height = this.scrollHeight + 'px';
  }
}