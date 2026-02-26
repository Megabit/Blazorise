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
        ".mui-bar-link",
        ".mui-bar-brand",
        ".mui-bar-toggler",
        ".mui-list-item.mui-list-item-action",
        ".mui-list-item > a:only-child",
        ".mui-list-item > details > summary",
        ".mui-button-close",
        ".mui-chip.mui-chip-link",
        ".mui-chip > .mui-chip-close"
    ].join(", ");

    const noRippleSelector = ".mui-dropdown-menu";

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
        if (target.matches(".mui-dropdown-menu")) {
            return;
        }

        if (target.closest(".mui-dropdown-menu") && !target.matches(".mui-dropdown-item")) {
            return;
        }

        const surface = ensureRippleSurface(target);
        const rect = surface.getBoundingClientRect();

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

        surface.appendChild(ripple);
        ripple.addEventListener("animationend", () => ripple.remove(), { once: true });
    };

    const ensureRippleSurface = target => {
        const existingSurface = Array.from(target.children).find(child => child.classList && child.classList.contains("mui-ripple-surface"));
        if (existingSurface) {
            return existingSurface;
        }

        const computedStyle = window.getComputedStyle(target);
        if (computedStyle.position === "static") {
            target.style.position = "relative";
        }

        const surface = document.createElement("span");
        surface.className = "mui-ripple-surface";
        surface.style.position = "absolute";
        surface.style.inset = "0";
        surface.style.overflow = "hidden";
        surface.style.borderRadius = "inherit";
        surface.style.pointerEvents = "none";
        surface.style.zIndex = "0";

        target.prepend(surface);

        return surface;
    };

    const resolveRippleTarget = event => {
        if (!(event.target instanceof Element)) {
            return null;
        }

        const dropdownMenu = event.target.closest(".mui-dropdown-menu");
        if (dropdownMenu) {
            const dropdownItem = event.target.closest(".mui-dropdown-item");
            return dropdownItem ?? null;
        }

        const path = typeof event.composedPath === "function"
            ? event.composedPath()
            : [event.target];

        for (const node of path) {
            if (!(node instanceof Element)) {
                continue;
            }

            if (node.matches(noRippleSelector)) {
                break;
            }

            if (node.matches(rippleTargetSelector)) {
                return node;
            }
        }

        return event.target.closest(rippleTargetSelector);
    };

    document.addEventListener("pointerdown", event => {
        if (!(event.target instanceof Element)) {
            return;
        }

        const isMousePointer = event.pointerType === "mouse" || event.pointerType === "";
        if (isMousePointer && event.button !== 0) {
            return;
        }

        if (event.isPrimary === false) {
            return;
        }

        const target = resolveRippleTarget(event);

        if (!target || isDisabled(target)) {
            return;
        }

        createRipple(target, event.clientX, event.clientY);
    }, { passive: true });

    document.addEventListener("touchstart", event => {
        if (window.PointerEvent || !(event.target instanceof Element)) {
            return;
        }

        const touch = event.changedTouches && event.changedTouches[0];
        if (!touch) {
            return;
        }

        const target = resolveRippleTarget(event);

        if (!target || isDisabled(target)) {
            return;
        }

        createRipple(target, touch.clientX, touch.clientY);
    }, { passive: true });

    document.addEventListener("keydown", event => {
        if ((event.key !== "Enter" && event.key !== " ") || !(event.target instanceof Element)) {
            return;
        }

        const target = resolveRippleTarget(event);

        if (!target || isDisabled(target)) {
            return;
        }

        const rect = target.getBoundingClientRect();
        createRipple(target, rect.left + rect.width / 2, rect.top + rect.height / 2);
    });
})();