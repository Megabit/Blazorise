window.blazorise.numericEdit = {
    _instances: [],

    initialize: (dotnetAdapter, element, elementId, options) => {
        const instance = new window.blazorise.NumericMaskValidator(dotnetAdapter, element, elementId, options);

        window.blazorise.numericEdit._instances[elementId] = instance;

        element.addEventListener("keypress", (e) => {
            window.blazorise.numericEdit.keyPress(window.blazorise.numericEdit._instances[elementId], e);
        });

        element.addEventListener("paste", (e) => {
            window.blazorise.numericEdit.paste(window.blazorise.numericEdit._instances[elementId], e);
        });

        if (instance.decimals && instance.decimals !== 2) {
            instance.truncate();
        }
    },
    update: (element, elementId, options) => {
        const instance = window.blazorise.numericEdit._instances[elementId];

        if (instance) {
            instance.update(options);
        }
    },
    destroy: (element, elementId) => {
        var instances = window.blazorise.numericEdit._instances || {};
        delete instances[elementId];
    },
    keyPress: (validator, e) => {
        var currentValue = String.fromCharCode(e.which);

        return e.which === 13 // still need to allow ENTER key so that we don't preventDefault on form submit
            || validator.isValid(currentValue)
            || e.preventDefault();
    },
    paste: (validator, e) => {
        return validator.isValid(e.clipboardData.getData("text/plain")) || e.preventDefault();
    }
};