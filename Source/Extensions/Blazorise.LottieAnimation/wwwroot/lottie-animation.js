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
    
    return animation;
}

// function invokeCallbackAsync(callback, ...args) {
//     callback.invokeMethodAsync('InvokeAsync', ...args)
//         .catch((reason) => {
//             console.error(reason);
//         });
// }
//
// function registerToEvents(instance) {
//     instance.animation.addEventListener('enterFrame', (event) => {
//         if(instance.options.enterFrameCallback)
//         {
//             invokeCallbackAsync(instance.options.enterFrameCallback, event);
//         }
//     });
// }