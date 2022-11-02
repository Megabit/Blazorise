import './vendors/lottie.js?v=1.1.2.0';

import {getRequiredElement} from "../Blazorise/utilities.js?v=1.1.2.0";

export function initializeAnimation(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
    {
        return;
    }
    
    options.container = element;
    const animation = lottie.loadAnimation(options);
    animation.setDirection( options.direction );
    animation.setSpeed( options.speed );
    
    animation.registeredEvents = new Set(options.registeredEvents);
    registerEvents(dotNetAdapter, animation);
    
    return animation;
}

function invokeDotNetMethodAsync(dotNetAdapter, methodName, ...args) {
    dotNetAdapter.invokeMethodAsync(methodName, ...args)
        .catch((reason) => {
            console.error(reason);
        });
}

function registerEvents(dotNetAdapter, animation) {
    animation.updateRegisteredEvents = (registeredEvents) => {
        animation.registeredEvents = new Set(registeredEvents);
    }
    
    animation.addEventListener('enterFrame', (event) => {
        if(animation.registeredEvents.has('enterFrame'))
        {
            invokeDotNetMethodAsync(dotNetAdapter, "NotifyEnteredFrame", event);
        }
    });
}