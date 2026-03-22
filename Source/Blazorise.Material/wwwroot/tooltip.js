import { initialize as baseInitialize, destroy, updateContent } from "../Blazorise/tooltip.js?v=2.0.3.0";

export function initialize(element, elementId, options) {
    options = Object.assign({}, options, { theme: "material", zIndex: options.zIndex ?? "var(--mui-zindex-tooltip)" });

    const tippyInstance = baseInitialize(element, elementId, options);

    if (options.autodetectInline && element && element.querySelector(".mui-check > input,.mui-radio > input,.mui-switch > input,.mui-button,button")) {
        element.classList.add("mui-tooltip-inline");
    }

    return tippyInstance;
}

export { destroy, updateContent };