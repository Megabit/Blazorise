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

    animation.setLoop = function(loop) {
        this.loop = loop;
    }
    
    registerEvents(dotNetAdapter, animation);
    
    return animation;
}

async function invokeDotNetMethodAsync(dotNetAdapter, methodName, ...args) {
    await dotNetAdapter.invokeMethodAsync(methodName, ...args)
        .catch((reason) => {
            console.error(reason);
        });
}

function registerEvents(dotNetAdapter, animation) {
    animation.addEventListener('enterFrame', async (event) => {
        
        if( animation.frameChangeNotificationSent )
        {
            return;
        }
        
        animation.frameChangeNotificationSent = true;
        
        await invokeDotNetMethodAsync(dotNetAdapter, "NotifyCurrentFrameChanged", event.currentTime);

        animation.frameChangeNotificationSent = false;
    });

    animation.addEventListener('DOMLoaded', () => {
        const dataInfo = {
            currentFrame: animation.currentFrame,
            totalFrames: animation.totalFrames
        };
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyLoaded", dataInfo);
    });

    animation.addEventListener('loopComplete', () => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyLoopCompleted");
    });

    animation.addEventListener('complete', () => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyCompleted");
    });
}