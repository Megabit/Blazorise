if (!window.antDesign) {
    window.antDesign = {};
}

window.antDesign = {
    tooltip: {
        initialize: (element, elementId, options) => {
            window.blazorise.tooltip.initialize(element, elementId, options);

            if (element.querySelector(".ant-input,.ant-btn")) {
                element.classList.add("b-tooltip-inline");
            }
        }
    },
    modal: {
        open: (element, scrollToTop) => {
            if (scrollToTop) {
                element.querySelector('.ant-modal-body').scrollTop = 0;
            }
        },
        close: (element) => {
        }
    }
};