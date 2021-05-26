if (!window.blazoriseFrolic) {
    window.blazoriseFrolic = {};
}

window.blazoriseFrolic = {
    tooltip: {
        initialize: (element, elementId, options) => {
            window.blazorise.tooltip.initialize(element, elementId, options);

            if (element.querySelector(".e-btn")) {
                element.classList.add("b-tooltip-inline");
            }
        }
    },
    modal: {
        open: (element, scrollToTop) => {
            if (scrollToTop) {
                element.querySelector('.e-modal-body').scrollTop = 0;
            }
        },
        close: (element) => {
        }
    }
};
