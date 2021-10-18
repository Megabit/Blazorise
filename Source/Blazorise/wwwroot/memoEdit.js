import { Behave } from "./vendors/Behave.js";

const _instances = [];

export function initialize(element, elementId, options) {
    const replaceTab = options.replaceTab || false;
    const tabSize = options.tabSize || 4;
    const softTabs = options.tabSize || true;

    let behave = replaceTab ? new Behave({
        textarea: element,
        replaceTab: replaceTab,
        softTabs: softTabs,
        tabSize: tabSize,
        autoOpen: true,
        overwrite: true,
        autoStrip: true,
        autoIndent: true,
        fence: false
    }) : null;

    _instances[elementId] = {
        element: element,
        elementId: elementId,
        replaceTab: replaceTab,
        tabSize: tabSize,
        softTabs: softTabs,
        behave: behave
    };
}

export function destroy(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.behave) {
        instance.behave.destroy();
        instance.behave = null;
    }

    delete _instances[elementId];
}

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (instance) {
        if (options.replaceTab.changed || options.tabSize.changed || options.softTabs.changed) {
            instance.replaceTab = options.replaceTab.value;
            instance.tabSize = options.tabSize.value;
            instance.softTabs = options.softTabs.value;

            if (instance.behave) {
                instance.behave.destroy();
                instance.behave = null;
            }

            if (instance.replaceTab) {
                instance.behave = new Behave({
                    textarea: element,
                    replaceTab: instance.replaceTab,
                    softTabs: instance.softTabs,
                    tabSize: instance.tabSize,
                    autoOpen: true,
                    overwrite: true,
                    autoStrip: true,
                    autoIndent: true,
                    fence: false
                });
            }
        }
    };
}