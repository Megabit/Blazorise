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
    return Array.from(element.querySelectorAll(focusableSelector))
        .filter(isFocusable)
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

function isFocusable(element) {
    if (!element || element.closest("[inert]") || element.getAttribute("aria-hidden") === "true") {
        return false;
    }

    const style = window.getComputedStyle(element);

    if (style.visibility === "hidden" || style.display === "none") {
        return false;
    }

    return element.tabIndex >= 0 && element.getClientRects().length > 0;
}

function focusElement(element) {
    if (element && typeof element.focus === "function") {
        element.focus({ preventScroll: true });
    }
}

function resolveElement(element, elementId) {
    return element || document.getElementById(elementId);
}