import { getRequiredElement } from "../Blazorise/utilities.js?v=1.2.3.0";
import "./vendors/sigpad.js?v=1.2.3.0";

const _instances = [];

export function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const sigpad = new SignaturePad(element, {
        minWidth: options.minWidth || 0.5,
        maxWidth: options.maxWidth || 2.5,
        backgroundColor: options.backgroundColor || "rgba(0,0,0,0)",
        penColor: options.penColor || "black",
        velocityFilterWeight: options.velocityFilterWeight || 0.7,
        dotSize: options.dotSize,
        throttle: options.throttle || 16,
        minPointDistance: options.minPointDistance || 0.5,
    });

    if (options.value) {
        sigpad.fromData(options.value, { clear: false });
    }

    const instance = {
        options: options,
        sigpad: sigpad,
    };

    registerToEvents(dotNetAdapter, instance);

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        if (instance.sigpad) {
            instance.sigpad.off();
        }

        delete instances[elementId];
    }
}

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];
    if (instance && instance.sigpad && options) {
        if (options.penColor.changed) {
            instance.sigpad.penColor = options.penColor.value || "black";
        }

        if (options.minWidth.changed) {
            instance.sigpad.minWidth = options.minWidth.value;
        }

        if (options.maxWidth.changed) {
            instance.sigpad.maxWidth = options.maxWidth.value;
        }

        if (options.backgroundColor.changed) {
            instance.sigpad.backgroundColor = options.backgroundColor.value || "rgba(0,0,0,0)";
            const data = instance.sigpad.toData();
            instance.sigpad.clear();
            instance.sigpad.fromData(data);
        }

        if (options.velocityFilterWeight.changed) {
            instance.sigpad.velocityFilterWeight = options.velocityFilterWeight.value;
        }

        if (options.dotSize.changed) {
            instance.sigpad.dotSize = options.dotSize.value;
        }

        if (options.imageType.changed) {
            instance.options.imageType = options.imageType.value;
        }

        if (options.imageQuality.changed) {
            instance.options.imageQuality = options.imageQuality.value;
        }
    }
}

export function setData(element, elementId, data) {
    const instance = _instances[elementId];

    if (instance && instance.sigpad) {
        instance.sigpad.fromData(data);
    }
}

export function clear(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.sigpad) {
        instance.sigpad.clear();
    }
}

function registerToEvents(dotNetAdapter, instance) {
    if (instance && instance.sigpad) {
        instance.sigpad.addEventListener("beginStroke", (e) => {
            dotNetAdapter.invokeMethodAsync("NotifyBeginStroke")
        });

        instance.sigpad.addEventListener("endStroke", (e) => {
            const dataURL = getImageDataURL(instance.sigpad, instance.options);

            dotNetAdapter.invokeMethodAsync("NotifyEndStroke", dataURL)
                .catch((reason) => {
                    console.error(reason);
                });
        });
    }
}

function getImageDataURL(sigpad, options) {
    if (!sigpad || !options) {
        return null;
    }

    if (options.imageType === "jpeg") {
        return sigpad.toDataURL("image/jpeg", options.imageQuality || 1);
    }
    else if (options.imageType === "svg") {
        return sigpad.toDataURL("image/svg+xml");
    }

    return sigpad.toDataURL("image/png", options.imageQuality || 1);
}