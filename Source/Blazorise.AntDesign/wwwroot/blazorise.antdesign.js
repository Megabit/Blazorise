if (!window.antDesign) {
    window.antDesign = {};
}

window.antDesign = {
    tooltip: {
        initialize: (element, elementId) => {
            if (element.querySelector(".ant-input,.ant-btn")) {
                element.classList.add("b-tooltip-inline");
            }

            return true;
        }
    },
    modal: {
        open: (element, elementId) => {
            return true;
        },
        close: (element, elementId) => {
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