window.blazorise.ButtonInfo = function (element, elementId, preventDefaultOnSubmit) {
    this.elementId = elementId;
    this.element = element;
    this.preventDefaultOnSubmit = preventDefaultOnSubmit;
};

window.blazorise.button = {
    _instances: [],

    initialize: (element, elementId, preventDefaultOnSubmit) => {
        window.blazorise.button._instances[elementId] = new window.blazorise.ButtonInfo(element, elementId, preventDefaultOnSubmit);

        if (element.type === "submit") {
            element.addEventListener("click", (e) => {
                window.blazorise.button.click(window.blazorise.button._instances[elementId], e);
            });
        }
    },
    destroy: (elementId) => {
        var instances = window.blazorise.button._instances || {};
        delete instances[elementId];
    },
    click: (buttonInfo, e) => {
        if (buttonInfo.preventDefaultOnSubmit) {
            return e.preventDefault();
        }
    }
};
