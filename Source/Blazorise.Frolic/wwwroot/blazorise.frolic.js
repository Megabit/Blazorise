if (!window.blazoriseFrolic) {
    window.blazoriseFrolic = {};
}

window.blazoriseFrolic = {
    tooltip: {
        initialize: (elementId, element) => {

            if (element.querySelector(".e-btn")) {
                element.classList.add("b-tooltip-inline");
            }

            return true;
        }
    },
    modal: {
        open: (element, elementId) => {
            element.querySelector('.e-modal-body').scrollTop = 0;

            return true;
        },
        close: (element, elementId) => {
            return true;
        }
    }
};
