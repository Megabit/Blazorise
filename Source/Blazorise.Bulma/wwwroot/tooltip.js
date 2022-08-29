import { initialize as baseInitialize, destroy, updateContent } from "../Blazorise/tooltip.js?v=1.0.6.0";

export function initialize(element, elementId, options) {
    baseInitialize(element, elementId, options);

    if (options.autodetectInline && element) {
        if (element.querySelector(".checkbox,.button")) {
            element.classList.add("b-tooltip-inline");
        }

        if (element.parentElement && element.parentElement.classList.contains("field-body")) {
            element.parentElement.style.cssText = "display: block;";
        }
    }
}

export { destroy, updateContent };