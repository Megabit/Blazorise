if (!window.blazoriseBulma) {
    window.blazoriseBulma = {};
}

window.blazoriseBulma = {
    tooltip: {
        initialize: function (element, elementId) {
            if (element.querySelector(".checkbox,.button")) {
                element.classList.add("b-tooltip-inline");
            }

            if (element.parentElement && element.parentElement.classList.contains("field-body")) {
                element.parentElement.style.cssText = "display: block;";
            }

            return true;
        }
    },
    activateDatePicker: function (elementId) {
        return true;
    },
    modal: {
        open: function (element, elementId, scrollToTop) {
            if (scrollToTop) {
                element.querySelector('.modal-card-body').scrollTop = 0;
            }

            return true;
        },
        close: function (element, elementId) {
            return true;
        }
    }
};
