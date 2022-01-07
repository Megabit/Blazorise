import "./vendors/plyr.js";
import "./vendors/dash.js";

import { getRequiredElement } from "./utilities.js";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"https://cdn.plyr.io/3.6.12/plyr.css\" />");

const _instances = [];

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const dash = dashjs.MediaPlayer().create();
    dash.initialize(element, options.source, true);

    const player = new Plyr(element, {
        captions: {
            active: true,
            update: true
        }
    });

    window.player = player;
    window.dash = dash;

    _instances[elementId] = player;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        instance.destroy();

        delete instances[elementId];
    }
}

////import "./vendors/ShakaPlayer";
////import * as utilities from "./utilities.js";

//const _instancesInfos = [];

//// Install built-in polyfills to patch browser incompatibilities.
//shaka.polyfill.installAll();

//export function initialize(element, elementId, options) {
//    element = utilities.getRequiredElement(element, elementId);

//    if (!element)
//        return;

//    // Check to see if the browser supports the basic APIs Shaka needs.
//    if (shaka.Player.isBrowserSupported()) {
//        // Everything looks good!
//        initPlayer(element, options);
//    } else {
//        // This browser does not have the minimum set of APIs we need.
//        console.error('Browser not supported for Video player!');
//    }

//    _instancesInfos[elementId] = instanceInfo;
//}

//export function initPlayer(element, options) {
//    const player = new shaka.Player(element);

//    // Attach player to the window to make it easy to access in the JS console.
//    window.player = player;

//    // Listen for error events.
//    player.addEventListener('error', onErrorEvent);

//    // Try to load a manifest.
//    // This is an asynchronous process.
//    try {
//        await player.load(options.manifestUri);
//        // This runs if the asynchronous load is successful.
//        console.log('The video has now been loaded!');
//    } catch (e) {
//        // onError is executed if the asynchronous load fails.
//        onError(e);
//    }

//    //Not sure about this one!! https://shaka-player-demo.appspot.com/docs/api/tutorial-basic-usage.html
//    document.addEventListener('DOMContentLoaded', initApp);
//}

//export function onErrorEvent(event) {
//    // Extract the shaka.util.Error object from the event.
//    onError(event.detail);
//}

//export function onError(error) {
//    // Log the error.
//    console.error('Error code', error.code, 'object', error);
//}

//export function destroy(element, elementId) {
//    const instanceInfo = _instancesInfos || {};
//    delete instanceInfo[elementId];
//}