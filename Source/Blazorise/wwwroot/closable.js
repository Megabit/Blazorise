"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.registerClosableComponent = registerClosableComponent;
exports.unregisterClosableComponent = unregisterClosableComponent;
var closableComponents = [];
var lastClickedDocumentElement = null;

function addClosableComponent(elementId, dotnetAdapter) {
  closableComponents.push({
    elementId: elementId,
    dotnetAdapter: dotnetAdapter
  });
}

function findClosableComponent(elementId) {
  var index = 0;

  for (index = 0; index < closableComponents.length; ++index) {
    if (closableComponents[index].elementId === elementId) return closableComponents[index];
  }

  return null;
}

function findClosableComponentIndex(elementId) {
  var index = 0;

  for (index = 0; index < closableComponents.length; ++index) {
    if (closableComponents[index].elementId === elementId) return index;
  }

  return -1;
}

function isClosableComponent(elementId) {
  var index = 0;

  for (index = 0; index < closableComponents.length; ++index) {
    if (closableComponents[index].elementId === elementId) return true;
  }

  return false;
}

function tryClose(closable, targetElementId, isEscapeKey, isChildClicked) {
  var request = new Promise(function (resolve, reject) {
    closable.dotnetAdapter.invokeMethodAsync('SafeToClose', targetElementId, isEscapeKey ? 'escape' : 'leave', isChildClicked).then(function (result) {
      return resolve({
        elementId: closable.elementId,
        dotnetAdapter: closable.dotnetAdapter,
        status: result === true ? 'ok' : 'cancelled'
      });
    })["catch"](function () {
      return resolve({
        elementId: closable.elementId,
        status: 'error'
      });
    });
  });

  if (request) {
    request.then(function (response) {
      if (response.status === 'ok') {
        response.dotnetAdapter.invokeMethodAsync('Close', isEscapeKey ? 'escape' : 'leave') // If the user navigates to another page then it will raise exception because the reference to the component cannot be found.
        // In that case just remove the elementId from the list.
        ["catch"](function () {
          return unregisterClosableComponent(response.elementId);
        });
      }
    });
  }
}

function registerClosableComponent(dotnetAdapter, element) {
  if (element) {
    if (isClosableComponent(element.id) !== true) {
      addClosableComponent(element.id, dotnetAdapter);
    }
  }
}

function unregisterClosableComponent(element) {
  if (element) {
    var index = findClosableComponentIndex(element.id);

    if (index !== -1) {
      closableComponents.splice(index, 1);
    }
  }
}

function hasParentInTree(element, parentElementId) {
  if (!element.parentElement) return false;
  if (element.parentElement.id === parentElementId) return true;
  return hasParentInTree(element.parentElement, parentElementId);
}

document.addEventListener('mousedown', function handler(evt) {
  lastClickedDocumentElement = evt.target;
});
document.addEventListener('mouseup', function handler(evt) {
  if (evt.button === 0 && evt.target === lastClickedDocumentElement && closableComponents && closableComponents.length > 0) {
    var lastClosable = closableComponents[closableComponents.length - 1];

    if (lastClosable) {
      tryClose(lastClosable, evt.target.id, false, hasParentInTree(evt.target, lastClosable.elementId));
    }
  }
});
document.addEventListener('keyup', function handler(evt) {
  if (evt.keyCode === 27 && closableComponents && closableComponents.length > 0) {
    var lastClosable = closableComponents[closableComponents.length - 1];

    if (lastClosable) {
      tryClose(lastClosable, lastClosable.elementId, true, hasParentInTree(evt.target, lastClosable.elementId));
    }
  }
});