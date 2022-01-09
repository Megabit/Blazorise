import "./vendors/plyr.js";
import "./vendors/dash.js";
import "./vendors/hls.js";

import { getRequiredElement } from "../Blazorise/utilities.js";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"https://cdn.plyr.io/3.6.12/plyr.css\" />");

const _instances = [];

export function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    if (options.streaming) {
        if (Hls.isSupported()) {
            var hls = new Hls({
                debug: false,
            });

            hls.loadSource(options.source);
            hls.attachMedia(element);

            window.hls = hls;
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

    player.on('progress', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyProgress", event.detail.plyr.buffered || 0);
    });

    player.on('playing', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyPlaying");
    });

    player.on('play', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyPlay");
    });

    player.on('pause', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyPause");
    });

    player.on('timeupdate', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyTimeUpdate", event.detail.plyr.currentTime || 0);
    });

    player.on('volumechange', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyVolumeChange", event.detail.plyr.volume || 0, event.detail.plyr.muted || false);
    });

    player.on('seeking', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifySeeking");
    });

    player.on('seeked', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifySeeked");
    });

    player.on('ratechange', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyRateChange", event.detail.plyr.speed || 0);
    });

    player.on('ended', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyEnded");
    });

    player.on('enterfullscreen', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyEnterFullScreen");
    });

    player.on('exitfullscreen', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyExitFullScreen");
    });

    player.on('captionsenabled', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyCaptionsEnabled");
    });

    player.on('captionsdisabled', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyCaptionsDisabled");
    });

    player.on('languagechange', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyLanguageChange", event.detail.plyr.language);
    });

    player.on('controlshidden', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyControlsHidden");
    });

    player.on('controlsshown', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyControlsShown");
    });

    player.on('ready', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyReady");
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

function invokeDotNetMethodAsync(dotNetAdapter, methodName, ...args) {
    dotNetAdapter.invokeMethodAsync(methodName, ...args)
        .catch((reason) => {
            console.error(reason);
        });
}