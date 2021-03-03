if (!window.blazoriseFrolic) {
    window.blazoriseFrolic = {};
}

window.blazoriseFrolic = {
    tooltip: {
        initialize: (element, elementId) => {

            if (element.querySelector(".e-btn")) {
                element.classList.add("b-tooltip-inline");
            }

            return true;
        }
    },
    modal: {
        open: (element, scrollToTop) => {
            if (scrollToTop) {
                element.querySelector('.e-modal-body').scrollTop = 0;
            }

            return true;
        },
        close: (element) => {
            return true;
        }
    }
};
