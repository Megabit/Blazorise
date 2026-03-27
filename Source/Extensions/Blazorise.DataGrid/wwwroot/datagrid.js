import { getRequiredElement } from "../Blazorise/utilities.js?v=2.0.3.0";

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

function findScrollContainer(table) {
    if (!table)
        return null;

    const candidates = [];

    // Virtualize renders its own scroll container element under tbody when virtualization is enabled.
    const virtualizeContainer = table.querySelector("tbody > div[style*='overflow']");
    if (virtualizeContainer)
        candidates.push(virtualizeContainer);

    if (table.parentElement)
        candidates.push(table.parentElement);

    let ancestor = table.parentElement ? table.parentElement.parentElement : null;
    while (ancestor && candidates.length < 3) {
        candidates.push(ancestor);
        ancestor = ancestor.parentElement;
    }

    for (const candidate of candidates) {
        if (!candidate)
            continue;

        const style = getComputedStyle(candidate);
        const overflowY = style ? style.overflowY : null;
        if (overflowY === "auto" || overflowY === "scroll")
            return candidate;
    }

    return table.parentElement;
}

export function scrollVirtualizedRowIntoView(table, elementId, rowIndex) {
    table = getRequiredElement(table, elementId);

    if (!table || rowIndex < 0)
        return;

    const container = findScrollContainer(table);
    if (!container)
        return;

    const renderedRows = table.querySelectorAll("tbody tr[data-row-index]");
    if (!renderedRows || renderedRows.length === 0)
        return;

    let totalHeight = 0;
    let minIndex = Number.POSITIVE_INFINITY;
    let maxIndex = Number.NEGATIVE_INFINITY;

    for (const row of renderedRows) {
        totalHeight += row.offsetHeight;

        const indexAttr = row.getAttribute("data-row-index");
        if (!indexAttr)
            continue;

        const parsedIndex = parseInt(indexAttr, 10);
        if (Number.isNaN(parsedIndex))
            continue;

        if (parsedIndex < minIndex)
            minIndex = parsedIndex;

        if (parsedIndex > maxIndex)
            maxIndex = parsedIndex;

        if (parsedIndex === rowIndex) {
            row.scrollIntoView({ block: "nearest" });
            return;
        }
    }

    const averageRowHeight = totalHeight / renderedRows.length;
    if (!averageRowHeight || !Number.isFinite(minIndex) || !Number.isFinite(maxIndex))
        return;

    const direction = rowIndex < minIndex ? -1 : 1;
    const boundaryIndex = direction < 0 ? minIndex : maxIndex;
    const distance = Math.max(1, Math.abs(rowIndex - boundaryIndex));
    const delta = direction * distance * averageRowHeight;

    container.scrollTop += delta;
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

export function blurActiveCellEditor(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element) {
        return;
    }

    const activeElement = document.activeElement;
    if (!activeElement || !element.contains(activeElement)) {
        return;
    }

    if (typeof activeElement.blur === "function") {
        activeElement.blur();
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

function isFocusableCell(cell) {
    return cell && cell.getAttribute("tabindex") == 0;
}

function focusCell(cell) {
    if (!isFocusableCell(cell))
        return false;

    cell.focus({ focusVisible: true });
    return true;
}

function getBodyRows(table) {
    return Array.from(table.querySelectorAll("tbody tr"));
}

function getRowCells(row) {
    return row ? Array.from(row.querySelectorAll("td")) : [];
}

function getNavigableCells(table) {
    return Array.from(table.querySelectorAll(QUERYSELECTOR_ALL_COLUMNS)).filter(isFocusableCell);
}

function findClosestNavigableCell(rowCells, preferredIndex) {
    if (!rowCells || rowCells.length === 0)
        return null;

    const startIndex = Math.max(0, Math.min(preferredIndex, rowCells.length - 1));

    for (let offset = 0; offset < rowCells.length; offset++) {
        const leftIndex = startIndex - offset;
        if (leftIndex >= 0 && isFocusableCell(rowCells[leftIndex]))
            return rowCells[leftIndex];

        const rightIndex = startIndex + offset;
        if (offset > 0 && rightIndex < rowCells.length && isFocusableCell(rowCells[rightIndex]))
            return rowCells[rightIndex];
    }

    return null;
}

function getPageJumpRowCount(table, rows) {
    if (!rows || rows.length === 0)
        return 1;

    const container = findScrollContainer(table);
    if (!container)
        return 1;

    let totalHeight = 0;
    let measuredRows = 0;

    for (const row of rows) {
        const rowHeight = row.offsetHeight;
        if (!rowHeight)
            continue;

        totalHeight += rowHeight;
        measuredRows += 1;
    }

    if (measuredRows === 0)
        return 1;

    const averageRowHeight = totalHeight / measuredRows;
    const containerHeight = container.clientHeight || table.parentElement?.clientHeight || 0;

    if (!averageRowHeight || !containerHeight)
        return 1;

    return Math.max(1, Math.floor(containerHeight / averageRowHeight));
}

function clickCellNavigation(e) {
    // Do not hijack clicks coming from interactive elements
    const interactiveClosest = e.target.closest('input,select,textarea,button,label,a,[role="button"],[role="checkbox"],[contenteditable="true"]');
    if (interactiveClosest) {
        return;
    }

    let element = findAncestorByTagName(e.target, TAG_NAME_TABLE);

    let allCells = element.querySelectorAll(QUERYSELECTOR_ALL_COLUMNS);
    if (!allCells || allCells.length == 0) {
        return;
    }

    // If the click was inside a TD (e.g., nested spans), resolve the containing cell first
    const targetCell = e.target.closest('td');
    let index = [].indexOf.call(allCells, targetCell ?? e.target);

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

    let isLeft = e.key === "ArrowLeft" || e.keyCode == 37;
    let isUp = e.key === "ArrowUp" || e.keyCode == 38;
    let isRight = e.key === "ArrowRight" || e.keyCode == 39;
    let isDown = e.key === "ArrowDown" || e.keyCode == 40;
    let isPageUp = e.key === "PageUp" || e.keyCode == 33;
    let isPageDown = e.key === "PageDown" || e.keyCode == 34;
    let isEnd = e.key === "End" || e.keyCode == 35;
    let isHome = e.key === "Home" || e.keyCode == 36;
    let isArrow = isLeft || isUp || isRight || isDown;
    let isNavigationKey = isArrow || isPageUp || isPageDown || isHome || isEnd;
    let isEnterKey = e.keyCode == 13;
    let isEscKey = e.keyCode == 27;

    let focusedElement = document.activeElement;
    let allCells = Array.from(element.querySelectorAll(QUERYSELECTOR_ALL_COLUMNS));
    let navigableCells = getNavigableCells(element);
    let focusedCell = focusedElement?.tagName === TAG_NAME_TABLE_COLUMN
        ? focusedElement
        : focusedElement?.closest(TAG_NAME_TABLE_COLUMN.toLowerCase());

    if (!allCells || allCells.length == 0 || !navigableCells || navigableCells.length == 0) {
        return;
    }

    let index = focusedCell ? allCells.indexOf(focusedCell) : -1;
    let isInputFocused = focusedElement && TAG_NAMES_INPUT.includes(focusedElement.tagName);

    if (isInputFocused && (isEnterKey || isEscKey)) {
        focusedElement.addEventListener("blur", () => {
            window.setTimeout(() => {
                let inputStillExists = element.contains(focusedElement);

                if (!inputStillExists) {

                    let tdElement = findAncestorByTagName(focusedElement, TAG_NAME_TABLE_COLUMN);
                    let tdIndex = tdElement ? allCells.indexOf(tdElement) : -1;
                    let toFocus = tdIndex > 0 ? allCells[tdIndex - 1] : null;
                    if (focusCell(toFocus)) {
                        return;
                    }
                }
            }, 50);

        });
    }

    if (isInputFocused) {
        return;
    }

    if (index == -1) {
        if (isNavigationKey) {
            let initialCell = isEnd ? navigableCells[navigableCells.length - 1] : navigableCells[0];
            focusCell(initialCell);
        }
        return;
    }

    if (isNavigationKey) {
        e.preventDefault();
    }

    let currentRow = focusedCell?.closest("tr");
    let rows = getBodyRows(element).filter(row => getRowCells(row).some(isFocusableCell));
    let rowIndex = currentRow ? rows.indexOf(currentRow) : -1;
    let rowCells = getRowCells(currentRow);
    let cellIndex = rowCells.indexOf(focusedCell);
    let navigableCellIndex = navigableCells.indexOf(focusedCell);

    if (rowIndex < 0 || cellIndex < 0 || navigableCellIndex < 0) {
        return;
    }

    if (isLeft) {
        focusCell(navigableCells[navigableCellIndex - 1]);

        return;
    }

    if (isUp) {
        let targetRowIndex = Math.max(0, rowIndex - 1);
        let toFocus = findClosestNavigableCell(getRowCells(rows[targetRowIndex]), cellIndex);
        focusCell(toFocus);
        return;
    }

    if (isRight) {
        focusCell(navigableCells[navigableCellIndex + 1]);

        return;
    }

    if (isDown) {
        let targetRowIndex = Math.min(rows.length - 1, rowIndex + 1);
        let toFocus = findClosestNavigableCell(getRowCells(rows[targetRowIndex]), cellIndex);
        focusCell(toFocus);
        return;
    }

    if (isPageUp) {
        let pageJump = getPageJumpRowCount(element, rows);
        let targetRowIndex = Math.max(0, rowIndex - pageJump);
        let toFocus = findClosestNavigableCell(getRowCells(rows[targetRowIndex]), cellIndex);
        focusCell(toFocus);
        return;
    }

    if (isPageDown) {
        let pageJump = getPageJumpRowCount(element, rows);
        let targetRowIndex = Math.min(rows.length - 1, rowIndex + pageJump);
        let toFocus = findClosestNavigableCell(getRowCells(rows[targetRowIndex]), cellIndex);
        focusCell(toFocus);
        return;
    }

    if (isHome) {
        let toFocus = e.ctrlKey || e.metaKey
            ? navigableCells[0]
            : rowCells.find(isFocusableCell);
        focusCell(toFocus);
        return;
    }

    if (isEnd) {
        let toFocus = e.ctrlKey || e.metaKey
            ? navigableCells[navigableCells.length - 1]
            : [...rowCells].reverse().find(isFocusableCell);
        focusCell(toFocus);
        return;
    }
}