import './vendors/lottie.js?v=1.1.2.0';

import {getRequiredElement} from "../Blazorise/utilities.js?v=1.1.2.0";

const _instances = [];

export function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;
    
    options.container = element;
    
    const instance = {
        options: options,
        animation: null
    }
    
    instance.animation = lottie.loadAnimation(options);
    registerToEvents(instance);

    _instances[elementId] = instance;
    
    return instance.animation;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        
        if(instance.animation) {
            instance.animation.destroy();
        }

        delete instances[elementId];
    }
}

function invokeCallbackAsync(callback, ...args) {
    callback.invokeMethodAsync('InvokeAsync', ...args)
        .catch((reason) => {
            console.error(reason);
        });
}

function registerToEvents(instance) {
    instance.animation.addEventListener('enterFrame', (event) => {
        if(instance.options.enterFrameCallback)
        {
            invokeCallbackAsync(instance.options.enterFrameCallback, event);
        }
    });
}