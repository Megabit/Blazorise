if (!window.blazoriseFrolic) {
    window.blazoriseFrolic = {};
}

window.blazoriseFrolic = {
    tooltip: {
        initialize: function (element, elementId) {
            if (element.querySelector(".e-btn")) {
                element.classList.add("b-tooltip-inline");
            }

            return true;
        }
    },
    modal: {
        open: function (element, elementId, scrollToTop) {
            if (scrollToTop) {
                element.querySelector('.e-modal-body').scrollTop = 0;
            }

            return true;
        },
        close: function (element, elementId) {
            return true;
        }
    }
};
