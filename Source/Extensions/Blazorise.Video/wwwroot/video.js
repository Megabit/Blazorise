import "./vendors/plyr.js";
import "./vendors/dash.js";
import "./vendors/hls.js";

import { getRequiredElement } from "../Blazorise/utilities.js";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"https://cdn.plyr.io/3.6.12/plyr.css\" />");

const _instances = [];

export function initialize(element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    if (options.streaming) {
        if (Hls.isSupported()) {
            var hls = new Hls({
                debug: true,
            });

            hls.loadSource(options.source);
            hls.attachMedia(element);

            window.hls = hls;
        }
        else if (element.canPlayType('application/vnd.apple.mpegurl')) {
            element.src = options.source;
            element.addEventListener('canplay', function () {
                element.play();
            });
        }
    }

    const player = new Plyr(element, {
        hideControls: options.automaticallyHideControls,
        autopause: options.autoPause || true,
        seekTime: options.seekTime || 10,
        volume: options.volume || 1,
        muted: options.muted || false,
        clickToPlay: options.clickToPlay || true,
        disableContextMenu: options.disableContextMenu || true,
        resetOnEnd: options.resetOnEnd || false,
        ratio: options.ratio,
        invertTime: options.invertTime || true,
        previewThumbnails: {
            enabled: options.poster && options.poster.length > 0,
            src: options.poster
        },
        captions: {
            active: true,
            update: true
        }
    });

    window.player = player;

    player.on('ready', (event) => {
        console.log(event.detail.plyr);
    });

    player.on('progress', (event) => {
        console.log(event.detail.plyr);
    });

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