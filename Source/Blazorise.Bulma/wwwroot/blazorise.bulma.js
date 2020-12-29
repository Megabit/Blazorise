if (!window.blazoriseBulma) {
    window.blazoriseBulma = {};
}

window.blazoriseBulma = {
    tooltip: {
        initialize: (element, elementId) => {
            if (element.querySelector(".checkbox,.button")) {
                element.classList.add("b-tooltip-inline");
            }

            if (element.parentElement && element.parentElement.classList.contains("field-body")) {
                element.parentElement.style.cssText = "display: block;";
            }

            return true;
        }
    },
    activateDatePicker: (elementId) => {
        return true;
    },
    modal: {
        open: (element, scrollToTop) => {
            if (scrollToTop) {
                element.querySelector('.modal-card-body').scrollTop = 0;
            }

            return true;
        },
        close: (element) => {
            return true;
        }
    }
};
