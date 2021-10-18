export function NoValidator() {
    this.isValid = function (currentValue) {
        return true;
    };
}