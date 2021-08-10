if (!window.blazoriseBootstrap) {
    window.blazoriseBootstrap = {};
}

window.blazoriseBootstrap = {
    tooltip: {
        initialize: (element, elementId, options) => {
            window.blazorise.tooltip.initialize(element, elementId, options);

            if (element && element.querySelector(".custom-control-input,.btn")) {
                element.classList.add("b-tooltip-inline");
            }
        }
    },
    modal: {
        open: (element, scrollToTop) => {
            // adjust modal and page padding BEFORE modal is shown
            window.blazoriseBootstrap.modal.adjustDialog(element);

            var modals = Number(document.body.getAttribute("data-modals") || "0");

            if (modals === 0) {
                window.blazorise.addClassToBody("modal-open");
            }

            modals += 1;

            document.body.setAttribute("data-modals", modals.toString());

            if (scrollToTop) {
                element.querySelector('.modal-body').scrollTop = 0;
            }
        },
        close: (element) => {
            var modals = Number(document.body.getAttribute("data-modals") || "0");

            modals -= 1;

            if (modals < 0) {
                modals = 0;
            }

            if (modals === 0) {
                window.blazorise.removeClassFromBody("modal-open");
            }

            document.body.setAttribute("data-modals", modals.toString());

            window.blazoriseBootstrap.modal.resetAdjustments(element);
        },
        adjustDialog: (element) => {
            if (element) {
                const rect = document.body.getBoundingClientRect();
                const isBodyOverflowing = Math.round(rect.left + rect.right) < window.innerWidth;
                const scrollbarWidth = window.blazoriseBootstrap.modal.getScrollBarWidth();

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
        },
        resetAdjustments: (element) => {
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
        },
        getScrollBarWidth: () => {
            const documentWidth = document.documentElement.clientWidth;
            return Math.abs(window.innerWidth - documentWidth);
        }
    }
};