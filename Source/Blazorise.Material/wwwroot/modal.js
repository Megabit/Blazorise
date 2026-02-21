import { addClassToBody, removeClassFromBody } from "../Blazorise/utilities.js?v=2.0.0.0";

let closeCleanupTimeoutId = null;

export function open(element, scrollToTop) {
    clearCloseCleanupTimeout();
    adjustDialogDimensionsBeforeShow(element);

    var modals = Number(document.body.getAttribute("data-modals") || "0");

    if (modals === 0) {
        // Save the original overflow value
        const originalOverflow = document.body.style.overflow || '';
        document.body.setAttribute('data-original-overflow', originalOverflow);

        // Hide the scrollbar
        document.body.style.overflow = 'hidden';

        addClassToBody("modal-open");
    }

    modals += 1;

    document.body.setAttribute("data-modals", modals.toString());

    if (scrollToTop) {
        const modalBody = element.querySelector('.mui-modal-body, .modal-body');

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
        clearCloseCleanupTimeout();
        const animationDuration = getAnimationDurationInMs(element);

        closeCleanupTimeoutId = window.setTimeout(() => {
            if (Number(document.body.getAttribute("data-modals") || "0") !== 0) {
                closeCleanupTimeoutId = null;
                return;
            }

            // Restore the original overflow value
            document.body.style.overflow = document.body.getAttribute('data-original-overflow') || '';
            document.body.removeAttribute('data-original-overflow');
            removeClassFromBody("modal-open");

            resetAdjustments(element, true);
            closeCleanupTimeoutId = null;
        }, animationDuration);
    } else {
        resetAdjustments(element, false);
    }

    document.body.setAttribute("data-modals", modals.toString());
}

export function adjustDialogDimensionsBeforeShow(element) {
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
                    const calculatedPadding = window.getComputedStyle(fixedContentElement).paddingRight;

                    fixedContentElement.style.paddingRight = `${toFloat(calculatedPadding) + scrollbarWidth}px`;
                });
            }

            // Adjust sticky content margin
            if (stickyContent) {
                stickyContent.forEach((stickyContentElement) => {
                    const calculatedMargin = window.getComputedStyle(stickyContentElement).marginRight;

                    stickyContentElement.style.marginRight = `${toFloat(calculatedMargin) - scrollbarWidth}px`;
                });
            }

            // Adjust body padding
            const calculatedPadding = window.getComputedStyle(document.body).paddingRight;

            document.body.style.paddingRight = `${toFloat(calculatedPadding) + scrollbarWidth}px`;
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

export function resetAdjustments(element, resetGlobal = true) {
    // Restore element padding
    if (element && element.style) {
        element.style.paddingLeft = '';
        element.style.paddingRight = '';
    }

    if (!resetGlobal) {
        return;
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

function toFloat(value) {
    const parsed = Number.parseFloat(value);
    return Number.isFinite(parsed) ? parsed : 0;
}

function clearCloseCleanupTimeout() {
    if (closeCleanupTimeoutId !== null) {
        window.clearTimeout(closeCleanupTimeoutId);
        closeCleanupTimeoutId = null;
    }
}

function getAnimationDurationInMs(element) {
    if (!element) {
        return 0;
    }

    const computedStyle = window.getComputedStyle(element);
    const duration =
        computedStyle.getPropertyValue("--modal-animation-duration") ||
        computedStyle.getPropertyValue("transition-duration");

    return parseDurationToMs(duration);
}

function parseDurationToMs(value) {
    if (!value) {
        return 0;
    }

    const normalized = value.toString().trim();

    if (normalized.endsWith("ms")) {
        return toFloat(normalized);
    }

    if (normalized.endsWith("s")) {
        return toFloat(normalized) * 1000;
    }

    return toFloat(normalized);
}