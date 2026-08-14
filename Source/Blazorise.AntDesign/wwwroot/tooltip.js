import { initialize as baseInitialize, destroy, updateContent } from "../Blazorise/tooltip.js?v=2.3.0.0";

export function initialize(element, elementId, options) {
    baseInitialize(element, elementId, options);

    if (options.autodetectInline && element && element.querySelector(".ant-input,.ant-btn")) {
        element.classList.add("ant-tooltip-host-inline");
    }
}

export { destroy, updateContent };