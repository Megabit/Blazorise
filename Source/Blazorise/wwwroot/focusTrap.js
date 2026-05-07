const focusTrapHandlers = new WeakMap();

const focusableSelector = [
    "a[href]",
    "area[href]",
    "button:not([disabled])",
    "input:not([disabled]):not([type='hidden'])",
    "select:not([disabled])",
    "textarea:not([disabled])",
    "iframe",
    "object",
    "embed",
    "audio[controls]",
    "video[controls]",
    "summary",
    "[contenteditable]:not([contenteditable='false'])",
    "[tabindex]:not([tabindex='-1'])"
].join(",");

export function initialize(element, elementId) {
    element = resolveElement(element, elementId);

    if (!element) {
        return;
    }

    destroy(element, elementId);

    const keydownHandler = (event) => {
        if (event.key !== "Tab" || event.defaultPrevented) {
            return;
        }

        const focusableElements = getFocusableElements(element);

        if (focusableElements.length === 0) {
            event.preventDefault();
            focusElement(element);
            return;
        }

        const firstFocusable = focusableElements[0];
        const lastFocusable = focusableElements[focusableElements.length - 1];
        const activeElement = element.ownerDocument.activeElement;

        if (!element.contains(activeElement)) {
            event.preventDefault();
            focusElement(event.shiftKey ? lastFocusable : firstFocusable);
        } else if (event.shiftKey && activeElement === firstFocusable) {
            event.preventDefault();
            focusElement(lastFocusable);
        } else if (!event.shiftKey && activeElement === lastFocusable) {
            event.preventDefault();
            focusElement(firstFocusable);
        }
    };

    element.addEventListener("keydown", keydownHandler);
    focusTrapHandlers.set(element, keydownHandler);
}

export function destroy(element, elementId) {
    element = resolveElement(element, elementId);

    if (!element) {
        return;
    }

    const keydownHandler = focusTrapHandlers.get(element);

    if (keydownHandler) {
        element.removeEventListener("keydown", keydownHandler);
        focusTrapHandlers.delete(element);
    }
}

export function focus(element, elementId) {
    element = resolveElement(element, elementId);

    if (!element) {
        return;
    }

    const focusableElements = getFocusableElements(element);
    focusElement(focusableElements[0] || element);
}

function getFocusableElements(element) {
    const focusableElements = Array.from(element.querySelectorAll(focusableSelector));

    return focusableElements
        .filter((focusableElement) => isFocusable(focusableElement, focusableElements))
        .map((focusableElement, index) => ({
            element: focusableElement,
            index,
            tabIndex: focusableElement.tabIndex
        }))
        .sort(compareTabOrder)
        .map((entry) => entry.element);
}

function compareTabOrder(left, right) {
    const leftPositive = left.tabIndex > 0;
    const rightPositive = right.tabIndex > 0;

    if (leftPositive && rightPositive && left.tabIndex !== right.tabIndex) {
        return left.tabIndex - right.tabIndex;
    }

    if (leftPositive !== rightPositive) {
        return leftPositive ? -1 : 1;
    }

    return left.index - right.index;
}

function isFocusable(element, focusableElements) {
    if (!element
        || element.closest("[inert]")
        || hasHiddenAncestor(element)
        || isDisabled(element)
        || !isTabbableRadio(element, focusableElements)) {
        return false;
    }

    const style = window.getComputedStyle(element);

    if (style.visibility === "hidden" || style.display === "none") {
        return false;
    }

    return element.tabIndex >= 0 && element.getClientRects().length > 0;
}

function hasHiddenAncestor(element) {
    for (let current = element; current; current = current.parentElement) {
        if (current.getAttribute("aria-hidden")?.toLowerCase() === "true") {
            return true;
        }
    }

    return false;
}

function isDisabled(element) {
    if (element.disabled) {
        return true;
    }

    const disabledFieldset = element.closest("fieldset[disabled]");

    if (disabledFieldset) {
        const firstLegend = Array.from(disabledFieldset.children).find((child) => child.tagName === "LEGEND");

        if (!firstLegend || !firstLegend.contains(element)) {
            return true;
        }
    }

    const closedDetails = element.closest("details:not([open])");

    if (closedDetails) {
        const firstSummary = Array.from(closedDetails.children).find((child) => child.tagName === "SUMMARY");

        if (!firstSummary || !firstSummary.contains(element)) {
            return true;
        }
    }

    return false;
}

function isTabbableRadio(element, focusableElements) {
    if (element.tagName !== "INPUT" || element.type !== "radio" || !element.name) {
        return true;
    }

    const radioGroup = focusableElements.filter((focusableElement) =>
        focusableElement.tagName === "INPUT"
        && focusableElement.type === "radio"
        && focusableElement.name === element.name
        && focusableElement.form === element.form);

    const checkedRadio = radioGroup.find((radio) => radio.checked);

    if (checkedRadio) {
        return checkedRadio === element;
    }

    return radioGroup[0] === element;
}

function focusElement(element) {
    if (element && typeof element.focus === "function") {
        element.focus({ preventScroll: true });
    }
}

function resolveElement(element, elementId) {
    return element || document.getElementById(elementId);
}