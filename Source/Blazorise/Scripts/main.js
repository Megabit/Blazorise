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
    link: {
        scrollIntoView: (elementId) => {
            var element = document.getElementById(elementId);

            if (element) {
                element.scrollIntoView();
                window.location.hash = elementId;
            }
        }
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

