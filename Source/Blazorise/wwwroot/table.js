"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.destroyResizable = destroyResizable;
exports.destroyTableFixedHeader = destroyTableFixedHeader;
exports.fixedHeaderScrollTableToPixels = fixedHeaderScrollTableToPixels;
exports.fixedHeaderScrollTableToRow = fixedHeaderScrollTableToRow;
exports.initializeResizable = initializeResizable;
exports.initializeTableFixedHeader = initializeTableFixedHeader;

var _utilities = require("./utilities.js");

function initializeTableFixedHeader(element, elementId) {
  element = (0, _utilities.getRequiredElement)(element, elementId);
  if (!element) return;
  var resizeTimeout = null;

  function resizeThottler() {
    if (!resizeTimeout) {
      resizeTimeout = setTimeout(function () {
        resizeTimeout = null;
        resizeHandler(element);
      }.bind(this), 66);
    }
  }

  function resizeHandler(element) {
    var thead = element.querySelector("thead:first-child");
    var tableRows = thead.querySelectorAll("tr");

    if (tableRows !== null && tableRows.length > 1) {
      (function () {
        var previousRowCellHeight = 0;

        for (var i = 0; i < tableRows.length; i++) {
          var currentTh = tableRows[i].querySelectorAll("th");
          currentTh.forEach(function (x) {
            return x.style.top = "".concat(previousRowCellHeight, "px");
          });
          previousRowCellHeight += currentTh[0].offsetHeight;
        }
      })();
    }
  }

  resizeHandler(element);
  window.addEventListener("resize", this.resizeThottler, false);
}

function destroyTableFixedHeader(element, elementId) {
  element = (0, _utilities.getRequiredElement)(element, elementId);
  if (!element) return;

  if (typeof this.resizeThottler === "function") {
    window.removeEventListener("resize", this.resizeThottler);
  }

  var thead = element.querySelector("thead:first-child");
  var tableRows = thead.querySelectorAll("tr");

  if (tableRows !== null && tableRows.length > 1) {
    for (var i = 0; i < tableRows.length; i++) {
      var currentTh = tableRows[i].querySelectorAll("th");
      currentTh.forEach(function (x) {
        return x.style.top = "".concat(0, "px");
      });
    }
  }
}

function fixedHeaderScrollTableToPixels(element, elementId, pixels) {
  if (element && element.parentElement) {
    element.parentElement.scrollTop = pixels;
  }
}

function fixedHeaderScrollTableToRow(element, elementId, row) {
  element = (0, _utilities.getRequiredElement)(element, elementId);

  if (element) {
    var rows = element.querySelectorAll("tr");
    var rowsLength = rows.length;

    if (rowsLength > 0 && row >= 0 && row < rowsLength) {
      rows[row].scrollIntoView({
        behavior: "smooth",
        block: "nearest"
      });
    }
  }
}

function initializeResizable(element, elementId, mode) {
  var resizerClass = "b-table-resizer";
  var resizingClass = "b-table-resizing";
  var resizerHeaderMode = 0;
  var cols = null;
  element = (0, _utilities.getRequiredElement)(element, elementId);

  if (element) {
    var thead = element.querySelector("thead:first-child");
    cols = thead.querySelectorAll('tr:first-child > th');
  }

  if (cols) {
    var calculateTableActualHeight = function calculateTableActualHeight() {
      var height = 0;

      if (element !== null) {
        var tableRows = element.querySelectorAll('tr');
        tableRows.forEach(function (x) {
          var firstCol = x.querySelector('th:first-child,td:first-child');

          if (firstCol !== null) {
            height += firstCol.offsetHeight;
          }
        });
      }

      return height;
    };

    var calculateModeHeight = function calculateModeHeight() {
      return mode === resizerHeaderMode ? element !== null ? element.querySelector('tr:first-child > th:first-child').offsetHeight : 0 : calculateTableActualHeight();
    };

    var actualHeight = calculateModeHeight();

    var createResizableColumn = function createResizableColumn(col) {
      if (col.querySelector(".".concat(resizerClass)) !== null) return; // Add a resizer element to the column

      var resizer = document.createElement('div');
      resizer.classList.add(resizerClass); // Set the height

      resizer.style.height = "".concat(actualHeight, "px");
      resizer.addEventListener("click", function (e) {
        e.preventDefault();
        e.stopPropagation();
      });
      var mouseDownDate;
      var mouseUpDate;
      col.addEventListener('click', function (e) {
        var resized = mouseDownDate !== null && mouseUpDate !== null;

        if (resized) {
          var currentDate = new Date(); // Checks if mouse down was some ms ago, which means click from resizing

          var elapsedFromMouseDown = currentDate - mouseDownDate;
          var clickFromResize = elapsedFromMouseDown > 100; // Checks if mouse up was some ms ago, which either means: 
          // we clicked from resizing just now or 
          // did not click from resizing and should handle click normally.

          var elapsedFromMouseUp = currentDate - mouseUpDate;
          var clickFromResizeJustNow = elapsedFromMouseUp < 100;

          if (resized && clickFromResize && clickFromResizeJustNow) {
            e.preventDefault();
            e.stopPropagation();
          }

          mouseDownDate = null;
          mouseUpDate = null;
        }
      });
      col.appendChild(resizer); // Track the current position of mouse

      var x = 0;
      var w = 0;

      var mouseDownHandler = function mouseDownHandler(e) {
        mouseDownDate = new Date(); // Get the current mouse position

        x = e.clientX; // Calculate the current width of column

        var styles = window.getComputedStyle(col);
        w = parseInt(styles.width, 10); // Attach listeners for document's events

        document.addEventListener('pointermove', mouseMoveHandler);
        document.addEventListener('pointerup', mouseUpHandler);
        resizer.classList.add(resizingClass);
      };

      var mouseMoveHandler = function mouseMoveHandler(e) {
        // Determine how far the mouse has been moved
        var dx = e.clientX - x;
        resizer.style.height = "".concat(calculateTableActualHeight(), "px"); // Update the width of column

        col.style.width = "".concat(w + dx, "px");
      }; // When user releases the mouse, remove the existing event listeners


      var mouseUpHandler = function mouseUpHandler() {
        mouseUpDate = new Date();
        resizer.classList.remove(resizingClass);
        element.querySelectorAll(".".concat(resizerClass)).forEach(function (x) {
          return x.style.height = "".concat(calculateModeHeight(), "px");
        });
        document.removeEventListener('pointermove', mouseMoveHandler);
        document.removeEventListener('pointerup', mouseUpHandler);
      };

      resizer.addEventListener('pointerdown', mouseDownHandler);
    };

    [].forEach.call(cols, function (col) {
      createResizableColumn(col);
    });
  }
}

function destroyResizable(element, elementId) {
  if (element !== null) {
    element.querySelectorAll('.b-table-resizer').forEach(function (x) {
      return x.remove();
    });
  }
}