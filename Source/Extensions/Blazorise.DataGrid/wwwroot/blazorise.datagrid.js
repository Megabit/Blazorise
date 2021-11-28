export function scrollTo(table, rowUnselectedClass) {
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

export function initialize(element, elementId) {
    var headerInputs = element.querySelectorAll("thead input");
    headerInputs.forEach(input => {
        input.addEventListener("keypress", (e) => {
            preventSubmitOnEnter(e);
        });

    });
}

function preventSubmitOnEnter(e) {
    if (e.keyCode == 13) {
        e.preventDefault();
    }
}
