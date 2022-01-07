import "./vendors/plyr.js";
import "./vendors/dash.js";

import { getRequiredElement } from "../Blazorise/utilities.js";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"https://cdn.plyr.io/3.6.12/plyr.css\" />");

const _instances = [];

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    if (options.streaming) {
        const dash = dashjs.MediaPlayer().create();
        dash.initialize(element, options.source, options.autoPlay);
        window.dash = dash;
    }

    const player = new Plyr(element, {
        captions: {
            active: true,
            update: true
        }
    });

    window.player = player;

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