if (!window.blazoriseBootstrap) {
    window.blazoriseBootstrap = {};
}

window.blazoriseBootstrap = {
    tooltip: {
        initialize: (element, elementId) => {
            if (element.querySelector(".custom-control-input,.btn")) {
                element.classList.add("b-tooltip-inline");
            }

            return true;
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

            return true;
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

            return true;
        }
    }
    //activateDatePicker: (elementId, formatSubmit) => {
    //    const element = $(`#${elementId}`);

    //    element.datepicker({
    //        uiLibrary: 'bootstrap4',
    //        format: 'yyyy-mm-dd',
    //        showOnFocus: true,
    //        showRightIcon: true,
    //        select: function (e, type) {
    //            // trigger onchange event on the DateEdit component
    //            mutateDOMChange(elementId);
    //        }
    //    });
    //    return true;
    //}
};

function mutateDOMChange(id) {
    el = document.getElementById(id);
    ev = document.createEvent('Event');
    ev.initEvent('change', true, false);
    el.dispatchEvent(ev);
}