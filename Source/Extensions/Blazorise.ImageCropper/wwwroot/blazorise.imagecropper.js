import "./vendors/cropper.js?v=1.2.0.0";

import { getRequiredElement } from "../Blazorise/utilities.js?v=1.2.0.0";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"_content/Blazorise.ImageCropper/blazorise.imagecropper.css?v=1.2.0.0\" />");

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

    const cropper = new Cropper(element, options);

    if (options.enabled) {
        cropper.enable();
    } else {
        cropper.disable();
    }

    instance.cropper = cropper;

    registerEvents(element, dotNetAdapter);

    _instances[elementId] = instance;
}

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    if (instance.cropper) {
        const cropper = instance.cropper;

        // changing the viewMode is a destructive operation so we need to create new cropper instance
        if (options.viewMode.changed) {
            const initOptions = {
                source: options.source.value,
                aspectRatio: options.aspectRatio.value || NaN,
                viewMode: options.viewMode.value || 0,
                preview: options.preview.value || '',
                enabled: options.enabled.value
            };

            cropper.destroy();

            initialize(instance.adapter, element, elementId, initOptions);
        }
        else {
            if (options.source.changed) {
                cropper.replace(options.source.value);
            }

            if (options.aspectRatio.changed) {
                cropper.setAspectRatio(options.aspectRatio.value || NaN);
            }

            if (options.preview.changed) {
                cropper.options.preview = options.preview.value || '';
            }

            if (options.enabled.changed) {
                if (options.enabled.value) {
                    cropper.enable();
                } else {
                    cropper.disable();
                }
            }
        }
    }
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        if (instance.cropper) {
            instance.cropper.destroy();
        }

        delete instances[elementId];
    }
}

export function cropBase64(element, elementId, options) {
    const instance = _instances[elementId];

    if (instance && instance.cropper) {
        const canvas = instance.cropper.getCroppedCanvas(options);
        return canvas.toDataURL();
    }

    return "";
}

export function executeCropperAction(element, elementId, methodName, ...args) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    const method = instance.cropper[methodName];
    if (!method) {
        console.error("Blazorise Image Cropper: Unknown cropperjs method " + methodName);
        return;
    }

    return method.apply(instance.cropper, ...args);
}

function registerEvents(element, dotNetAdapter) {
    element.addEventListener('cropstart', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "CropStart");
    });

    element.addEventListener('cropmove', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "CropMove");
    });

    element.addEventListener('cropend', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "CropEnd");
    });

    element.addEventListener('crop', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "Crop");
    });

    element.addEventListener('zoom', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "Zoom");
    });
}

function invokeDotNetMethodAsync(dotNetAdapter, methodName, ...args) {
    dotNetAdapter.invokeMethodAsync(methodName, ...args)
        .catch((reason) => {
            console.error(reason);
        });
}
