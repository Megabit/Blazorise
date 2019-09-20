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
    }
};
