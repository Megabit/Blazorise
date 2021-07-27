window.blazorise.timePicker = {
    _pickers: [],

    initialize: (element, elementId, options) => {
        function mutationObserverCallback(mutationsList, observer) {
            mutationsList.forEach(mutation => {
                if (mutation.attributeName === 'class') {
                    const picker = window.blazorise.timePicker._pickers[mutation.target.id];

                    if (picker && picker.altInput) {
                        const altInputClassListToRemove = [...picker.altInput.classList].filter(cn => !["input", "active"].includes(cn));
                        const inputClassListToAdd = [...picker.input.classList].filter(cn => !["flatpickr-input"].includes(cn));

                        altInputClassListToRemove.forEach(name => {
                            picker.altInput.classList.remove(name);
                        });

                        inputClassListToAdd.forEach(name => {
                            picker.altInput.classList.add(name);
                        });
                    }
                }
            });
        }

        // When flatpickr is defined with altInput=true, it will create a second input
        // element while the original input element will be hidden. With MutationObserver
        // we can copy classnames from hidden to the visible element.
        const mutationObserver = new MutationObserver(mutationObserverCallback);
        mutationObserver.observe(document.getElementById(elementId), { attributes: true });

        const picker = flatpickr(element, {
            enableTime: true,
            noCalendar: true,
            dateFormat: "H:i",
            allowInput: true,
            altInput: true,
            altFormat: options.displayFormat ? options.displayFormat : "H:i",
            defaultValue: options.default,
            minTime: options.min,
            maxTime: options.max,
            time_24hr: options.timeAs24hr ? options.timeAs24hr : false
        });

        window.blazorise.timePicker._pickers[elementId] = picker;
    },

    destroy: (element, elementId) => {
        const instances = window.blazorise.timePicker._pickers || {};
        delete instances[elementId];
    },

    updateValue: (element, elementId, value) => {
        const picker = window.blazorise.timePicker._pickers[elementId];

        if (picker) {
            picker.setDate(value);
        }
    },

    updateOptions: (element, elementId, options) => {
        const picker = window.blazorise.timePicker._pickers[elementId];

        if (picker) {
            if (options.displayFormat.changed) {
                picker.set("altFormat", options.displayFormat.value);
            }

            if (options.timeAs24hr.changed) {
                picker.set("time_24hr", options.timeAs24hr.value);
            }

            if (options.min.changed) {
                picker.set("minTime", options.min.value);
            }

            if (options.max.changed) {
                picker.set("maxTime", options.max.value);
            }
        }
    },

    open: (element, elementId) => {
        const picker = window.blazorise.timePicker._pickers[elementId];

        if (picker) {
            picker.open();
        }
    },

    close: (element, elementId) => {
        const picker = window.blazorise.timePicker._pickers[elementId];

        if (picker) {
            picker.close();
        }
    },

    toggle: (element, elementId) => {
        const picker = window.blazorise.timePicker._pickers[elementId];

        if (picker) {
            picker.toggle();
        }
    }
};