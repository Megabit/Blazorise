import Cropper, { CropperViewer } from "./vendors/cropper.js?v=1.3.1.0";

import { getRequiredElement } from "../Blazorise/utilities.js?v=1.3.1.0";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"_content/Blazorise.Cropper/blazorise.cropper.css?v=1.3.1.0\" />");

const _instances = [];

export function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = {
        options: options,
        adapter: dotNetAdapter,
        cropper: null,
    };

    const image = new Image();

    image.src = options.source;
    image.alt = options.alt;
    image.crossOrigin = options.crossOrigin;

    const template = (
        `<cropper-canvas background="${options.showBackground}" disabled="${!options.enabled}">
          <cropper-image rotatable="${options.image.rotatable}" scalable="${options.image.scalable}" skewable="${options.image.skewable}" translatable="${options.image.translatable}"></cropper-image>
          <cropper-shade hidden></cropper-shade>
          <cropper-handle action="select" plain></cropper-handle>
          <cropper-selection id="cropper-selection-${elementId}" initial-coverage="${options.selection.initialCoverage ?? NaN}" aspect-ratio="${options.selection.aspectRatio ?? NaN}" initial-aspect-ratio="${options.selection.initialAspectRatio ?? NaN}" movable="${options.selection.movable}" resizable="${options.selection.resizable}" zoomable="${options.selection.zoomable}" keyboard="${options.selection.keyboard}" outlined="${options.selection.outlined}">
          <cropper-grid role="grid" rows="${options.grid.rows}" columns="${options.grid.columns}" bordered="${options.grid.bordered}" covered="${options.grid.covered}"></cropper-grid>
          <cropper-crosshair centered></cropper-crosshair>
          <cropper-handle action="move" theme-color="rgba(255, 255, 255, 0.35)"></cropper-handle>
          <cropper-handle action="n-resize"></cropper-handle>
          <cropper-handle action="e-resize"></cropper-handle>
          <cropper-handle action="s-resize"></cropper-handle>
          <cropper-handle action="w-resize"></cropper-handle>
          <cropper-handle action="ne-resize"></cropper-handle>
          <cropper-handle action="nw-resize"></cropper-handle>
          <cropper-handle action="se-resize"></cropper-handle>
          <cropper-handle action="sw-resize"></cropper-handle>
          </cropper-selection>
        </cropper-canvas>`
    );

    const cropper = new Cropper(image, {
        container: element,
        template: template
    });

    instance.cropper = cropper;

    const cropperCanvas = cropper.getCropperCanvas();
    const cropperSelection = cropper.getCropperSelection();
    const cropperImage = cropper.getCropperImage();

    cropperImage.$ready((image) => {
        invokeDotNetMethodAsync(dotNetAdapter, "ImageReady");
    });

    registerEvents(cropperCanvas, cropperSelection, dotNetAdapter);

    _instances[elementId] = instance;
}

export function initializeViewer(cropperElementRef, cropperElementId, element, elementId, options) {
    const instance = _instances[cropperElementId];

    if (!instance)
        return;

    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const cropperViewer = new CropperViewer();

    cropperViewer.selection = `#cropper-selection-${cropperElementId}`;

    element.appendChild(cropperViewer);
}

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    if (instance.cropper) {
        const cropper = instance.cropper;
        const cropperCanvas = cropper.getCropperCanvas();
        const cropperImage = cropper.getCropperImage();
        const cropperSelection = cropper.getCropperSelection();

        if (cropperCanvas) {
            if (options.enabled.changed) {
                cropperCanvas.disabled = !options.enabled.value;
            }
        }

        if (cropperImage) {
            if (options.source.changed) {
                cropperImage.src = options.source.value;

                // Callback needs to be setup again after each source changed.
                cropperImage.$ready((image) => {
                    invokeDotNetMethodAsync(instance.adapter, "ImageReady");
                });
            }

            if (options.alt.changed) {
                cropperImage.alt = options.alt.value;
            }

            if (options.crossOrigin.changed) {
                cropperImage.crossOrigin = options.crossOrigin.value;
            }

            if (options.image.changed) {
                const image = options.image.value;

                cropperImage.rotatable = image.rotatable || true;
                cropperImage.scalable = image.scalable || true;
                cropperImage.skewable = image.skewable || true;
                cropperImage.translatable = image.translatable || true;
            }
        }

        if (cropperSelection && options.selection.changed) {
            const selection = options.selection.value;

            cropperSelection.aspectRatio = selection.aspectRatio || NaN;
            cropperSelection.initialAspectRatio = selection.initialAspectRatio || NaN;
            cropperSelection.initialCoverage = selection.initialCoverage || NaN;
            cropperSelection.movable = selection.movable || false;
            cropperSelection.resizable = selection.resizable || false;
            cropperSelection.zoomable = selection.zoomable || false;
            cropperSelection.keyboard = selection.keyboard || false;
            cropperSelection.outlined = selection.outlined || false;

            cropperSelection.$move(1);
            cropperSelection.$move(-1);
        }
    }
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    delete instances[elementId];
}

