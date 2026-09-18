import { getRequiredElement } from "./utilities.js?v=2.3.1.0";

const instances = new Map();

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    destroy(element, elementId);

    const instance = {
        element: element,
        options: options,
        owner: options.containerSelector ? element.closest(options.containerSelector) : element.parentElement,
        visible: isVisible(element, options),
        originalInert: element.inert,
        hasOriginalAnimatingClass: options.animatingClassName && element.classList.contains(options.animatingClassName),
        animation: null,
        observer: null,
        reducedMotion: window.matchMedia("(prefers-reduced-motion: reduce)"),
        onReducedMotion: null
    };

    instance.observer = new MutationObserver(records => {
        const panelChanged = records.some(record => record.target === element);
        const optionsChanged = records.some(record => record.target !== element);
        const visible = panelChanged ? isVisible(element, options) : instance.visible;

        if (visible !== instance.visible || (instance.animation && optionsChanged))
            transition(instance, visible);
        else if (instance.animation)
            setPanelClasses(instance, true);
    });

    instance.onReducedMotion = () => {
        if (instance.reducedMotion.matches)
            finish(instance);
    };

    observe(instance);

    instance.reducedMotion.addEventListener("change", instance.onReducedMotion);
    element.inert = instance.originalInert || !instance.visible;
    instances.set(elementId, instance);
}

function isVisible(element, options) {
    return options.visibleClassName
        ? element.classList.contains(options.visibleClassName)
        : !element.classList.contains(options.hiddenClassName);
}

function observe(instance) {
    instance.observer.observe(instance.element, { attributes: true, attributeFilter: ["class"] });

    if (instance.owner)
        instance.observer.observe(instance.owner, { attributes: true, attributeFilter: ["data-animation-duration", "style"] });
}

function transition(instance, visible) {
    const element = instance.element;
    const wasVisible = instance.visible;
    // A running Web Animation retains its current height even when Blazor changes the classes.
    const currentHeight = instance.animation ? parseFloat(getComputedStyle(element).height) : null;

    cancelAnimation(instance);
    instance.visible = visible;
    element.inert = instance.originalInert || !visible;

    // Measure natural content height without adding a wrapper or retaining an inline height.
    setPanelClasses(instance, false, true);

    const expandedHeight = parseFloat(getComputedStyle(element).height) || 0;
    const startHeight = currentHeight ?? (wasVisible ? expandedHeight : 0);
    const endHeight = visible ? expandedHeight : 0;

    setPanelClasses(instance, true);

    const style = getComputedStyle(element);
    const properties = style.transitionProperty.split(",").map(property => property.trim());
    const durations = style.transitionDuration.split(",");
    const easings = style.transitionTimingFunction.match(/cubic-bezier\([^)]+\)|steps\([^)]+\)|[^,]+/g);
    const index = Math.max(0, properties.findIndex(property => property === "height" || property === "all"));
    const durationString = durations[index % durations.length].trim();
    const duration = instance.reducedMotion.matches || !properties.some(property => property === "height" || property === "all")
        ? 0
        : parseFloat(durationString) * (durationString.endsWith("ms") ? 1 : 1000);

    if (!duration || startHeight === endHeight || !element.animate) {
        finish(instance);
        return;
    }

    const animation = element.animate(
        [{ height: startHeight + "px" }, { height: endHeight + "px" }],
        { duration: duration, easing: easings[index % easings.length].trim(), fill: "both" });

    instance.animation = animation;
    animation.onfinish = () => {
        if (instance.animation === animation)
            finish(instance);
    };
}

function setPanelClasses(instance, transitioning, visible = instance.visible) {
    // Ignore temporary animation classes when observing Blazor visibility changes.
    instance.observer.disconnect();

    const options = instance.options;
    toggleClass(instance.element, options.baseClassName, !transitioning);
    toggleClass(instance.element, options.animatingClassName, transitioning || instance.hasOriginalAnimatingClass);
    toggleClass(instance.element, options.visibleClassName, !transitioning && visible);
    toggleClass(instance.element, options.hiddenClassName, !transitioning && !visible);
    observe(instance);
}

function toggleClass(element, className, enabled) {
    if (className)
        element.classList.toggle(className, enabled);
}

function cancelAnimation(instance) {
    if (instance.animation) {
        instance.animation.onfinish = null;
        instance.animation.cancel();
        instance.animation = null;
    }
}

function finish(instance) {
    cancelAnimation(instance);
    setPanelClasses(instance, false);
}

export function destroy(element, elementId) {
    const instance = instances.get(elementId);

    if (!instance)
        return;

    finish(instance);
    instance.observer.disconnect();
    instance.reducedMotion.removeEventListener("change", instance.onReducedMotion);
    instance.element.inert = instance.originalInert;
    instances.delete(elementId);
}