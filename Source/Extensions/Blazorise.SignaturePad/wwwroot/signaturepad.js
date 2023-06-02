import { getRequiredElement } from "../Blazorise/utilities.js?v=1.3.0.0";
import "./vendors/sigpad.js?v=1.3.0.0";

const _instances = [];

export function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const sigpad = new SignaturePad(element, {
        minWidth: options.minLineWidth || 0.5,
        maxWidth: options.maxLineWidth || 2.5,
        backgroundColor: options.backgroundColor || "rgba(0,0,0,0)",
        penColor: options.penColor || "black",
        velocityFilterWeight: options.velocityFilterWeight || 0.7,
        dotSize: options.dotSize || 0,
        throttle: options.throttle || 16,
        minPointDistance: options.minPointDistance || 0.5,
    });

    if (options.dataUrl) {
        sigpad.fromDataURL(options.dataUrl);
    }

    if (options.readOnly === true) {
        sigpad.off();
    }

    const instance = {
        options: options,
        sigpad: sigpad,
        dotNetAdapter: dotNetAdapter
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
        if (options.dataUrl.changed) {
            if (options.dataUrl.value) {
                instance.sigpad.fromDataURL(options.dataUrl.value);
            } else {
                instance.sigpad.clear();
            }
        }

        if (options.penColor.changed) {
            instance.sigpad.penColor = options.penColor.value || "black";
        }

        if (options.minLineWidth.changed) {
            instance.sigpad.minWidth = options.minLineWidth.value;
        }

        if (options.maxLineWidth.changed) {
            instance.sigpad.maxWidth = options.maxLineWidth.value;
        }

        if (options.backgroundColor.changed) {
            instance.sigpad.backgroundColor = options.backgroundColor.value || "rgba(0,0,0,0)";
            const data = instance.sigpad.toData();
            instance.sigpad.fromData(data);
        }

        if (options.velocityFilterWeight.changed) {
            instance.sigpad.velocityFilterWeight = options.velocityFilterWeight.value;
        }

        if (options.dotSize.changed) {
            instance.sigpad.dotSize = options.dotSize.value;
        }

        if (options.readOnly.changed) {
            if (options.readOnly.value === true) {
                instance.sigpad.off();
            }
            else {
                instance.sigpad.on();
            }
        }

        if (options.imageType.changed) {
            instance.options.imageType = options.imageType.value;
        }

        if (options.imageQuality.changed) {
            instance.options.imageQuality = options.imageQuality.value;
        }

        if (options.includeImageBackgroundColor.changed) {
            instance.options.includeImageBackgroundColor = options.includeImageBackgroundColor.value;
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

export function undo(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.sigpad) {
        const data = instance.sigpad.toData();

        if (data && data.length > 0) {
            data.pop();
            instance.sigpad.fromData(data);

            return getImageDataURL(instance.sigpad, instance.options);
        }
    }

    return null;
}

function registerToEvents(dotNetAdapter, instance) {
    if (instance && instance.sigpad) {
        instance.sigpad.addEventListener("beginStroke", (e) => {
            if (e && e.detail) {
                dotNetAdapter.invokeMethodAsync("NotifyBeginStroke", e.detail.offsetX, e.detail.offsetY)
            }
        });

        instance.sigpad.addEventListener("endStroke", (e) => {
            if (e && e.detail) {
                const dataURL = getImageDataURL(instance.sigpad, instance.options);

                dotNetAdapter.invokeMethodAsync("NotifyEndStroke", dataURL, e.detail.offsetX, e.detail.offsetY);
            }
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
        return sigpad.toDataURL("image/svg+xml", { includeBackgroundColor: options.includeImageBackgroundColor || false });
    }

    return sigpad.toDataURL("image/png", options.imageQuality || 1);
}