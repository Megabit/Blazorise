function getElement(element) {
    return element ?? null;
}

function applyVerticalPopupPlacement(popup) {
    if (!popup) {
        return;
    }

    popup.style.top = '';

    const viewportMargin = 16;
    const rect = popup.getBoundingClientRect();
    const viewportBottom = window.innerHeight - viewportMargin;
    const overflowBottom = rect.bottom - viewportBottom;

    if (overflowBottom <= 0) {
        return;
    }

    const targetTop = Math.max(viewportMargin, rect.top - overflowBottom);
    const offsetY = targetTop - rect.top;

    popup.style.top = `${offsetY}px`;
}

export function updatePopupPlacement(element, visible) {
    const popup = getElement(element);

    if (!popup) {
        return;
    }

    if (!visible) {
        popup.style.top = '';
        return;
    }

    requestAnimationFrame(() => applyVerticalPopupPlacement(popup));
}

export function resetPopupPlacement(element) {
    const popup = getElement(element);

    if (!popup) {
        return;
    }

    popup.style.top = '';
}