export function RegExMaskValidator(element, elementId, editMask) {
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
}