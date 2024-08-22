import { initialize as baseInitialize, destroy, updateContent } from "../Blazorise/tooltip.js?v=1.6.1.0";

export function initialize(element, elementId, options) {
    baseInitialize(element, elementId, options);

    if (options.autodetectInline && element && element.querySelector(".custom-control-input,.btn")) {
        element.classList.add("b-tooltip-inline");
    }
}

export { destroy, updateContent };