import "./vendors/flatpickr.js";
import * as utilities from "./utilities.js";

const _pickers = [];

export function initialize(element, elementId, options) {
    element = utilities.getRequiredElement(element, elementId);

    if (!element)
        return;

    function mutationObserverCallback(mutationsList, observer) {
        mutationsList.forEach(mutation => {
            if (mutation.attributeName === 'class') {
                const picker = _pickers[mutation.target.id];

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
        locale: options.localization || {
            firstDayOfWeek: options.firstDayOfWeek
        },
        time_24hr: options.timeAs24hr ? options.timeAs24hr : false,
        clickOpens: !(options.readOnly || false),
        disable: options.disabledDates || []
    };

    const pluginOptions = options.inputMode === 2 ? {
        plugins: [new monthSelectPlugin({
            shorthand: false,
            dateFormat: "Y-m-d",
            altFormat: "M Y"
        })]
    } : {};

    const picker = flatpickr(element, Object.assign({}, defaultOptions, pluginOptions));

    if (options) {
        picker.altInput.disabled = options.disabled || false;
        picker.altInput.readOnly = options.readOnly || false;
    }

    _pickers[elementId] = picker;
}

export function destroy(element, elementId) {
    const instances = _pickers || {};
    delete instances[elementId];
}

export function updateValue(element, elementId, value) {
    const picker = _pickers[elementId];

    if (picker) {
        picker.setDate(value);
    }
}

export function updateOptions(element, elementId, options) {
    const picker = _pickers[elementId];

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

        if (options.disabled.changed) {
            picker.altInput.disabled = options.disabled.value;
        }

        if (options.readOnly.changed) {
            picker.altInput.readOnly = options.readOnly.value;
            picker.set("clickOpens", !options.readOnly.value);
        }

        if (options.disabledDates.changed) {
            picker.set("disable", options.disabledDates.value || []);
        }
    }
}

export function open(element, elementId) {
    const picker = _pickers[elementId];

    if (picker) {
        picker.open();
    }
}

export function close(element, elementId) {
    const picker = _pickers[elementId];

    if (picker) {
        picker.close();
    }
}

export function toggle(element, elementId) {
    const picker = _pickers[elementId];

    if (picker) {
        picker.toggle();
    }
}

export function updateLocalization(element, elementId, localization) {
    const picker = _pickers[elementId];

    if (picker) {
        picker.config.locale = localization;

        if (picker.l10n) {
            picker.l10n.months = localization.months;
            picker.l10n.weekdays = localization.weekdays;
            picker.l10n.amPM = localization.amPM;
        }

        if (picker.weekdayContainer) {
            for (let i = 0; i < 7; ++i) {
                picker.weekdayContainer.children[0].children[i].innerHtml = localization.weekdays.shorthand[i];
                picker.weekdayContainer.children[0].children[i].innerText = localization.weekdays.shorthand[i];
            }
        }

        if (picker.amPM) {
            const selectedDate = picker.selectedDates && picker.selectedDates.length > 0 ? picker.selectedDates[0] : null;
            const index = selectedDate && selectedDate.getHours() >= 12 ? 1 : 0;

            picker.amPM.innerHtml = localization.amPM[index];
            picker.amPM.innerText = localization.amPM[index];
        }

        picker.redraw();
    }
}

export function focus(element, elementId, scrollToElement) {
    const picker = _pickers[elementId];

    if (picker && picker.altInput) {
        utilities.focus(picker.altInput, null, scrollToElement);
    }
}

export function select(element, elementId, focus) {
    const picker = _pickers[elementId];

    if (picker && picker.altInput) {
        utilities.select(picker.altInput, null, focus);
    }
}