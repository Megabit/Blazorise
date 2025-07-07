import "./vendors/tippy.js?v=1.8.0.0";
import "./vendors/popper.js?v=1.8.0.0";
import { getRequiredElement } from "./utilities.js?v=1.8.0.0";

const _instances = [];

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const triggerTarget = options.triggerTargetId ? document.getElementById(options.triggerTargetId) : null;

    const appendTo = determineAppendTo(options.appendTo);

    const defaultOptions = {
        theme: options.theme || 'blazorise',
        content: options.text,
        placement: options.placement,
        maxWidth: options.maxWidth ? options.maxWidth : options.multiline ? "15rem" : null,
        duration: options.fade ? [options.fadeDuration, options.fadeDuration] : [0, 0],
        arrow: options.showArrow,
        allowHTML: true,
        trigger: options.trigger,
        triggerTarget: triggerTarget,
        zIndex: options.zIndex,
        interactive: options.interactive,
        zIndex: options.zIndex || 9999,
        appendTo: appendTo,
        delay: [options.delay.show, options.delay.hide],
    };

    const alwaysActiveOptions = options.alwaysActive
        ? {
            showOnCreate: true,
            hideOnClick: false,
            trigger: "manual"
        } : {};

    const instance = tippy(element, Object.assign({}, defaultOptions, alwaysActiveOptions));

    if (options.text) {
        instance.enable();
    }
    else {
        instance.disable();
    }

    _instances[elementId] = instance;

    return instance;
}

export function destroy(element, elementId) {
    var instances = _instances || {};

    const instance = instances[elementId];

    if (instance) {
        instance.hide();

        delete instances[elementId];
    }
}

export function updateContent(element, elementId, content) {
    const instance = _instances[elementId];

    if (instance) {
        instance.setContent(content);

        if (content) {
            instance.enable();
        }
        else {
            instance.disable();
        }
    }
}

function determineAppendTo(value) {
    if (!value || value === "body") {
        return () => document.body;
    }

    if (value === "parent")
        return "parent";
    else if (value.length > 0 && value[0] === "#")
        return document.getElementById(value);

    return () => document.body;
}