if (!window.blazoriseBulma) {
    window.blazoriseBulma = {};
}

window.blazoriseBulma = {
    tooltip: {
        initialize: (element, elementId, options) => {
            window.blazorise.tooltip.initialize(element, elementId, options);

            if (element) {
                if (element.querySelector(".checkbox,.button")) {
                    element.classList.add("b-tooltip-inline");
                }

                if (element.parentElement && element.parentElement.classList.contains("field-body")) {
                    element.parentElement.style.cssText = "display: block;";
                }
            }
        }
    },
    activateDatePicker: (elementId) => {
        return true;
    },
    modal: {
        open: (element, scrollToTop) => {
            if (scrollToTop) {
                const modalBody = element.querySelector('.modal-card-body');

                if (modalBody) {
                    modalBody.scrollTop = 0;
                }
            }
        },
        close: (element) => {
        }
    }
};
