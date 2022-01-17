export function DateTimeMaskValidator(element, elementId) {
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
}