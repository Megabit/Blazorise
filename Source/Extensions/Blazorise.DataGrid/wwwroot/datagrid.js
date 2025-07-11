import { getRequiredElement } from "../Blazorise/utilities.js?v=1.8.0.0";

const QUERYSELECTOR_ALL_COLUMNS = "tbody tr td";
const QUERYSELECTOR_ALL_TABLE_HEAD_INPUT = "tbody tr td";
const TAG_NAME_TABLE = "TABLE";
export function initialize(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (element) {
        var headerInputs = element.querySelectorAll(QUERYSELECTOR_ALL_TABLE_HEAD_INPUT);
        headerInputs.forEach(input => {
            input.addEventListener("keypress", keyPressPreventSubmitOnEnter);
        });
    }
}

export function initializeTableCellNavigation(element, elementId) {

    element = getRequiredElement(element, elementId);

    element.addEventListener("click", clickCellNavigation);
    element.addEventListener("keydown", KeyDownCellNavigation)
}

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

export function destroy(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (element) {
        element.removeEventListener("click", clickCellNavigation);
        element.removeEventListener("keydown", KeyDownCellNavigation)

        var headerInputs = element.querySelectorAll(QUERYSELECTOR_ALL_TABLE_HEAD_INPUT);
        headerInputs.forEach(input => {
            input.removeEventListener("keypress", keyPressPreventSubmitOnEnter);
        });
    }
}
function preventSubmitOnEnter(e) {
    if (e.keyCode == 13) {
        e.preventDefault();
    }
}

function keyPressPreventSubmitOnEnter(e) {
    preventSubmitOnEnter(e);
}

function findAncestorByTagName(el, tagName) {
    while ((el = el.parentElement) && el.tagName !== tagName);
    return el;
}

function clickCellNavigation(e) {

    let element = findAncestorByTagName(e.target, TAG_NAME_TABLE);

    let allCells = element.querySelectorAll(QUERYSELECTOR_ALL_COLUMNS);
    if (!allCells || allCells.length == 0) {
        return;
    }

    let index = [].indexOf.call(allCells, e.target);
    if (index >= 0) {
        let toFocus = allCells[index];
        if (toFocus && toFocus.getAttribute("tabindex") == 0) {
            toFocus.focus({ focusVisible: true });
            return;
        }
    }
}
function KeyDownCellNavigation(e) {

    let element = findAncestorByTagName(e.target, TAG_NAME_TABLE);

    if (!element) {
        return;
    }
    const TAG_NAMES_INPUT = ["INPUT", "SELECT", "TEXTAREA"];
    const TAG_NAME_TABLE_COLUMN = "TD";
    const QUERYSELECTOR_FIRST_ROW_COLUMNS = "tbody tr:first-child td";

    let isLeft = e.keyCode == 37;
    let isUp = e.keyCode == 38;
    let isRight = e.keyCode == 39;
    let isDown = e.keyCode == 40;
    let isArrow = isLeft | isUp | isRight | isDown;
    let isEnterKey = e.keyCode == 13;
    let isEscKey = e.keyCode == 27;

    let focusedElement = document.activeElement;
    let allCells = element.querySelectorAll(QUERYSELECTOR_ALL_COLUMNS);

    if (!allCells || allCells.length == 0) {
        return;
    }

    let index = [].indexOf.call(allCells, focusedElement);
    let isInputFocused = focusedElement && TAG_NAMES_INPUT.includes(focusedElement.tagName);

    if (isInputFocused && (isEnterKey || isEscKey)) {
        focusedElement.addEventListener("blur", () => {
            window.setTimeout(() => {
                let inputStillExists = element.contains(focusedElement);

                if (!inputStillExists) {

                    let tdElement = findAncestorByTagName(focusedElement, TAG_NAME_TABLE_COLUMN);

                    let index = [].indexOf.call(allCells, tdElement);
                    let toFocus = element.querySelectorAll(QUERYSELECTOR_ALL_COLUMNS)[index - 1];
                    if (toFocus && toFocus.getAttribute("tabindex") == 0) {
                        toFocus.focus();
                        return;
                    }
                }
            }, 50);

        });
    }

    if (index == -1) {
        if (isArrow && !isInputFocused) {
            while (index < allCells.length - 1) {
                let toFocus = allCells[index + 1];
                if (toFocus.getAttribute("tabindex") == 0) {
                    toFocus.focus();
                    return;
                }
                index += 1;
            }
        }
        return;
    }

    if (isArrow) {
        e.preventDefault();
    }


    if (isLeft) {
        while (index > 0) {
            let toFocus = allCells[index - 1];
            if (toFocus.getAttribute("tabindex") == 0) {
                toFocus.focus();
                return;
            }
            index -= 1;
        }

        return;
    }

    if (isUp) {
        let rowCount = element.querySelectorAll(QUERYSELECTOR_FIRST_ROW_COLUMNS).length;
        let toFocus = allCells[index - rowCount];
        if (toFocus && toFocus.getAttribute("tabindex") == 0) {
            toFocus.focus();
        }
        return;
    }

    if (isRight) {

        while (index < allCells.length - 1) {
            let toFocus = allCells[index + 1];
            if (toFocus.getAttribute("tabindex") == 0) {
                toFocus.focus();
                return;
            }
            index += 1;
        }

        return;
    }

    if (isDown) {
        let rowCount = element.querySelectorAll(QUERYSELECTOR_FIRST_ROW_COLUMNS).length;
        let toFocus = allCells[index + rowCount];
        if (toFocus && toFocus.getAttribute("tabindex") == 0) {
            toFocus.focus();
        }
        return;
    }


}