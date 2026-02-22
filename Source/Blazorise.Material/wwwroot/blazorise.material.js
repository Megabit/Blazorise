(() => {
    const initializedKey = "__blazoriseMaterialRippleInitialized";

    if (window[initializedKey]) {
        return;
    }

    window[initializedKey] = true;

    const rippleTargetSelector = [
        ".mui-button",
        ".mui-dropdown-item",
        ".mui-tab-link",
        ".mui-pagination-link",
        ".mui-breadcrumb-link",
        ".mui-list-item.mui-list-item-action",
        ".mui-list-item > a:only-child",
        ".mui-list-item > details > summary",
        ".mui-button-close",
        ".mui-chip.mui-chip-link",
        ".mui-chip > .mui-chip-close"
    ].join(", ");

    const disabledSelector = [
        ".mui-button-disabled",
        ".mui-dropdown-disabled",
        ".mui-tab-item-disabled",
        ".mui-pagination-item-disabled",
        ".disabled",
        "[disabled]",
        "[aria-disabled=\"true\"]"
    ].join(", ");

    const isDisabled = target => {
        if (!target) {
            return true;
        }

        if (target.matches(disabledSelector)) {
            return true;
        }

        return target.closest(disabledSelector) !== null;
    };

    const createRipple = (target, clientX, clientY) => {
        const rect = target.getBoundingClientRect();

        if (!rect.width || !rect.height) {
            return;
        }

        const size = Math.max(rect.width, rect.height) * 2;
        const ripple = document.createElement("span");
        const left = clientX - rect.left - size / 2;
        const top = clientY - rect.top - size / 2;

        ripple.className = "mui-ripple";
        ripple.style.width = `${size}px`;
        ripple.style.height = `${size}px`;
        ripple.style.left = `${left}px`;
        ripple.style.top = `${top}px`;

        target.appendChild(ripple);
        ripple.addEventListener("animationend", () => ripple.remove(), { once: true });
    };

    document.addEventListener("pointerdown", event => {
        if (event.button !== 0 || !(event.target instanceof Element)) {
            return;
        }

        const target = event.target.closest(rippleTargetSelector);

        if (!target || isDisabled(target)) {
            return;
        }

        createRipple(target, event.clientX, event.clientY);
    }, { passive: true });

    document.addEventListener("keydown", event => {
        if ((event.key !== "Enter" && event.key !== " ") || !(event.target instanceof Element)) {
            return;
        }

        const target = event.target.closest(rippleTargetSelector);

        if (!target || isDisabled(target)) {
            return;
        }

        const rect = target.getBoundingClientRect();
        createRipple(target, rect.left + rect.width / 2, rect.top + rect.height / 2);
    });
})();