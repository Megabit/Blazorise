"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.destroy = destroy;
exports.initialize = initialize;
exports.reset = reset;

var _utilities = require("./utilities.js");

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); Object.defineProperty(Constructor, "prototype", { writable: false }); return Constructor; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

var _instances = [];

function initialize(adapter, element, elementId) {
  element = (0, _utilities.getRequiredElement)(element, elementId);
  if (!element) return;
  var nextFileId = 0; // save an instance of adapter

  _instances[elementId] = new FileEditInfo(adapter, element, elementId);
  element.addEventListener('change', function handleInputFileChange(event) {
    // Reduce to purely serializable data, plus build an index by ID
    element._blazorFilesById = {};
    var fileList = Array.prototype.map.call(element.files, function (file) {
      var fileEntry = {
        id: ++nextFileId,
        lastModified: new Date(file.lastModified).toISOString(),
        name: file.name,
        size: file.size,
        type: file.type
      };
      element._blazorFilesById[fileEntry.id] = fileEntry; // Attach the blob data itself as a non-enumerable property so it doesn't appear in the JSON

      Object.defineProperty(fileEntry, 'blob', {
        value: file
      });
      return fileEntry;
    });
    adapter.invokeMethodAsync('NotifyChange', fileList).then(null, function (err) {
      throw new Error(err);
    });
  });
}

function destroy(element, elementId) {
  var instances = _instances || {};
  delete instances[elementId];
}

function reset(element, elementId) {
  element = (0, _utilities.getRequiredElement)(element, elementId);

  if (element) {
    element.value = '';
    var fileEditInfo = _instances[elementId];

    if (fileEditInfo) {
      fileEditInfo.adapter.invokeMethodAsync('NotifyChange', []).then(null, function (err) {
        throw new Error(err);
      });
    }
  }
}

var FileEditInfo = /*#__PURE__*/_createClass(function FileEditInfo(adapter, element, elementId) {
  _classCallCheck(this, FileEditInfo);

  _defineProperty(this, "adapter", void 0);

  _defineProperty(this, "element", void 0);

  _defineProperty(this, "elementId", void 0);

  this.adapter = adapter;
  this.element = element;
  this.elementId = elementId;
});