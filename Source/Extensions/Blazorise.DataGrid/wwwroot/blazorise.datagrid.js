window.blazoriseDataGrid = {
    virtualize: {
        scrollToTop: function (table) {
            if (table !== null) {
                table.parentElement.scrollTop = 0;
            }
        },
        isolateElement: function (table) {
            let elementTr; 
            var bodyHeight = table.parentElement.offsetHeight - table.querySelector("thead").offsetHeight;


            let allTr = document.querySelectorAll("table tbody tr");
            let scrollTo = 0;
            for (let i = 0; i < allTr.length; i++) {
                let tr = allTr[i];
            
                if (tr.classList.contains("table-row-selectable"))
                    scrollTo += tr.offsetHeight;
                else {
                    elementTr = tr;
                    break;
                }
            }

            
            let rows = elementTr.querySelectorAll(".row");
            let formRow;
            let minusHeight = Number.parseInt(window.getComputedStyle(elementTr.querySelector("td")).paddingTop.split("px")[0]);
            for (let i = 0; i < rows.length; i++) {
                let row = rows[i];
                let isFormRow = row.querySelector(".form-row");
                if (isFormRow) {
                    formRow = isFormRow;
                } else {
                    minusHeight += row.offsetHeight;
                }
            }

            console.log(bodyHeight);
            console.log(minusHeight);
            formRow.style.overflowY = "scroll";
            formRow.style.maxHeight = (bodyHeight - minusHeight) + "px";

            table.parentElement.scrollTop = scrollTo;
            table.parentElement.style.overflowY = "hidden";
        },
        reset: function (table) {
            table.parentElement.style.overflowY = "scroll";
        }
    }
}; 