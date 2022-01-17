"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.addBreakpointComponent = addBreakpointComponent;
exports.findBreakpointComponentIndex = findBreakpointComponentIndex;
exports.getBreakpoint = getBreakpoint;
exports.isBreakpointComponent = isBreakpointComponent;
exports.registerBreakpointComponent = registerBreakpointComponent;
exports.unregisterBreakpointComponent = unregisterBreakpointComponent;
// holds the list of components that are triggers to breakpoint
var breakpointComponents = [];
var lastBreakpoint = null; // Recalculate breakpoint on resize

if (window.attachEvent) {
  window.attachEvent('onresize', windowResized);
} else if (window.addEventListener) {
  window.addEventListener('resize', windowResized, true);
} else {//The browser does not support Javascript event binding
}

function windowResized() {
  if (breakpointComponents && breakpointComponents.length > 0) {
    var currentBreakpoint = getBreakpoint();

    if (lastBreakpoint !== currentBreakpoint) {
      lastBreakpoint = currentBreakpoint;
      var index = 0;

      for (index = 0; index < breakpointComponents.length; ++index) {
        onBreakpoint(breakpointComponents[index].dotnetAdapter, currentBreakpoint);
      }
    }
  }
} // Set initial breakpoint


lastBreakpoint = getBreakpoint(); // Get the current breakpoint

function getBreakpoint() {
  return window.getComputedStyle(document.body, ':before').content.replace(/\"/g, '');
}

function addBreakpointComponent(elementId, dotnetAdapter) {
  breakpointComponents.push({
    elementId: elementId,
    dotnetAdapter: dotnetAdapter
  });
}

function findBreakpointComponentIndex(elementId) {
  var index = 0;

  for (index = 0; index < breakpointComponents.length; ++index) {
    if (breakpointComponents[index].elementId === elementId) return index;
  }

  return -1;
}

function isBreakpointComponent(elementId) {
  var index = 0;

  for (index = 0; index < breakpointComponents.length; ++index) {
    if (breakpointComponents[index].elementId === elementId) return true;
  }

  return false;
}

function onBreakpoint(dotnetAdapter, currentBreakpoint) {
  dotnetAdapter.invokeMethodAsync('OnBreakpoint', currentBreakpoint);
}

function registerBreakpointComponent(dotnetAdapter, elementId) {
  if (isBreakpointComponent(elementId) !== true) {
    addBreakpointComponent(elementId, dotnetAdapter);
  }
}

function unregisterBreakpointComponent(elementId) {
  var index = findBreakpointComponentIndex(elementId);

  if (index !== -1) {
    breakpointComponents.splice(index, 1);
  }
}