"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.addVariable = addVariable;

function addVariable(name, value) {
  var themeVariablesElement = document.getElementById("b-theme-variables"); // make sure that themeVariables element exists and that we don't have the variable already defined

  if (themeVariablesElement && themeVariablesElement.innerHTML) {
    var newVariable = "\n" + name + ": " + value + ";";
    var variableStartIndex = themeVariablesElement.innerHTML.indexOf(name + ":");

    if (variableStartIndex >= 0) {
      var variableEndIndex = themeVariablesElement.innerHTML.indexOf(";", variableStartIndex);
      var existingVariable = themeVariablesElement.innerHTML.substr(variableStartIndex, variableEndIndex);
      var result = themeVariablesElement.innerHTML.replace(existingVariable, newVariable);
      themeVariablesElement.innerHTML = result;
    } else {
      var innerHTML = themeVariablesElement.innerHTML;
      var position = innerHTML.lastIndexOf(';');

      if (position >= 0) {
        var _result = [innerHTML.slice(0, position + 1), newVariable, innerHTML.slice(position + 1)].join('');

        themeVariablesElement.innerHTML = _result;
      }
    }

    return;
  } // The fallback mechanism for custom CSS variables where we don't use theme provider
  // is to apply them to the body element


  document.body.style.setProperty(name, value);
}