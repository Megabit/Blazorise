"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.NumericMaskValidator = NumericMaskValidator;

function NumericMaskValidator(dotnetAdapter, element, elementId, options) {
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
  this.changeTextOnKeyPress = options.changeTextOnKeyPress || true;
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
    var value = this.element.value,
        selection = this.carret();

    if (value = value.substring(0, selection[0]) + currentValue + value.substring(selection[1]), !!this.regex().test(value)) {
      value = (value || "").replace(this.separator, "."); // Now that we know the number is valid we also need to make sure it can fit in the min-max range ot the TValue type.

      var number = Number(value);
      var numberOverflow = false;

      if (number > this.typeMax) {
        number = Number(this.typeMax);
        numberOverflow = true;
      } else if (number < this.typeMin) {
        number = Number(this.typeMin);
        numberOverflow = true;
      }

      if (numberOverflow) {
        value = this.fromExponential(number); // Update input with new value and also make sure that Blazor knows it is changed.

        this.element.value = value; // Trigger event so that Blazor can pick it up.

        var eventName = this.changeTextOnKeyPress ? 'input' : 'change';

        if ("createEvent" in document) {
          var event = document.createEvent("HTMLEvents");
          event.initEvent(eventName, false, true);
          this.element.dispatchEvent(event);
        } else {
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
    var eParts = this.getExponentialParts(num);
    return !Number.isNaN(Number(eParts[1]));
  };

  this.fromExponential = function (num) {
    var eParts = this.getExponentialParts(num);

    if (!this.isExponential(eParts)) {
      return eParts[0];
    }

    var sign = eParts[0][0] === '-' ? '-' : '';
    var digits = eParts[0].replace(/^-/, '');
    var digitsParts = digits.split('.');
    var wholeDigits = digitsParts[0];
    var fractionDigits = digitsParts[1] || '';
    var e = Number(eParts[1]);

    if (e === 0) {
      return "".concat(sign + wholeDigits, ".").concat(fractionDigits);
    } else if (e < 0) {
      // move dot to the left
      var countWholeAfterTransform = wholeDigits.length + e;

      if (countWholeAfterTransform > 0) {
        // transform whole to fraction
        var wholeDigitsAfterTransform = wholeDigits.substr(0, countWholeAfterTransform);
        var wholeDigitsTransformedToFraction = wholeDigits.substr(countWholeAfterTransform);
        return "".concat(sign + wholeDigitsAfterTransform, ".").concat(wholeDigitsTransformedToFraction).concat(fractionDigits);
      } else {
        // not enough whole digits: prepend with fractional zeros
        // first e goes to dotted zero
        var zeros = '0.';
        e = countWholeAfterTransform;

        while (e) {
          zeros += '0';
          e += 1;
        }

        return sign + zeros + wholeDigits + fractionDigits;
      }
    } else {
      // move dot to the right
      var countFractionAfterTransform = fractionDigits.length - e;

      if (countFractionAfterTransform > 0) {
        // transform fraction to whole
        // countTransformedFractionToWhole = e
        var fractionDigitsAfterTransform = fractionDigits.substr(e);
        var fractionDigitsTransformedToWhole = fractionDigits.substr(0, e);
        return "".concat(sign + wholeDigits + fractionDigitsTransformedToWhole, ".").concat(fractionDigitsAfterTransform);
      } else {
        // not enough fractions: append whole zeros
        var zerosCount = -countFractionAfterTransform;
        var _zeros = '';

        while (zerosCount) {
          _zeros += '0';
          zerosCount -= 1;
        }

        return sign + wholeDigits + fractionDigits + _zeros;
      }
    }
  };

  this.truncate = function () {
    var value = (this.element.value || "").replace(this.separator, ".");

    if (value) {
      var number = Number(value);
      number = Math.trunc(number * Math.pow(10, this.decimals)) / Math.pow(10, this.decimals);
      var newValue = number.toString().replace(".", this.separator);
      this.element.value = newValue;

      if (this.dotnetAdapter) {
        this.dotnetAdapter.invokeMethodAsync('SetValue', newValue);
      }
    }
  };
}