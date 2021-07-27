window.blazorise.datePicker = {
    _pickers: [],

    initialize: (element, elementId, options) => {
        function mutationObserverCallback(mutationsList, observer) {
            mutationsList.forEach(mutation => {
                if (mutation.attributeName === 'class') {
                    const picker = window.blazorise.datePicker._pickers[mutation.target.id];

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

        const defaultOptions = {
            enableTime: options.inputMode === 1,
            dateFormat: options.inputMode === 1 ? 'Y-m-d H:i' : 'Y-m-d',
            allowInput: true,
            altInput: true,
            altFormat: options.displayFormat ? options.displayFormat : (options.inputMode === 1 ? 'Y-m-d H:i' : 'Y-m-d'),
            defaultValue: options.default,
            minDate: options.min,
            maxDate: options.max,
            locale: {
                firstDayOfWeek: options.firstDayOfWeek
            },
            time_24hr: options.timeAs24hr ? options.timeAs24hr : false
        };

        const pluginOptions = options.inputMode === 2 ? {
            plugins: [new monthSelectPlugin({
                shorthand: false,
                dateFormat: "Y-m-d",
                altFormat: "M Y"
            })]
        } : {};

        const picker = flatpickr(element, {
            ...defaultOptions,
            ...pluginOptions
        });

        window.blazorise.datePicker._pickers[elementId] = picker;
    },

    destroy: (element, elementId) => {
        const instances = window.blazorise.datePicker._pickers || {};
        delete instances[elementId];
    },

    updateValue: (element, elementId, value) => {
        const picker = window.blazorise.datePicker._pickers[elementId];

        if (picker) {
            picker.setDate(value);
        }
    },

    updateOptions: (element, elementId, options) => {
        const picker = window.blazorise.datePicker._pickers[elementId];

        if (picker) {
            if (options.firstDayOfWeek.changed) {
                picker.set("firstDayOfWeek", options.firstDayOfWeek.value);
            }

            if (options.displayFormat.changed) {
                picker.set("altFormat", options.displayFormat.value);
            }

            if (options.timeAs24hr.changed) {
                picker.set("time_24hr", options.timeAs24hr.value);
            }

            if (options.min.changed) {
                picker.set("minDate", options.min.value);
            }

            if (options.max.changed) {
                picker.set("maxDate", options.max.value);
            }
        }
    },

    open: (element, elementId) => {
        const picker = window.blazorise.datePicker._pickers[elementId];

        if (picker) {
            picker.open();
        }
    },

    close: (element, elementId) => {
        const picker = window.blazorise.datePicker._pickers[elementId];

        if (picker) {
            picker.close();
        }
    },

    toggle: (element, elementId) => {
        const picker = window.blazorise.datePicker._pickers[elementId];

        if (picker) {
            picker.toggle();
        }
    }
};