export function NumericMaskValidator(dotnetAdapter, element, elementId, options) {
    options = options || {};

    this.dotnetAdapter = dotnetAdapter;
    this.elementId = elementId;
    this.element = element;
    this.decimals = options.decimals === null || options.decimals === undefined ? 2 : options.decimals;
    this.separator = options.separator || ".";
    this.step = options.step || 1;
    this.min = options.min;
    this.max = options.max;
    this.typeMin = options.typeMin;
    this.typeMax = options.typeMax;
    this.immediate = options.immediate || true;
    this.selectAllOnFocus = options.selectAllOnFocus || false;

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
        let value = this.element.value,
            selection = this.carret();

        if (value = value.substring(0, selection[0]) + currentValue + value.substring(selection[1]), !!this.regex().test(value)) {

            value = (value || "").replace(this.separator, ".");

            // Now that we know the number is valid we also need to make sure it can fit in the min-max range ot the TValue type.
            let number = Number(value);
            let numberOverflow = false;

            if (number > this.typeMax) {
                number = Number(this.typeMax);

                numberOverflow = true;
            }
            else if (number < this.typeMin) {
                number = Number(this.typeMin);

                numberOverflow = true;
            }

            if (numberOverflow) {
                value = this.fromExponential(number);

                // Update input with new value and also make sure that Blazor knows it is changed.
                this.element.value = value;

                // Trigger event so that Blazor can pick it up.
                let eventName = this.immediate ? 'input' : 'change';

                if ("createEvent" in document) {
                    let event = document.createEvent("HTMLEvents");
                    event.initEvent(eventName, false, true);
                    this.element.dispatchEvent(event);
                }
                else {
                    this.element.fireEvent("on" + eventName);
                }

                return false; // This will make it fail the validation and do the preventDefault().
            }

            return value;
        }

        return false;
    };
    this.update = function (options) {
        if (options.decimals && options.decimals.changed) {
            this.decimals = options.decimals.value;

            this.truncate();
        }
    };
    this.getExponentialParts = function (num) {
        return Array.isArray(num) ? num : String(num).split(/[eE]/);
    };
    this.isExponential = function (num) {
        const eParts = this.getExponentialParts(num);
        return !Number.isNaN(Number(eParts[1]));
    };
    this.fromExponential = function (num) {
        const eParts = this.getExponentialParts(num);
        if (!this.isExponential(eParts)) {
            return eParts[0];
        }

        const sign = eParts[0][0] === '-' ? '-' : '';
        const digits = eParts[0].replace(/^-/, '');
        const digitsParts = digits.split('.');
        const wholeDigits = digitsParts[0];
        const fractionDigits = digitsParts[1] || '';
        let e = Number(eParts[1]);

        if (e === 0) {
            return `${sign + wholeDigits}.${fractionDigits}`;
        } else if (e < 0) {
            // move dot to the left
            const countWholeAfterTransform = wholeDigits.length + e;
            if (countWholeAfterTransform > 0) {
                // transform whole to fraction
                const wholeDigitsAfterTransform = wholeDigits.substr(0, countWholeAfterTransform);
                const wholeDigitsTransformedToFraction = wholeDigits.substr(countWholeAfterTransform);
                return `${sign + wholeDigitsAfterTransform}.${wholeDigitsTransformedToFraction}${fractionDigits}`;
            } else {
                // not enough whole digits: prepend with fractional zeros

                // first e goes to dotted zero
                let zeros = '0.';
                e = countWholeAfterTransform;
                while (e) {
                    zeros += '0';
                    e += 1;
                }
                return sign + zeros + wholeDigits + fractionDigits;
            }
        } else {
            // move dot to the right
            const countFractionAfterTransform = fractionDigits.length - e;
            if (countFractionAfterTransform > 0) {
                // transform fraction to whole
                // countTransformedFractionToWhole = e
                const fractionDigitsAfterTransform = fractionDigits.substr(e);
                const fractionDigitsTransformedToWhole = fractionDigits.substr(0, e);
                return `${sign + wholeDigits + fractionDigitsTransformedToWhole}.${fractionDigitsAfterTransform}`;
            } else {
                // not enough fractions: append whole zeros
                let zerosCount = -countFractionAfterTransform;
                let zeros = '';
                while (zerosCount) {
                    zeros += '0';
                    zerosCount -= 1;
                }
                return sign + wholeDigits + fractionDigits + zeros;
            }
        }
    };
    this.truncate = function () {
        let value = (this.element.value || "").replace(this.separator, ".");

        if (value) {
            let number = Number(value);

            number = Math.trunc(number * Math.pow(10, this.decimals)) / Math.pow(10, this.decimals);

            let newValue = number.toString().replace(".", this.separator);

            this.element.value = newValue;

            if (this.dotnetAdapter) {
                this.dotnetAdapter.invokeMethodAsync('SetValue', newValue);
            }
        }
    };
}