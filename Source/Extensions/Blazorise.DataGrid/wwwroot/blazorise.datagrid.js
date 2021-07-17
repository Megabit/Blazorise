window.blazoriseDataGrid = {
    virtualize: {
        onEditSetScroll: function (table, rowUnselectedClass) {
            let allTr = table.querySelectorAll("tbody tr");
            let scrollTo = table.querySelector("tbody > div").offsetHeight;
            for (let i = 0; i < allTr.length; i++) {
                let tr = allTr[i];

                if (tr.classList.contains(rowUnselectedClass))
                    scrollTo += tr.offsetHeight;
                else {
                    break;
                }
            }

            table.parentElement.scrollTop = scrollTo;
            return scrollTo;
        }
    }
};