if (!window.blazoriseBootstrap) {
    window.blazoriseBootstrap = {};
}

window.blazoriseBootstrap = {
    tooltip: {
        initialize: (element, elementId, options) => {
            window.blazorise.tooltip.initialize(element, elementId, options);

            if (element && element.querySelector(".custom-control-input,.btn")) {
                element.classList.add("b-tooltip-inline");
            }
        }
    }
};