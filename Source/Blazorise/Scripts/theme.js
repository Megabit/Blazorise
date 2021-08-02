if (!window.blazorise) {
    window.blazorise = {};
}

window.blazorise.theme = {
    addVariable: (name, value) => {
        const themeVariablesElement = document.getElementById("b-theme-variables");

        // make sure that themeVariables element exists and that we don't have the variable already defined
        if (themeVariablesElement && themeVariablesElement.innerHTML && themeVariablesElement.innerHTML.indexOf(name + ":") < 0) {
            const innerHTML = themeVariablesElement.innerHTML;
            const position = innerHTML.lastIndexOf(';');

            if (position >= 0) {
                const newVariable = "\n" + name + ": " + value + ";";

                const result = [innerHTML.slice(0, position + 1), newVariable, innerHTML.slice(position + 1)].join('');

                themeVariablesElement.innerHTML = result;

                return;
            }
        }

        // The fallback mechanism for custom CSS variables where we don't use theme provider
        // is to apply them to the body element
        document.body.style.setProperty(name, value);
    }
};

