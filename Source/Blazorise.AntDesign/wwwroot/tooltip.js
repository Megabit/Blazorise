import { initialize as baseInitialize, destroy, updateContent } from "/_content/Blazorise/tooltip.js";

export function initialize(element, elementId, options) {
    baseInitialize(element, elementId, options);

    if (element && element.querySelector(".ant-input,.ant-btn")) {
        element.classList.add("b-tooltip-inline");
    }
}

export { destroy, updateContent };