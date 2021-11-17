import { initialize as baseInitialize, destroy, updateContent } from "/_content/Blazorise/tooltip.js";

export function initialize(element, elementId, options) {
    baseInitialize(element, elementId, options);

    if (element) {
        if (element.querySelector(".checkbox,.button")) {
            element.classList.add("b-tooltip-inline");
        }

        if (element.parentElement && element.parentElement.classList.contains("field-body")) {
            element.parentElement.style.cssText = "display: block;";
        }
    }
}

export { destroy, updateContent };