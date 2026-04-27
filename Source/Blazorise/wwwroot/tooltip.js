import "./vendors/tippy.js?v=2.1.1.0";
import "./vendors/popper.js?v=2.1.1.0";
import { getRequiredElement, registerDisconnectCleanup, unregisterDisconnectCleanup } from "./utilities.js?v=2.1.1.0";

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
        aria: { content: "describedby" },
    };

    const alwaysActiveOptions = options.alwaysActive
        ? {
            showOnCreate: true,
            hideOnClick: false,
            trigger: "manual"
        } : {};

    const tippyInstance = tippy(element, Object.assign({}, defaultOptions, alwaysActiveOptions));

    if (options.text) {
        tippyInstance.enable();
    }
    else {
        tippyInstance.disable();
    }

    _instances[elementId] = {
        tippy: tippyInstance,
        disconnectCleanupId: registerDisconnectCleanup(element, () => destroy(null, elementId, false))
    };

    return tippyInstance;
}

export function destroy(element, elementId, unregisterCleanup = true) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        if (unregisterCleanup) {
            unregisterDisconnectCleanup(instance.disconnectCleanupId);
        }

        if (instance.tippy) {
            instance.tippy.destroy();
        }

        delete instances[elementId];
    }
}

export function updateContent(element, elementId, content) {
    const instance = _instances[elementId]?.tippy;

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