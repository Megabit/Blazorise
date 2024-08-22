import "./vendors/flatpickr.js?v=1.6.1.0";
import * as utilities from "./utilities.js?v=1.6.1.0";
import * as inputmask from "./inputMask.js?v=1.6.1.0";
import { ClassWatcher } from "./observer.js?v=1.6.1.0";

const _pickers = [];

export function initialize(dotnetAdapter, element, elementId, options) {
    element = utilities.getRequiredElement(element, elementId);

    if (!element)
        return;

    function mutationObserverCallback(mutationsList, observer) {
        mutationsList.forEach(mutation => {
            if (mutation.attributeName === 'class') {
                if (mutation.target.id) {
                    // remove the special hidden id that we did before
                    const targetId = mutation.target.id.replace("flatpickr_hidden_", "");
                    const picker = _pickers[targetId];

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
            }
        });
    }

    // When flatpickr is defined with altInput=true, it will create a second input
    // element while the original input element will be hidden. With MutationObserver
    // we can copy classnames from hidden to the visible element.
    const mutationObserver = new MutationObserver(mutationObserverCallback);
    mutationObserver.observe(document.getElementById(elementId), { attributes: true });

    const disableDatesOptions = options.disabledDates || [];
    const disableDaysOptions = options.disabledDays ? [function (date) {
        return options.disabledDays && options.disabledDays.length > 0 && options.disabledDays.includes(date.getDay());
    }] : [];

    const defaultOptions = {
        enableTime: options.inputMode === 1,
        dateFormat: options.inputMode === 1 ? 'Y-m-d H:i' : 'Y-m-d',
        allowInput: true,
        altInput: true,
        altFormat: options.displayFormat ? options.displayFormat : (options.inputMode === 1 ? 'Y-m-d H:i' : 'Y-m-d'),
        defaultDate: options.defaultDate,
        minDate: options.min,
        maxDate: options.max,
        locale: options.localization || {
            firstDayOfWeek: options.firstDayOfWeek
        },
        time_24hr: options.timeAs24hr ? options.timeAs24hr : false,
        clickOpens: !(options.readOnly || false),
        disable: disableDatesOptions.concat(disableDaysOptions),
        inline: options.inline || false,
        disableMobile: options.disableMobile || true,
        static: options.staticPicker,
        errorHandler: (error) => {
            // do nothing to prevent warnings in the console
        },
        onReady: (selectedDates, dateStr, instance) => {
            // move the id from the hidden element to the visible element
            if (instance && instance.input && instance.input.parentElement) {
                const id = instance.input.id;
                const input = instance.input.parentElement.querySelector(".input");
                if (id && input) {
                    instance.input.id = "flatpickr_hidden_" + id;
                    input.id = id;
                }
            }
        }
    };

    if (options.selectionMode)
        defaultOptions.mode = options.selectionMode;

    const pluginOptions = options.inputMode === 2 ? {
        plugins: [new monthSelectPlugin({
            shorthand: false,
            dateFormat: "Y-m-d",
            altFormat: "M Y"
        })]
    } : {};

    const picker = flatpickr(element, Object.assign({}, defaultOptions, pluginOptions));

    picker.altInput.dotnetAdapter = dotnetAdapter;

    if (options) {
        picker.altInput.disabled = options.disabled || false;
        picker.altInput.readOnly = options.readOnly || false;
        picker.altInput.placeholder = utilities.coalesce(options.placeholder, "");

        picker.altInput.addEventListener("blur", (e) => {
            const isInput = e.target === picker._input;

            // Workaround for: onchange does not fire when user writes the time and then click outside of the input area.
            if (isInput && picker.isOpen === false) {
                picker.input.dispatchEvent(utilities.createEvent("change"));
                picker.input.dispatchEvent(utilities.createEvent("input"));
            }
        });

        if (options.inputFormat) {
            setInputMask(picker, options.inputFormat, options.placeholder);
        }

        if (options.validationStatus) {
            const flatpickrWrapper = picker.altInput.parentElement;

            if (flatpickrWrapper) {
                if (options.validationStatus.errorClass) {
                    function errorClassAddHandler() {
                        flatpickrWrapper.classList.add(options.validationStatus.errorClass);
                    }

                    function errorClassRemoveHandler() {
                        flatpickrWrapper.classList.remove(options.validationStatus.errorClass);
                    }

                    picker.errorClassWatcher = new ClassWatcher(picker.altInput, options.validationStatus.errorClass, errorClassAddHandler, errorClassRemoveHandler);
                }

                if (options.validationStatus.successClass) {
                    function successClassAddHandler() {
                        flatpickrWrapper.classList.add(options.validationStatus.successClass);
                    }

                    function successClassRemoveHandler() {
                        flatpickrWrapper.classList.remove(options.validationStatus.successClass);
                    }

                    picker.successClassWatcher = new ClassWatcher(picker.altInput, options.validationStatus.successClass, successClassAddHandler, successClassRemoveHandler);
                }
            }
        }
    }

    picker.customOptions = {
        inputMode: options.inputMode
    };

    attachEventHandlers(picker.altInput);

    _pickers[elementId] = picker;
}

function attachEventHandlers(picker) {
    picker.addEventListener("keydown", keyDownHandler);
    picker.addEventListener("keyup", keyUpHandler);
    picker.addEventListener("focus", focusHandler);
    picker.addEventListener("focusin", focusInHandler);
    picker.addEventListener("focusout", focusOutHandler);
    picker.addEventListener("keypress", keyPressHandler);
    picker.addEventListener("blur", blurHandler);
}

function removeEventHandlers(picker) {
    picker.removeEventListener("keydown", keyDownHandler);
    picker.removeEventListener("keyup", keyUpHandler);
    picker.removeEventListener("focus", focusHandler);
    picker.removeEventListener("focusin", focusInHandler);
    picker.removeEventListener("focusout", focusOutHandler);
    picker.removeEventListener("keypress", keyPressHandler);
    picker.removeEventListener("blur", blurHandler);
}

function keyDownHandler(e) {
    if (e.target.dotnetAdapter) {
        e.target.dotnetAdapter.invokeMethodAsync("OnKeyDownHandler", e);
    }
}

function keyUpHandler(e) {
    if (e.target.dotnetAdapter) {
        e.target.dotnetAdapter.invokeMethodAsync("OnKeyUpHandler", e);
    }
}

function focusHandler(e) {
    if (e.target.dotnetAdapter) {
        e.target.dotnetAdapter.invokeMethodAsync("OnFocusHandler", e);
    }
}

function focusInHandler(e) {
    if (e.target.dotnetAdapter) {
        e.target.dotnetAdapter.invokeMethodAsync("OnFocusInHandler", e);
    }
}

function focusOutHandler(e) {
    if (e.target.dotnetAdapter) {
        e.target.dotnetAdapter.invokeMethodAsync("OnFocusOutHandler", e);
    }
}

function keyPressHandler(e) {
    if (e.target.dotnetAdapter) {
        e.target.dotnetAdapter.invokeMethodAsync("OnKeyPressHandler", e);
    }
}

function blurHandler(e) {
    if (e.target.dotnetAdapter) {
        e.target.dotnetAdapter.invokeMethodAsync("OnBlurHandler", e);
    }
}

export function destroy(element, elementId) {
    const instances = _pickers || {};

    const instance = instances[elementId];

    if (instance && instance.altInput) {
        removeEventHandlers(instance.altInput);
    }

    if (instance) {
        if (instance.errorClassWatcher) {
            instance.errorClassWatcher.disconnect();
        }

        if (instance.successClassWatcher) {
            instance.successClassWatcher.disconnect();
        }

        instance.destroy();
    }

    delete instances[elementId];
}

export function updateValue(element, elementId, value) {
    const picker = _pickers[elementId];

    if (picker) {
        picker.setDate(value);

        // workaround for https://github.com/flatpickr/flatpickr/issues/2861
        if (picker.customOptions && picker.customOptions.inputMode === 2 && picker.nextMonthNav) {
            picker.nextMonthNav.click();
            picker.jumpToDate(value, false);
        }
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

        if (options.inputFormat.changed) {
            setInputMask(picker, options.inputFormat.value, options.placeholder.value);
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

        if (options.disabledDates.changed || options.disabledDays.changed) {
            const disableDatesOptions = options.disabledDates.value || [];
            const disableDaysOptions = options.disabledDays.value ? [function (date) {
                return options.disabledDays.value && options.disabledDays.value.length > 0 && options.disabledDays.value.includes(date.getDay());
            }] : [];

            picker.set("disable", disableDatesOptions.concat(disableDaysOptions));
        }

        if (options.selectionMode.changed) {
            picker.set("mode", options.selectionMode.value);
        }

        if (options.inline.changed) {
            picker.set("inline", options.inline.value || false);
        }

        if (options.disableMobile.changed) {
            picker.set("disableMobile", options.disableMobile.value || true);
        }

        if (options.placeholder.changed) {
            picker.altInput.placeholder = utilities.coalesce(options.placeholder.value, "");
        }

        if (options.staticPicker.changed) {
            picker.set("static", options.staticPicker.value);
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
            picker.l10n.rangeSeparator = localization.rangeSeparator;
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

function setInputMask(picker, inputFormat, placeholder) {
    if (picker && picker.altInput) {
        if (picker.inputMask && picker.inputMask.remove) {
            picker.inputMask.remove();
        }

        picker.inputMask = inputmask.initialize(null, picker.altInput, null, {
            placeholder: utilities.coalesce(placeholder, inputFormat),
            alias: "datetime",
            inputFormat: inputFormat
        });
    }
}