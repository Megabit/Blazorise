export function addVariable(name, value) {
    const themeVariablesElement = document.getElementById("b-theme-variables");

    // make sure that themeVariables element exists and that we don't have the variable already defined
    if (themeVariablesElement && themeVariablesElement.innerHTML) {
        const newVariable = "\n" + name + ": " + value + ";";

        const variableStartIndex = themeVariablesElement.innerHTML.indexOf(name + ":");

        if (variableStartIndex >= 0) {
            const variableEndIndex = themeVariablesElement.innerHTML.indexOf(";", variableStartIndex);
            const existingVariable = themeVariablesElement.innerHTML.substr(variableStartIndex, variableEndIndex);

            const result = themeVariablesElement.innerHTML.replace(existingVariable, newVariable);

            themeVariablesElement.innerHTML = result;
        }
        else {
            const innerHTML = themeVariablesElement.innerHTML;
            const position = innerHTML.lastIndexOf(';');

            if (position >= 0) {
                const result = [innerHTML.slice(0, position + 1), newVariable, innerHTML.slice(position + 1)].join('');

                themeVariablesElement.innerHTML = result;
            }
        }

        return;
    }

    // The fallback mechanism for custom CSS variables where we don't use theme provider
    // is to apply them to the body element
    document.body.style.setProperty(name, value);
}