import { getRequiredElement } from "../Blazorise/utilities.js?v=2.0.0.0";
import "./vendors/sigpad.js?v=2.0.0.0";

const _instances = [];

export function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    // we need to match the canvas size ans size in styles so that we can properly calculate scaling based on screen size
    if (element.width && element.height) {
        element.style.width = `${element.width}px`;
        element.style.height = `${element.height}px`;
    }

    const sigpad = new SignaturePad(element, {
        minWidth: options.minLineWidth || 0.5,
        maxWidth: options.maxLineWidth || 2.5,
        backgroundColor: options.backgroundColor || "rgba(0,0,0,0)",
        penColor: options.penColor || "black",
        velocityFilterWeight: options.velocityFilterWeight || 0.7,
        dotSize: options.dotSize || 0,
        throttle: options.throttle || 16,
        minPointDistance: options.minPointDistance || 0.5,
        canvasContextOptions: {
            willReadFrequently: true
        }
    });

    if (options.dataUrl) {
        sigpad.fromDataURL(options.dataUrl, { ratio: getRatio() });
    }

    if (options.readOnly === true) {
        sigpad.off();
    }

    const instance = {
        options: options,
        sigpad: sigpad,
        dotNetAdapter: dotNetAdapter,
        element: element
    };

    registerToEvents(dotNetAdapter, instance);

    resizeCanvas(sigpad, element);

    // Observe the element visibily. This is needed in cases when signaturepad is placed inside of tabs.
    const observer = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            resizeCanvas(sigpad, entry.target);
        });
    }, options);
    observer.observe(element);

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
                instance.sigpad.fromDataURL(options.dataUrl.value, { ratio: getRatio() });
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
                const offsets = getStrokeOffsets(e.detail, instance.element);
                dotNetAdapter.invokeMethodAsync("NotifyBeginStroke", offsets.offsetX, offsets.offsetY)
            }
        });

        instance.sigpad.addEventListener("endStroke", (e) => {
            if (e && e.detail) {
                const dataURL = getImageDataURL(instance.sigpad, instance.options);

                const offsets = getStrokeOffsets(e.detail, instance.element);
                dotNetAdapter.invokeMethodAsync("NotifyEndStroke", dataURL, offsets.offsetX, offsets.offsetY);
            }
        });
    }
}

function getStrokeOffsets(detail, element) {
    if (!detail || !element) {
        return { offsetX: 0, offsetY: 0 };
    }

    const rect = element.getBoundingClientRect();

    if (typeof detail.x === "number" && typeof detail.y === "number") {
        return {
            offsetX: detail.x - rect.left,
            offsetY: detail.y - rect.top
        };
    }

    const event = detail.event;
    if (event && typeof event.offsetX === "number" && typeof event.offsetY === "number") {
        return { offsetX: event.offsetX, offsetY: event.offsetY };
    }

    if (event && typeof event.clientX === "number" && typeof event.clientY === "number") {
        return {
            offsetX: event.clientX - rect.left,
            offsetY: event.clientY - rect.top
        };
    }

    return { offsetX: 0, offsetY: 0 };
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

window.onresize = resizeAllCanvas;

function resizeAllCanvas() {
    if (_instances) {
        for (let i = 0; i < _instances.length; i++) {
            const instance = _instances[i];

            if (instance) {
                resizeCanvas(instance.sigpad, instance.element);
            }
        }
    }
}

function resizeCanvas(sigpad, canvas) {
    if (!sigpad || !canvas) {
        return null;
    }

    const offsetWidth = canvas.offsetWidth;
    const offsetHeight = canvas.offsetHeight;

    if (!offsetWidth || !offsetHeight) {
        return null;
    }

    // When zoomed out to less than 100%, for some very strange reason,
    // some browsers report devicePixelRatio as less than 1
    // and only part of the canvas is cleared then.
    const ratio = getRatio();

    const context = canvas.getContext("2d", { willReadFrequently: true });

    const canvasWidth = canvas.width;
    const canvasHeight = canvas.height;
    const canReadPixels = canvasWidth > 0 && canvasHeight > 0;
    const imageData = canReadPixels
        ? context.getImageData(0, 0, canvasWidth, canvasHeight, { colorSpace: 'srgb' })
        : null;

    // This part causes the canvas to be cleared
    canvas.width = offsetWidth * ratio;
    canvas.height = offsetHeight * ratio;
    context.scale(ratio, ratio);

    sigpad.clear();

    if (imageData) {
        context.putImageData(imageData, 0, 0);
    }
}

function getRatio() {
    return Math.max(window.devicePixelRatio || 1, 1);
}