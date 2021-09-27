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
    },
    modal: {
        open: (element, scrollToTop) => {
            if (scrollToTop) {
                const modalBody = element.querySelector('.ant-modal-body');

                if (modalBody) {
                    modalBody.scrollTop = 0;
                }
            }
        },
        close: (element) => {
        }
    }
};