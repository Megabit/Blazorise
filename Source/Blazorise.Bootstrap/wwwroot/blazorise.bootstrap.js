if (!window.blazoriseBootstrap) {
    window.blazoriseBootstrap = {};
}

window.blazoriseBootstrap = {
    tooltip: {
        initialize: (element, elementId, options) => {
            window.blazorise.tooltip.initialize(element, elementId, options);

            if (element.querySelector(".custom-control-input,.btn")) {
                element.classList.add("b-tooltip-inline");
            }
        }
    },
    modal: {
        open: (element, scrollToTop) => {
            var modals = Number(document.body.getAttribute("data-modals") || "0");

            if (modals === 0) {
                window.blazorise.addClassToBody("modal-open");
            }

            modals += 1;

            document.body.setAttribute("data-modals", modals.toString());

            if (scrollToTop) {
                element.querySelector('.modal-body').scrollTop = 0;
            }
        },
        close: (element) => {
            var modals = Number(document.body.getAttribute("data-modals") || "0");

            modals -= 1;

            if (modals < 0) {
                modals = 0;
            }

            if (modals === 0) {
                window.blazorise.removeClassFromBody("modal-open");
            }

            document.body.setAttribute("data-modals", modals.toString());
        }
    }
};