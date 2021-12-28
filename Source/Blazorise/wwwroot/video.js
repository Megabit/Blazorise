import "./vendors/ShakaPlayer";
import * as utilities from "./utilities.js";

const _instancesInfos = [];

// Install built-in polyfills to patch browser incompatibilities.
shaka.polyfill.installAll();

export function initialize(dotnetAdapter, element, elementId, options) {
    element = utilities.getRequiredElement(element, elementId);

    if (!element)
        return;

    // Check to see if the browser supports the basic APIs Shaka needs.
    if (shaka.Player.isBrowserSupported()) {
        // Everything looks good!
        initPlayer();
    } else {
        // This browser does not have the minimum set of APIs we need.
        console.error('Browser not supported for Video player!');
    }

    _instancesInfos[elementId] = instanceInfo;
}

export function initPlayer() {
    // Create a Player instance.
    const video = document.getElementById('video');
    const player = new shaka.Player(video);

    // Attach player to the window to make it easy to access in the JS console.
    window.player = player;

    // Listen for error events.
    player.addEventListener('error', onErrorEvent);

    // Try to load a manifest.
    // This is an asynchronous process.
    try {
        await player.load(manifestUri);
        // This runs if the asynchronous load is successful.
        console.log('The video has now been loaded!');
    } catch (e) {
        // onError is executed if the asynchronous load fails.
        onError(e);
    }

    //Not sure about this one!! https://shaka-player-demo.appspot.com/docs/api/tutorial-basic-usage.html
    document.addEventListener('DOMContentLoaded', initApp);
}

export function onErrorEvent(event) {
    // Extract the shaka.util.Error object from the event.
    onError(event.detail);
}

export function onError(error) {
    // Log the error.
    console.error('Error code', error.code, 'object', error);
}

export function destroy(element, elementId) {
    const instanceInfo = _instancesInfos || {};
    delete instanceInfo[elementId];
}