import { addClassToBody, removeClassFromBody } from "../Blazorise/utilities.js";

export function open(element, scrollToTop) {
    // adjust modal and page padding BEFORE modal is shown
    adjustDialog(element);

    var modals = Number(document.body.getAttribute("data-modals") || "0");

    if (modals === 0) {
        addClassToBody("modal-open");
    }

    modals += 1;

    document.body.setAttribute("data-modals", modals.toString());

    if (scrollToTop) {
        const modalBody = element.querySelector('.modal-body');

        if (modalBody) {
            modalBody.scrollTop = 0;
        }
    }
}

export function close(element) {
    var modals = Number(document.body.getAttribute("data-modals") || "0");

    modals -= 1;

    if (modals < 0) {
        modals = 0;
    }

    if (modals === 0) {
        removeClassFromBody("modal-open");
    }

    document.body.setAttribute("data-modals", modals.toString());

    resetAdjustments(element);
}

export function adjustDialog(element) {
    if (element) {
        const rect = document.body.getBoundingClientRect();
        const isBodyOverflowing = Math.round(rect.left + rect.right) < window.innerWidth;
        const scrollbarWidth = getScrollBarWidth();

        if (isBodyOverflowing) {
            const fixedContent = [].slice.call(document.querySelectorAll('.fixed-top, .fixed-bottom, .is-fixed, .sticky-top'));
            const stickyContent = [].slice.call(document.querySelectorAll('.sticky-top'));

            // Adjust fixed content padding
            if (fixedContent) {
                fixedContent.forEach((fixedContentElement) => {
                    const calculatedPadding = fixedContentElement.style.paddingRight;

                    fixedContentElement.style.paddingRight = `${parseFloat(calculatedPadding) + scrollbarWidth}px`;
                });
            }

            // Adjust sticky content margin
            if (stickyContent) {
                stickyContent.forEach((stickyContentElement) => {
                    const calculatedMargin = stickyContentElement.style.marginRight;

                    stickyContentElement.style.marginRight = `${parseFloat(calculatedMargin) - scrollbarWidth}px`;
                });
            }

            // Adjust body padding
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
export function resetAdjustments(element) {
    // Restore element padding
    if (element && element.style) {
        element.style.paddingLeft = ''
        element.style.paddingRight = '';
    }

    const fixedContent = [].slice.call(document.querySelectorAll('.fixed-top, .fixed-bottom, .is-fixed, .sticky-top'));
    const stickyContent = [].slice.call(document.querySelectorAll('.sticky-top'));

    // Restore fixed content padding
    if (fixedContent) {
        fixedContent.forEach((fixedContentElement) => {
            fixedContentElement.style.paddingRight = '';
        });
    }

    // Restore sticky content
    if (stickyContent) {
        stickyContent.forEach((stickyContentElement) => {
            stickyContentElement.style.marginRight = '';
        });
    }

    // Restore body padding
    document.body.style.paddingRight = '';
}

export function getScrollBarWidth() {
    const documentWidth = document.documentElement.clientWidth;
    return Math.abs(window.innerWidth - documentWidth);
}