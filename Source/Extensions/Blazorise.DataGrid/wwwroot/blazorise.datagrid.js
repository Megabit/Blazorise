﻿window.blazoriseDataGrid = {

    initResizable: function (table, mode) {
        const resizerClass = "b-datagrid-resizer";
        const resizingClass = "b-datagrid-resizing";
        const resizerHeaderMode = 0;

        const cols = table.querySelectorAll('tr:first-child > th');
        if (cols != null) {

            const calculateTableActualHeight = function () {
                let height = 0;
                const tableRows = table.querySelectorAll('tr');

                tableRows.forEach(x => {
                    let firstCol = x.querySelector('th:first-child,td:first-child');
                    if (firstCol != null) {
                        height += firstCol.offsetHeight;
                    }
                });
                return height;
            };

            const calculateModeHeight = () => {
                return mode == resizerHeaderMode ? table.querySelector('tr:first-child > th:first-child').offsetHeight : calculateTableActualHeight();
            };

            let actualHeight = calculateModeHeight();

            const createResizableColumn = function (col) {
                // Add a resizer element to the column
                const resizer = document.createElement('div');
                resizer.classList.add(resizerClass);

                // Set the height
                resizer.style.height = `${actualHeight}px`;

                col.appendChild(resizer);

                // Track the current position of mouse
                let x = 0;
                let w = 0;

                const mouseDownHandler = function (e) {
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
                    resizer.classList.remove(resizingClass);

                    table.querySelectorAll(`.${resizerClass}`).forEach(x => x.style.height = `${calculateModeHeight()}px`);

                    document.removeEventListener('pointermove', mouseMoveHandler);
                    document.removeEventListener('pointerup', mouseUpHandler);
                };

                resizer.addEventListener('pointerdown', mouseDownHandler);
            };


            [].forEach.call(cols, function (col) {
                createResizableColumn(col);
            });
        }
    },
    destroyResizable: function (table) {
        table.querySelectorAll('.b-datagrid-resizer').forEach(x => x.remove());
    }
}; 