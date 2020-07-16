if (!window.blazoriseBootstrap) {
    window.blazoriseBootstrap = {};
}

window.blazoriseBootstrap = {
    tooltip: {
        initialize: function (element, elementId) {
            if (element.querySelector(".custom-control-input,.btn")) {
                element.classList.add("b-tooltip-inline");
            }

            return true;
        }
    },
    modal: {
        open: function (element, elementId, scrollToTop) {
            window.blazorise.addClassToBody("modal-open");

            if (scrollToTop) {
                element.querySelector('.modal-body').scrollTop = 0;
            }

            return true;
        },
        close: function (element, elementId) {
            window.blazorise.removeClassFromBody("modal-open");

            return true;
        }
    }
    //activateDatePicker: (elementId, formatSubmit) {
    //    var element = $('#' + elementId);

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