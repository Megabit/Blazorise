import { registerDisconnectCleanup, unregisterDisconnectCleanup } from "./utilities.js?v=2.0.4.0";

const modalDisconnectCleanupProperty = "blazoriseModalDisconnectCleanupId";
const modalCountAttribute = "data-modals";

export function registerModalDisconnectCleanup(element, closeCore) {
    if (!element || typeof closeCore !== "function")
        return;

    unregisterModalDisconnectCleanup(element);

    element[modalDisconnectCleanupProperty] = registerDisconnectCleanup(element, () => {
        clearModalDisconnectCleanupHandle(element);
        closeCore();
    });
}

export function unregisterModalDisconnectCleanup(element) {
    if (!element)
        return;

    unregisterDisconnectCleanup(element[modalDisconnectCleanupProperty]);
    clearModalDisconnectCleanupHandle(element);
}

export function scrollModalBodyToTop(element, scrollToTop, selector) {
    if (!element || !scrollToTop)
        return;

    const modalBody = element.querySelector(selector);

    if (modalBody) {
        modalBody.scrollTop = 0;
    }
}

export function openStackedModal(element, options) {
    const modalOptions = options || {};

    if (typeof modalOptions.beforeOpen === "function") {
        modalOptions.beforeOpen(element);
    }

    let modals = Number(document.body.getAttribute(modalCountAttribute) || "0");

    if (modals === 0 && typeof modalOptions.onFirstModalOpen === "function") {
        modalOptions.onFirstModalOpen(element);
    }

    modals += 1;

    document.body.setAttribute(modalCountAttribute, modals.toString());

    scrollModalBodyToTop(element, modalOptions.scrollToTop, modalOptions.bodySelector || ".modal-body");
}

export function closeStackedModal(element, options) {
    const modalOptions = options || {};

    let modals = Number(document.body.getAttribute(modalCountAttribute) || "0");

    modals -= 1;

    if (modals < 0) {
        modals = 0;
    }

    if (modals === 0 && typeof modalOptions.onLastModalClose === "function") {
        modalOptions.onLastModalClose(element);
    }

    document.body.setAttribute(modalCountAttribute, modals.toString());

    if (typeof modalOptions.afterClose === "function") {
        modalOptions.afterClose(element);
    }
}

export function adjustDialogDimensionsBeforeShow(element, adjustmentSelectors) {
    if (element) {
        const rect = document.body.getBoundingClientRect();
        const isBodyOverflowing = Math.round(rect.left + rect.right) < window.innerWidth;
        const scrollbarWidth = getScrollBarWidth();
        const selectors = adjustmentSelectors || {};
        const fixedContent = queryElements(selectors.fixedContentSelector);
        const stickyContent = queryElements(selectors.stickyContentSelector);

        if (isBodyOverflowing) {
            if (fixedContent) {
                fixedContent.forEach((fixedContentElement) => {
                    const calculatedPadding = fixedContentElement.style.paddingRight;

                    fixedContentElement.style.paddingRight = `${parseFloat(calculatedPadding) + scrollbarWidth}px`;
                });
            }

            if (stickyContent) {
                stickyContent.forEach((stickyContentElement) => {
                    const calculatedMargin = stickyContentElement.style.marginRight;

                    stickyContentElement.style.marginRight = `${parseFloat(calculatedMargin) - scrollbarWidth}px`;
                });
            }

            const calculatedPadding = document.body.style.paddingRight;

            document.body.style.paddingRight = `${calculatedPadding + scrollbarWidth}px`;
        }

        const isModalOverflowing = element.scrollHeight > document.documentElement.clientHeight;

        if (!isBodyOverflowing && isModalOverflowing) {
            element.style.paddingLeft = `${scrollbarWidth}px`;
        }

        if (isBodyOverflowing && !isModalOverflowing) {
            element.style.paddingRight = `${scrollbarWidth}px`;
        }
    }
}

export function resetAdjustments(element, adjustmentSelectors) {
    if (element && element.style) {
        element.style.paddingLeft = '';
        element.style.paddingRight = '';
    }

    const selectors = adjustmentSelectors || {};
    const fixedContent = queryElements(selectors.fixedContentSelector);
    const stickyContent = queryElements(selectors.stickyContentSelector);

    if (fixedContent) {
        fixedContent.forEach((fixedContentElement) => {
            fixedContentElement.style.paddingRight = '';
        });
    }

    if (stickyContent) {
        stickyContent.forEach((stickyContentElement) => {
            stickyContentElement.style.marginRight = '';
        });
    }

    document.body.style.paddingRight = '';
}

export function getScrollBarWidth() {
    const documentWidth = document.documentElement.clientWidth;
    return Math.abs(window.innerWidth - documentWidth);
}

function clearModalDisconnectCleanupHandle(element) {
    if (element && Object.prototype.hasOwnProperty.call(element, modalDisconnectCleanupProperty)) {
        delete element[modalDisconnectCleanupProperty];
    }
}

function queryElements(selector) {
    if (!selector)
        return [];

    return [].slice.call(document.querySelectorAll(selector));
}