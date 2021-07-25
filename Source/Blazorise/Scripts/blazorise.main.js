if (!window.blazorise) {
    window.blazorise = {};
}

window.blazorise = {
    lastClickedDocumentElement: null,

    utils: {
        getRequiredElement: (element, elementId) => {
            if (element)
                return element;

            return document.getElementById(elementId);
        }
    },

    // adds a classname to the specified element
    addClass: (element, classname) => {
        element.classList.add(classname);
    },

    // removes a classname from the specified element
    removeClass: (element, classname) => {
        if (element.classList.contains(classname)) {
            element.classList.remove(classname);
        }
    },

    // toggles a classname on the given element id
    toggleClass: (element, classname) => {
        if (element) {
            if (element.classList.contains(classname)) {
                element.classList.remove(classname);
            } else {
                element.classList.add(classname);
            }
        }
    },

    // adds a classname to the body element
    addClassToBody: (classname) => {
        blazorise.addClass(document.body, classname);
    },

    // removes a classname from the body element
    removeClassFromBody: (classname) => {
        blazorise.removeClass(document.body, classname);
    },

    // indicates if parent node has a specified classname
    parentHasClass: (element, classname) => {
        if (element && element.parentElement) {
            return element.parentElement.classList.contains(classname);
        }
        return false;
    },

    // sets the value to the element property
    setProperty: (element, property, value) => {
        if (element && property) {
            element[property] = value;
        }
    },

    getElementInfo: (element, elementId) => {
        if (!element) {
            element = document.getElementById(elementId);
        }

        if (element) {
            const position = element.getBoundingClientRect();

            return {
                boundingClientRect: {
                    x: position.x,
                    y: position.y,
                    top: position.top,
                    bottom: position.bottom,
                    left: position.left,
                    right: position.right,
                    width: position.width,
                    height: position.height
                },
                offsetTop: element.offsetTop,
                offsetLeft: element.offsetLeft,
                offsetWidth: element.offsetWidth,
                offsetHeight: element.offsetHeight,
                scrollTop: element.scrollTop,
                scrollLeft: element.scrollLeft,
                scrollWidth: element.scrollWidth,
                scrollHeight: element.scrollHeight,
                clientTop: element.clientTop,
                clientLeft: element.clientLeft,
                clientWidth: element.clientWidth,
                clientHeight: element.clientHeight
            };
        }

        return {};
    },

    setTextValue(element, value) {
        element.value = value;
    },

    hasSelectionCapabilities: (element) => {
        const nodeName = element && element.nodeName && element.nodeName.toLowerCase();

        return (
            nodeName &&
            ((nodeName === 'input' &&
                (element.type === 'text' ||
                    element.type === 'search' ||
                    element.type === 'tel' ||
                    element.type === 'url' ||
                    element.type === 'password')) ||
                nodeName === 'textarea' ||
                element.contentEditable === 'true')
        );
    },

    setCaret: (element, caret) => {
        if (window.blazorise.hasSelectionCapabilities(element)) {
            window.requestAnimationFrame(() => {
                element.selectionStart = caret;
                element.selectionEnd = caret;
            });
        }
    },

    getCaret: (element) => {
        return window.blazorise.hasSelectionCapabilities(element)
            ? element.selectionStart :
            -1;
    },

    getSelectedOptions: (elementId) => {
        const element = document.getElementById(elementId);
        const len = element.options.length;
        var opts = [], opt;

        for (var i = 0; i < len; i++) {
            opt = element.options[i];

            if (opt.selected) {
                opts.push(opt.value);
            }
        }

        return opts;
    },

    setSelectedOptions: (elementId, values) => {
        const element = document.getElementById(elementId);

        if (element && element.options) {
            const len = element.options.length;

            for (var i = 0; i < len; i++) {
                const opt = element.options[i];

                if (values && values.find(x => x !== null && x.toString() === opt.value)) {
                    opt.selected = true;
                } else {
                    opt.selected = false;
                }
            }
        }
    },

    // holds the list of components that are triggers to close other components
    closableComponents: [],

    addClosableComponent: (elementId, dotnetAdapter) => {
        window.blazorise.closableComponents.push({ elementId: elementId, dotnetAdapter: dotnetAdapter });
    },

    findClosableComponent: (elementId) => {
        for (index = 0; index < window.blazorise.closableComponents.length; ++index) {
            if (window.blazorise.closableComponents[index].elementId === elementId)
                return window.blazorise.closableComponents[index];
        }
        return null;
    },

    findClosableComponentIndex: (elementId) => {
        for (index = 0; index < window.blazorise.closableComponents.length; ++index) {
            if (window.blazorise.closableComponents[index].elementId === elementId)
                return index;
        }
        return -1;
    },

    isClosableComponent: (elementId) => {
        for (index = 0; index < window.blazorise.closableComponents.length; ++index) {
            if (window.blazorise.closableComponents[index].elementId === elementId)
                return true;
        }
        return false;
    },

    registerClosableComponent: (element, dotnetAdapter) => {
        if (element) {
            if (window.blazorise.isClosableComponent(element.id) !== true) {
                window.blazorise.addClosableComponent(element.id, dotnetAdapter);
            }
        }
    },

    unregisterClosableComponent: (element) => {
        if (element) {
            const index = window.blazorise.findClosableComponentIndex(element.id);
            if (index !== -1) {
                window.blazorise.closableComponents.splice(index, 1);
            }
        }
    },

    tryClose: (closable, targetElementId, isEscapeKey, isChildClicked) => {
        let request = new Promise((resolve, reject) => {
            closable.dotnetAdapter.invokeMethodAsync('SafeToClose', targetElementId, isEscapeKey ? 'escape' : 'leave', isChildClicked)
                .then((result) => resolve({ elementId: closable.elementId, dotnetAdapter: closable.dotnetAdapter, status: result === true ? 'ok' : 'cancelled' }))
                .catch(() => resolve({ elementId: closable.elementId, status: 'error' }));
        });

        if (request) {
            request
                .then((response) => {
                    if (response.status === 'ok') {
                        response.dotnetAdapter.invokeMethodAsync('Close', isEscapeKey ? 'escape' : 'leave')
                            // If the user navigates to another page then it will raise exception because the reference to the component cannot be found.
                            // In that case just remove the elementId from the list.
                            .catch(() => window.blazorise.unregisterClosableComponent(response.elementId));
                    }
                });
        }
    },
    focus: (element, elementId, scrollToElement) => {
        element = window.blazorise.utils.getRequiredElement(element, elementId);

        if (element) {
            element.focus({
                preventScroll: !scrollToElement
            });
        }
    },
    textEdit: {
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
    },
    numericEdit: {
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
    },
    datePicker: {
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
    },

    timePicker: {
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
    },

    NoValidator: function () {
        this.isValid = function (currentValue) {
            return true;
        };
    },
    NumericMaskValidator: function (dotnetAdapter, element, elementId, options) {
        this.dotnetAdapter = dotnetAdapter;
        this.elementId = elementId;
        this.element = element;
        this.decimals = options.decimals === null || options.decimals === undefined ? 2 : options.decimals;
        this.separator = options.separator || ".";
        this.step = options.step || 1;
        this.min = options.min;
        this.max = options.max;

        this.regex = function () {
            var sep = "\\" + this.separator,
                dec = this.decimals,
                reg = "{0," + dec + "}";

            return dec ? new RegExp("^(-)?(((\\d+(" + sep + "\\d" + reg + ")?)|(" + sep + "\\d" + reg + ")))?$") : /^(-)?(\d*)$/;
        };
        this.carret = function () {
            return [this.element.selectionStart, this.element.selectionEnd];
        };
        this.isValid = function (currentValue) {
            var value = this.element.value,
                selection = this.carret();

            if (value = value.substring(0, selection[0]) + currentValue + value.substring(selection[1]), !!this.regex().test(value)) {
                return value = (value || "").replace(this.separator, ".");
            }

            return false;
        };
        this.update = function (options) {
            if (options.decimals && options.decimals.changed) {
                this.decimals = options.decimals.value;

                this.truncate();
            }
        };
        this.truncate = function () {
            let value = (this.element.value || "").replace(this.separator, ".");
            let number = Number(value);

            number = Math.trunc(number * Math.pow(10, this.decimals)) / Math.pow(10, this.decimals);

            let newValue = number.toString().replace(".", this.separator);

            this.element.value = newValue;
            this.dotnetAdapter.invokeMethodAsync('SetValue', newValue);
        };
    },
    DateTimeMaskValidator: function (element, elementId) {
        this.elementId = elementId;
        this.element = element;
        this.regex = function () {
            return /^\d{0,4}$|^\d{4}-0?$|^\d{4}-(?:0?[1-9]|1[012])(?:-(?:0?[1-9]?|[12]\d|3[01])?)?$/;
        };
        this.carret = function () {
            return [this.element.selectionStart, this.element.selectionEnd];
        };
        this.isValid = function (currentValue) {
            var value = this.element.value,
                selection = this.carret();

            return value = value.substring(0, selection[0]) + currentValue + value.substring(selection[1]), !!this.regex().test(value);
        };
    },
    RegExMaskValidator: function (element, elementId, editMask) {
        this.elementId = elementId;
        this.element = element;
        this.editMask = editMask;
        this.regex = function () {
            return new RegExp(this.editMask);
        };
        this.carret = function () {
            return [this.element.selectionStart, this.element.selectionEnd];
        };
        this.isValid = function (currentValue) {
            var value = this.element.value,
                selection = this.carret();

            return value = value.substring(0, selection[0]) + currentValue + value.substring(selection[1]), !!this.regex().test(value);
        };
    },
    button: {
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
    },
    ButtonInfo: function (element, elementId, preventDefaultOnSubmit) {
        this.elementId = elementId;
        this.element = element;
        this.preventDefaultOnSubmit = preventDefaultOnSubmit;
    },
    link: {
        scrollIntoView: (elementId) => {
            var element = document.getElementById(elementId);

            if (element) {
                element.scrollIntoView();
                window.location.hash = elementId;
            }
        }
    },
    fileEdit: {
        _instances: [],

        initialize: (adapter, element, elementId) => {
            var nextFileId = 0;

            // save an instance of adapter
            window.blazorise.fileEdit._instances[elementId] = new window.blazorise.FileEditInfo(adapter, element, elementId);

            element.addEventListener('change', function handleInputFileChange(event) {
                // Reduce to purely serializable data, plus build an index by ID
                element._blazorFilesById = {};
                var fileList = Array.prototype.map.call(element.files, function (file) {
                    var result = {
                        id: ++nextFileId,
                        lastModified: new Date(file.lastModified).toISOString(),
                        name: file.name,
                        size: file.size,
                        type: file.type
                    };
                    element._blazorFilesById[result.id] = result;

                    // Attach the blob data itself as a non-enumerable property so it doesn't appear in the JSON
                    Object.defineProperty(result, 'blob', { value: file });

                    return result;
                });

                adapter.invokeMethodAsync('NotifyChange', fileList).then(null, function (err) {
                    throw new Error(err);
                });
            });
        },
        destroy: (element, elementId) => {
            var instances = window.blazorise.fileEdit._instances || {};
            delete instances[elementId];
        },

        reset: (element, elementId) => {
            if (element) {
                element.value = '';

                var fileEditInfo = window.blazorise.fileEdit._instances[elementId];

                if (fileEditInfo) {
                    fileEditInfo.adapter.invokeMethodAsync('NotifyChange', []).then(null, function (err) {
                        throw new Error(err);
                    });
                }
            }
        },

        readFileData: function readFileData(element, fileEntryId, position, length) {
            var readPromise = getArrayBufferFromFileAsync(element, fileEntryId);

            return readPromise.then(function (arrayBuffer) {
                var uint8Array = new Uint8Array(arrayBuffer, position, length);
                var base64 = uint8ToBase64(uint8Array);
                return base64;
            });
        },

        ensureArrayBufferReadyForSharedMemoryInterop: function ensureArrayBufferReadyForSharedMemoryInterop(elem, fileId) {
            return getArrayBufferFromFileAsync(elem, fileId).then(function (arrayBuffer) {
                getFileById(elem, fileId).arrayBuffer = arrayBuffer;
            });
        },

        readFileDataSharedMemory: function readFileDataSharedMemory(readRequest) {
            // This uses various unsupported internal APIs. Beware that if you also use them,
            // your code could become broken by any update.
            var inputFileElementReferenceId = Blazor.platform.readStringField(readRequest, 0);
            var inputFileElement = document.querySelector('[_bl_' + inputFileElementReferenceId + ']');
            var fileId = Blazor.platform.readInt32Field(readRequest, 4);
            var sourceOffset = Blazor.platform.readUint64Field(readRequest, 8);
            var destination = Blazor.platform.readInt32Field(readRequest, 16);
            var destinationOffset = Blazor.platform.readInt32Field(readRequest, 20);
            var maxBytes = Blazor.platform.readInt32Field(readRequest, 24);

            var sourceArrayBuffer = getFileById(inputFileElement, fileId).arrayBuffer;
            var bytesToRead = Math.min(maxBytes, sourceArrayBuffer.byteLength - sourceOffset);
            var sourceUint8Array = new Uint8Array(sourceArrayBuffer, sourceOffset, bytesToRead);

            var destinationUint8Array = Blazor.platform.toUint8Array(destination);
            destinationUint8Array.set(sourceUint8Array, destinationOffset);

            return bytesToRead;
        },
        open: (element, elementId) => {
            if (!element && elementId) {
                element = document.getElementById(elementId);
            }

            if (element) {
                element.click();
            }
        }
    },

    FileEditInfo: function (adapter, element, elementId) {
        this.adapter = adapter;
        this.element = element;
        this.elementId = elementId;
    },

    breakpoint: {
        // Get the current breakpoint
        getBreakpoint: function () {
            return window.getComputedStyle(document.body, ':before').content.replace(/\"/g, '');
        },

        // holds the list of components that are triggers to breakpoint
        breakpointComponents: [],

        lastBreakpoint: null,

        addBreakpointComponent: (elementId, dotnetAdapter) => {
            window.blazorise.breakpoint.breakpointComponents.push({ elementId: elementId, dotnetAdapter: dotnetAdapter });
        },

        findBreakpointComponentIndex: (elementId) => {
            for (index = 0; index < window.blazorise.breakpoint.breakpointComponents.length; ++index) {
                if (window.blazorise.breakpoint.breakpointComponents[index].elementId === elementId)
                    return index;
            }
            return -1;
        },

        isBreakpointComponent: (elementId) => {
            for (index = 0; index < window.blazorise.breakpoint.breakpointComponents.length; ++index) {
                if (window.blazorise.breakpoint.breakpointComponents[index].elementId === elementId)
                    return true;
            }
            return false;
        },

        registerBreakpointComponent: (elementId, dotnetAdapter) => {
            if (window.blazorise.breakpoint.isBreakpointComponent(elementId) !== true) {
                window.blazorise.breakpoint.addBreakpointComponent(elementId, dotnetAdapter);
            }
        },

        unregisterBreakpointComponent: (elementId) => {
            const index = window.blazorise.breakpoint.findBreakpointComponentIndex(elementId);
            if (index !== -1) {
                window.blazorise.breakpoint.breakpointComponents.splice(index, 1);
            }
        },

        onBreakpoint: (dotnetAdapter, currentBreakpoint) => {
            dotnetAdapter.invokeMethodAsync('OnBreakpoint', currentBreakpoint);
        }
    }
};

