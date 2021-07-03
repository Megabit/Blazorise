window.blazoriseDataGrid = {
    virtualize: {
        scrollToTop: function (table) {
            if (table !== null) {
                table.parentElement.scrollTop = 0;
            }
        }
    }
}; 