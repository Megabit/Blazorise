if (!window.blazoriseBulma) {
    window.blazoriseBulma = {};
}

window.blazoriseBulma = {
    tooltip: {
        initialize: (elementId, element) => {
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
    }
};