export async function cropBase64(element, elementId, options) {
    const instance = _instances[elementId];

    if (instance && instance.cropper) {
        const cropper = instance.cropper;
        const cropperSelection = cropper.getCropperSelection();

        if (cropperSelection) {
            const croppedCanvas = cropperSelection.$toCanvas(options);

            return await croppedCanvas.then((canvas) => {
                return canvas.toDataURL();
            });
        }
    }

    return "";
}

export function move(element, elementId, x, y) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    if (instance.cropper) {
        const cropper = instance.cropper;
        const cropperImage = cropper.getCropperImage();

        if (cropperImage) {
            cropperImage.$move(x, y);
        }
    }
}

export function moveTo(element, elementId, x, y) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    if (instance.cropper) {
        const cropper = instance.cropper;
        const cropperImage = cropper.getCropperImage();

        if (cropperImage) {
            cropperImage.$moveTo(x, y);
        }
    }
}

export function zoom(element, elementId, scale) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    if (instance.cropper) {
        const cropper = instance.cropper;
        const cropperImage = cropper.getCropperImage();

        if (cropperImage) {
            cropperImage.$zoom(scale);
        }
    }
}

export function rotate(element, elementId, angle) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    if (instance.cropper) {
        const cropper = instance.cropper;
        const cropperImage = cropper.getCropperImage();

        if (cropperImage) {
            cropperImage.$rotate(`${angle}deg`);
        }
    }
}

export function scale(element, elementId, x, y) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    if (instance.cropper) {
        const cropper = instance.cropper;
        const cropperImage = cropper.getCropperImage();

        if (cropperImage) {
            cropperImage.$scale(x, y);
        }
    }
}

export function center(element, elementId, size) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    if (instance.cropper) {
        const cropper = instance.cropper;
        const cropperImage = cropper.getCropperImage();

        if (cropperImage) {
            if (size) {
                cropperImage.$center(size);
            }
            else {
                cropperImage.$center();
            }
        }
    }
}

export function resetSelection(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.cropper) {
        const cropper = instance.cropper;
        const cropperSelection = cropper.getCropperSelection();

        if (cropperSelection) {
            cropperSelection.$reset();
        }
    }
}

function registerEvents(cropperCanvas, cropperSelection, dotNetAdapter) {
    if (cropperCanvas) {
        cropperCanvas.addEventListener('actionstart', (event) => {
            invokeDotNetMethodAsync(dotNetAdapter, "CropStart");
        });

        cropperCanvas.addEventListener('actionmove', (event) => {
            invokeDotNetMethodAsync(dotNetAdapter, "CropMove");
        });

        cropperCanvas.addEventListener('actionend', (event) => {
            invokeDotNetMethodAsync(dotNetAdapter, "CropEnd");
        });

        cropperCanvas.addEventListener('action', (event) => {
            if (event.detail.action !== "scale") {
                invokeDotNetMethodAsync(dotNetAdapter, "Crop", event.detail.startX, event.detail.startY, event.detail.endX, event.detail.endY);
            }
        });

        cropperCanvas.addEventListener('action', (event) => {
            if (event.detail.action === "scale") {
                invokeDotNetMethodAsync(dotNetAdapter, "Zoom", event.detail.scale);
            }
        });
    }

    if (cropperSelection) {
        cropperSelection.addEventListener('change', (event) => {
            invokeDotNetMethodAsync(dotNetAdapter, "SelectionChanged", event.detail.x, event.detail.y, event.detail.width, event.detail.height);
        });
    }
}

function invokeDotNetMethodAsync(dotNetAdapter, methodName, ...args) {
    dotNetAdapter.invokeMethodAsync(methodName, ...args)
        .catch((reason) => {
            console.error(reason);
        });
}
