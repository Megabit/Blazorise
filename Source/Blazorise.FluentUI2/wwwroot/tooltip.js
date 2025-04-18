import { initialize as baseInitialize, destroy, updateContent } from "../Blazorise/tooltip.js?v=1.7.6.0";

export function initialize(element, elementId, options) {
    options = Object.assign({}, options, { theme: "fluentui", zIndex: "var(--zIndexTooltip)" });

    const tippyInstance = baseInitialize(element, elementId, options);

    if (options.autodetectInline && element && element.querySelector(".fui-Input,.fui-Input__input,button")) {
        element.classList.add("fui-Display-inline-block");
    }

    if (tippyInstance && tippyInstance.popper) {
        tippyInstance.popper.className = "fui-Tooltip";

        const tippyBox = tippyInstance.popper.querySelector(".tippy-box");

        if (tippyBox) {
            tippyBox.className = "fui-Tooltip__box";

            const tippyContent = tippyBox.querySelector(".tippy-content");

            if (tippyContent) {
                tippyContent.className = "fui-Tooltip__content";
            }
            const tippyArrow = tippyBox.querySelector(".tippy-arrow");

            if (tippyArrow) {
                tippyArrow.classList.add("fui-Tooltip__arrow");
            }
        }
    }
}

export { destroy, updateContent };