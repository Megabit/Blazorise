if (!window.antDesign) {
    window.antDesign = {};
}

window.antDesign = {
    tooltip: {
        initialize: (element, elementId, options) => {
            window.blazorise.tooltip.initialize(element, elementId, options);

            if (element && element.querySelector(".ant-input,.ant-btn")) {
                element.classList.add("b-tooltip-inline");
            }
        }
    }
};