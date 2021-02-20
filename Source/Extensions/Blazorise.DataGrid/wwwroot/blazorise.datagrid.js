window.blazoriseDataGrid = {
    initResizable: function (table) {
        const cols = table.querySelectorAll('th');
        const tableHeadRows = table.querySelector('thead').querySelectorAll('tr').length;

        const calculateActualHeight = function () {
            let height = 0;
            const tableRows = table.querySelectorAll('tr');

            tableRows.forEach(x => {
                let firstCol = x.querySelector('th:first-child,td:first-child').offsetHeight;
                if (firstCol != null) {
                    height += firstCol;
                }
            });
            return height; 
        };
        let actualHeight = calculateActualHeight();

        const createResizableColumn = function (col) {
            // Add a resizer element to the column
            const resizer = document.createElement('div');
            resizer.classList.add('b-datagrid-resizer');

            // Set the height
            resizer.style.top = `-${col.offsetHeight * (tableHeadRows - 1)}px`;
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

                resizer.classList.add('b-datagrid-resizing');
            };

            const mouseMoveHandler = function (e) {
                // Determine how far the mouse has been moved
                const dx = e.clientX - x;

                // Update the width of column
                col.style.width = `${w + dx}px`;
            };

            // When user releases the mouse, remove the existing event listeners
            const mouseUpHandler = function () {
                resizer.classList.remove('b-datagrid-resizing');

                document.removeEventListener('pointermove', mouseMoveHandler);
                document.removeEventListener('pointerup', mouseUpHandler);

                // Recalculate resizer height
                window.blazoriseDataGrid.destroyResizable(table);
                window.blazoriseDataGrid.initResizable(table);
            };

            resizer.addEventListener('pointerdown', mouseDownHandler);
        };

        [].forEach.call(cols, function (col) {
            createResizableColumn(col);
        });
    },
    destroyResizable: function (table) {
        table.querySelectorAll('.b-datagrid-resizer').forEach(x => x.remove());
    }
}; 