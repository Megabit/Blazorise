if (!window.blazoriseMaterial) {
    window.blazoriseMaterial = {};
}

window.blazoriseMaterial = {
    tooltip: {
        initialize: (element, elementId) => {
            if (element.querySelector(".custom-control-input,.btn")) {
                element.classList.add("b-tooltip-inline");
            }

            return true;
        }
    },
    activateDatePicker: (elementId, formatSubmit) => {
        const element = $(`#${elementId}`);

        element.pickdate({
            ok: '',
            cancel: 'Clear',
            today: 'Today',
            closeOnCancel: true,
            closeOnSelect: true,
            container: 'body',
            containerHidden: 'body',
            firstDay: 1, // monday
            format: 'dd.mm.yyyy',
            selectMonths: true,
            selectYears: true,
            formatSubmit: formatSubmit,
            onClose: function (s) {
                // trigger onchange event on the DateEdit component
                mutateDOMChange(elementId);
            }
        });
        return true;
    },
    modal: {
        open: (element, scrollToTop) => {
            window.blazorise.addClassToBody("modal-open");

            if (scrollToTop) {
                element.querySelector('.modal-body').scrollTop = 0;
            }

            return true;
        },
        close: (element) => {
            window.blazorise.removeClassFromBody("modal-open");

            return true;
        }
    }
};

function mutateDOMChange(id) {
    el = document.getElementById(id);
    ev = document.createEvent('Event');
    ev.initEvent('change', true, false);
    el.dispatchEvent(ev);
}
