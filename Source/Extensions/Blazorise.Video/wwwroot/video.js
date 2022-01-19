import "./vendors/plyr.js";
import "./vendors/dash.js";
import "./vendors/hls.js";

import { getRequiredElement, isString } from "../Blazorise/utilities.js";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"https://cdn.plyr.io/3.6.12/plyr.css\" />");

const _instances = [];

export function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    if (options.streamingLibrary !== "Html5") {
        const source = extractSingleSourceUrl(options.source);

        if (options.streamingLibrary === "Hls") {
            if (Hls.isSupported()) {
                const hls = new Hls({
                    debug: false,
                });
                hls.loadSource(source);
                hls.attachMedia(element);
                window.hls = hls;
            }
        } else if (options.streamingLibrary === "Dash") {
            const dash = dashjs.MediaPlayer().create();
            dash.initialize(element, source, options.autoPlay || false);

            if (options.protection) {
                if (options.protection && options.protection.type === "PlayReady") {
                    const protectionData = {
                        "com.microsoft.playready": {
                            "serverURL": options.protection.serverUrl,
                            "httpRequestHeaders": {
                                "X-AxDRM-Message": options.protection.httpRequestHeaders
                            }
                        }
                    };

                    dash.setProtectionData(protectionData);
                }
                else if (options.protection && options.protection.type === "Widevine") {
                    const protectionData = {
                        "com.widevine.alpha": {
                            "serverURL": options.protection.serverUrl,
                            "httpRequestHeaders": {
                                "X-AxDRM-Message": options.protection.httpRequestHeaders
                            }
                        }
                    };

                    dash.setProtectionData(protectionData);
                }
            }

            window.dash = dash;
        }
    }

    const player = new Plyr(element, {
        source: options.source,
        poster: options.poster,
        hideControls: options.automaticallyHideControls,
        autopause: options.autoPause || true,
        seekTime: options.seekTime || 10,
        volume: options.volume || 1,
        currentTime: options.currentTime || 0,
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
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyFullScreenEntered");
    });

    player.on('exitfullscreen', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyFullScreenExited");
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

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (instance && options) {
        if (options.source.changed) {
            instance.source = options.source.value;
        }

        if (options.currentTime.changed) {
            instance.currentTime = options.currentTime.value;
        }

        if (options.volume.changed) {
            instance.volume = options.volume.value;
        }
    }
}

export function updateSource(element, elementId, source) {
    const instance = _instances[elementId];

    if (instance) {
        instance.source = source;
    }
}

export function play(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        instance.play();
    }
}

export function pause(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        instance.pause();
    }
}

export function togglePlay(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        instance.togglePlay();
    }
}

export function stop(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        instance.stop();
    }
}

export function restart(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        instance.restart();
    }
}

export function rewind(element, elementId, seekTime) {
    const instance = _instances[elementId];

    if (instance) {
        instance.rewind(seekTime);
    }
}

export function forward(element, elementId, seekTime) {
    const instance = _instances[elementId];

    if (instance) {
        instance.forward(seekTime);
    }
}

export function increaseVolume(element, elementId, step) {
    const instance = _instances[elementId];

    if (instance) {
        instance.increaseVolume(step);
    }
}

export function decreaseVolume(element, elementId, step) {
    const instance = _instances[elementId];

    if (instance) {
        instance.decreaseVolume(step);
    }
}

export function toggleCaptions(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        instance.toggleCaptions();
    }
}

export function enterFullscreen(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        instance.fullscreen.enter();
    }
}

export function exitFullscreen(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        instance.fullscreen.exit();
    }
}

export function toggleFullscreen(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        instance.fullscreen.toggle();
    }
}

export function airplay(element, elementId) {
    const instance = _instances[elementId];

    if (instance) {
        instance.airplay();
    }
}

export function toggleControls(element, elementId, toggle) {
    const instance = _instances[elementId];

    if (instance) {
        instance.toggleControls(toggle);
    }
}

function extractSingleSourceUrl(source) {
    if (!source)
        return null;

    if (isString(source)) {
        return source;
    }
    else if (source.sources && source.sources.length > 0) {
        return source.sources[0].src;
    }

    return null;
}

function invokeDotNetMethodAsync(dotNetAdapter, methodName, ...args) {
    dotNetAdapter.invokeMethodAsync(methodName, ...args)
        .catch((reason) => {
            console.error(reason);
        });
}