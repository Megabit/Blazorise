import './vendors/lottie.js?v=1.8.1.0';

import { getRequiredElement } from "../Blazorise/utilities.js?v=1.8.1.0";

/**
 * Initializes a new animation instance
 * @param dotNetAdapter Reference to the dotnet object
 * @param element Container element reference
 * @param elementId Container element Id
 * @param options Animation configuration options
 * @returns Reference to the Lottie Animation
 */
export function initializeAnimation(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element) {
        return;
    }

    options.container = element;

    const animation = lottie.loadAnimation(options);
    animation.setDirection(options.direction);
    animation.setSpeed(options.speed);

    // Add a function to set a flag for whether or not we're sending current frame updates
    animation.setSendCurrentFrame = function (shouldSend) {
        this.sendCurrentFrame = shouldSend;
    }

    // Add a function to set the loop property
    animation.setLoop = function (loop) {
        this.loop = loop;
    }

    animation.setSendCurrentFrame(options.sendCurrentFrame);
    registerEvents(dotNetAdapter, animation);

    return animation;
}

function invokeDotNetMethodAsync(dotNetAdapter, methodName, ...args) {
    return dotNetAdapter.invokeMethodAsync(methodName, ...args)
        .catch((reason) => {
            console.error(reason);
        });
}

function registerEvents(dotNetAdapter, animation) {
    animation.addEventListener('enterFrame', async (event) => {
        if (!animation.sendCurrentFrame || animation.frameChangeNotificationSent) {
            // We've either already sent an event that hasn't been processed yet,
            // or nobody is listening for the events. Skip sending this event to save
            // on bandwidth.
            return;
        }

        animation.frameChangeNotificationSent = true;
        
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyCurrentFrameChanged", event.currentTime)
            .then(() => {
                animation.frameChangeNotificationSent = false;
            })
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