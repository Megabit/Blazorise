import "./vendors/cropper.min.js?v=1.2.0.0";

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

    options.ready = () => {
        if (options.radius) {
            const cropper = instance.cropper.cropper;
            const borderRadius = options.radius + '%';
            cropper.getElementsByClassName('cropper-face')[0].style.borderRadius = borderRadius;
            cropper.getElementsByClassName('cropper-view-box')[0].style.borderRadius = borderRadius;
        }
    };

    instance.cropper = new Cropper(element, options);

    registerEvents(element, dotNetAdapter);

    _instances[elementId] = instance;
}

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    if (instance.cropper) {
        instance.cropper.destroy();
    }

    initialize(instance.adapter, element, elementId, options);
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
