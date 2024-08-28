import { getRequiredElement } from "./utilities.js?v=1.6.1.0";

export function initializeTableFixedHeader(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    let resizeTimeout = null

    function resizeThottler() {
        if (!resizeTimeout) {
            resizeTimeout = setTimeout(function () {
                resizeTimeout = null;
                resizeHandler(element);
            }.bind(this), 66);
        }
    }

    function resizeHandler(element) {
        const thead = element.querySelector("thead:first-child");
        const tableRows = thead.querySelectorAll("tr");
        if (tableRows !== null && tableRows.length > 1) {
            let previousRowCellHeight = 0;
            for (let i = 0; i < tableRows.length; i++) {
                let currentTh = tableRows[i].querySelectorAll("th");
                currentTh.forEach(x => x.style.top = `${previousRowCellHeight}px`);
                previousRowCellHeight += currentTh[0].offsetHeight;
            }
        }
    }

    resizeHandler(element);

    window.addEventListener("resize", this.resizeThottler, false);
}

export function destroyTableFixedHeader(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    if (typeof this.resizeThottler === "function") {
        window.removeEventListener("resize", this.resizeThottler);
    }

    const thead = element.querySelector("thead:first-child");
    const tableRows = thead.querySelectorAll("tr");

    if (tableRows !== null && tableRows.length > 1) {
        for (let i = 0; i < tableRows.length; i++) {
            let currentTh = tableRows[i].querySelectorAll("th");
            currentTh.forEach(x => x.style.top = `${0}px`);
        }
    }
}

export function fixedHeaderScrollTableToPixels(element, elementId, pixels) {
    if (element && element.parentElement) {

        element.parentElement.scroll({
            top: pixels,
            behavior: "smooth"
        });
    }
}

export function fixedHeaderScrollTableToRow(element, elementId, row) {
    element = getRequiredElement(element, elementId);

    if (element) {
        let rows = element.querySelectorAll("tr");
        let rowsLength = rows.length;

        if (rowsLength > 0 && row >= 0 && row < rowsLength) {
            rows[row].scrollIntoView({
                behavior: "smooth",
                block: "nearest"
            });
        }
    }
}

export function initializeResizable(element, elementId, mode) {
    const resizerClass = "b-table-resizer";
    const resizingClass = "b-table-resizing";
    const resizerHeaderMode = 0;
    let cols = null;

    element = getRequiredElement(element, elementId);

    if (element) {
        const thead = element.querySelector("thead:first-child");
        cols = thead.querySelectorAll('tr:first-child > th');
    }

    if (cols) {

        const calculateTableActualHeight = function () {
            let height = 0;
            if (element !== null) {
                const tableRows = element.querySelectorAll('tr');

                tableRows.forEach(x => {
                    let firstCol = x.querySelector('th:first-child,td:first-child');
                    if (firstCol !== null) {
                        height += firstCol.offsetHeight;
                    }
                });
            }
            return height;
        };

        const calculateModeHeight = () => {
            return mode === resizerHeaderMode
                ? element !== null
                    ? element.querySelector('tr:first-child > th:first-child').offsetHeight
                    : 0
                : calculateTableActualHeight();
        };

        let actualHeight = calculateModeHeight();

        const createResizableColumn = function (col) {
            if (col.querySelector(`.${resizerClass}`) !== null)
                return;
            // Add a resizer element to the column
            const resizer = document.createElement('div');
            resizer.classList.add(resizerClass);

            // Set the height
            resizer.style.height = `${actualHeight}px`;

            resizer.addEventListener("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
            });

            let mouseDownDate;
            let mouseUpDate;

            col.addEventListener('click', function (e) {
                let resized = (mouseDownDate !== null && mouseUpDate !== null);
                if (resized) {
                    let currentDate = new Date();

                    // Checks if mouse down was some ms ago, which means click from resizing
                    let elapsedFromMouseDown = currentDate - mouseDownDate;
                    let clickFromResize = elapsedFromMouseDown > 100;

                    // Checks if mouse up was some ms ago, which either means: 
                    // we clicked from resizing just now or 
                    // did not click from resizing and should handle click normally.
                    let elapsedFromMouseUp = currentDate - mouseUpDate;
                    let clickFromResizeJustNow = elapsedFromMouseUp < 100;

                    if (resized && clickFromResize && clickFromResizeJustNow) {
                        e.preventDefault();
                        e.stopPropagation();
                    }
                    mouseDownDate = null;
                    mouseUpDate = null;
                }
            });
            col.appendChild(resizer);

            // Track the current position of mouse
            let x = 0;
            let w = 0;

            const mouseDownHandler = function (e) {
                mouseDownDate = new Date();

                // Get the current mouse position
                x = e.clientX;

                // Calculate the current width of column
                const styles = window.getComputedStyle(col);
                w = parseInt(styles.width, 10);

                // Attach listeners for document's events
                document.addEventListener('pointermove', mouseMoveHandler);
                document.addEventListener('pointerup', mouseUpHandler);

                resizer.classList.add(resizingClass);
            };

            const mouseMoveHandler = function (e) {
                // Determine how far the mouse has been moved
                const dx = e.clientX - x;

                resizer.style.height = `${calculateTableActualHeight()}px`;

                // Update the width of column
                col.style.width = `${w + dx}px`;
            };

            // When user releases the mouse, remove the existing event listeners
            const mouseUpHandler = function () {
                mouseUpDate = new Date();

                resizer.classList.remove(resizingClass);

                element.querySelectorAll(`.${resizerClass}`).forEach(x => x.style.height = `${calculateModeHeight()}px`);

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

export function destroyResizable(element, elementId) {
    if (element !== null) {
        element.querySelectorAll('.b-table-resizer').forEach(x => x.remove());
    }
}