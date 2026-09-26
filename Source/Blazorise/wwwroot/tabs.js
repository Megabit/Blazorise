import { getRequiredElement } from "./utilities.js?v=2.3.2.0";

export function initialize(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const tabList = getTabList(element);

    if (!tabList)
        return;

    const tabs = getTabs(tabList);
    const activeTab = tabs.find(tab => tab.getAttribute("aria-selected") === "true") || tabs[0];
    const focusedTab = tabs.find(tab => tab === document.activeElement);

    setTabStop(tabList, focusedTab || activeTab);

    element.addEventListener("keydown", onKeyDown);
    element.addEventListener("focusout", onFocusOut);
}

export function destroy(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (element) {
        element.removeEventListener("keydown", onKeyDown);
        element.removeEventListener("focusout", onFocusOut);
    }
}

function getTabList(element) {
    return element.matches('[role="tablist"]') ? element : element.querySelector('[role="tablist"]');
}

function getTabs(tabList) {
    return Array.from(tabList.querySelectorAll('[role="tab"]')).filter(tab =>
        tab.closest('[role="tablist"]') === tabList
        && !tab.hasAttribute("disabled")
        && tab.getAttribute("aria-disabled") !== "true");
}

function setTabStop(tabList, focusedTab) {
    for (const tab of tabList.querySelectorAll('[role="tab"]')) {
        if (tab.closest('[role="tablist"]') === tabList)
            tab.tabIndex = tab === focusedTab ? 0 : -1;
    }
}

function onFocusOut(event) {
    const tabList = getTabList(event.currentTarget);

    if (!tabList)
        return;

    const tabs = getTabs(tabList);

    // Return to the selected tab when the user next enters the tab list.
    if (tabs.includes(event.target) && !tabs.includes(event.relatedTarget))
        setTabStop(tabList, tabs.find(tab => tab.getAttribute("aria-selected") === "true") || tabs[0]);
}

function onKeyDown(event) {
    if (event.altKey || event.ctrlKey || event.metaKey || event.shiftKey)
        return;

    const tabList = getTabList(event.currentTarget);

    if (!tabList)
        return;

    const tabs = getTabs(tabList).filter(tab =>
        tab.getClientRects().length > 0 && getComputedStyle(tab).visibility !== "hidden");
    const index = tabs.indexOf(event.target);

    // Ignore events from tab content, nested tab groups, and controls inside a tab.
    if (index < 0)
        return;

    const vertical = tabList.getAttribute("aria-orientation") === "vertical";
    const rtl = getComputedStyle(tabList).direction === "rtl";
    let nextIndex = index;

    switch (event.key) {
        case "ArrowLeft":
        case "ArrowRight":
            if (vertical)
                return;
            nextIndex += (event.key === "ArrowRight") !== rtl ? 1 : -1;
            break;
        case "ArrowUp":
        case "ArrowDown":
            if (!vertical)
                return;
            nextIndex += event.key === "ArrowDown" ? 1 : -1;
            break;
        case "Home":
            nextIndex = 0;
            break;
        case "End":
            nextIndex = tabs.length - 1;
            break;
        case "Enter":
        case " ":
            event.preventDefault();
            event.target.click();
            return;
        default:
            return;
    }

    event.preventDefault();

    const nextTab = tabs[(nextIndex + tabs.length) % tabs.length];
    setTabStop(tabList, nextTab);
    nextTab.focus();
}