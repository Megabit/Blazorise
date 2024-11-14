import { initialize as baseInitialize, destroy, updateContent } from "../Blazorise/tooltip.js?v=1.7.0.0";

export function initialize(element, elementId, options) {
    baseInitialize(element, elementId, options);

    if (options.autodetectInline && element && element.querySelector(".ant-input,.ant-btn")) {
        element.classList.add("b-tooltip-inline");
    }
}

export { destroy, updateContent };