window.blazorise.textEdit = {
    _instances: [],

    initialize: (element, elementId, maskType, editMask) => {
        var instances = window.blazorise.textEdit._instances = window.blazorise.textEdit._instances || {};

        if (maskType === "numeric") {
            instances[elementId] = new window.blazorise.NumericMaskValidator(element, elementId);
        }
        else if (maskType === "datetime") {
            instances[elementId] = new window.blazorise.DateTimeMaskValidator(element, elementId);
        }
        else if (maskType === "regex") {
            instances[elementId] = new window.blazorise.RegExMaskValidator(element, elementId, editMask);
        }
        else {
            instances[elementId] = new window.blazorise.NoValidator();
        }

        element.addEventListener("keypress", (e) => {
            window.blazorise.textEdit.keyPress(instances[elementId], e);
        });

        element.addEventListener("paste", (e) => {
            window.blazorise.textEdit.paste(instances[elementId], e);
        });
    },
    destroy: (element, elementId) => {
        var instances = window.blazorise.textEdit._instances || {};
        delete instances[elementId];
    },
    keyPress: (validator, e) => {
        var currentValue = String.fromCharCode(e.which);

        return validator.isValid(currentValue) || e.preventDefault();
    },
    paste: (validator, e) => {
        return validator.isValid(e.clipboardData.getData("text/plain")) || e.preventDefault();
    }
};