import { Plyr } from "./vendors/plyr.js?v=1.5.3.0";

import { getRequiredElement, isString } from "../Blazorise/utilities.js?v=1.5.3.0";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"_content/Blazorise.Video/vendors/player.css\" />");
document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"_content/Blazorise.Video/vendors/plyr.css\" />");

const _instances = [];

export function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = {
        options: options,
        player: null,
        hls: null,
        dash: null,
    };

    const plyr = new Plyr(element, {
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
        controls: options.controlsList,
        settings: options.settingsList,
        quality: {
            default: options.defaultQuality || 576,
            options: options.availableQualities || [4320, 2880, 2160, 1440, 1080, 720, 576, 480, 360, 240]
        },
        previewThumbnails: {
            enabled: options.poster && options.poster.length > 0,
            src: options.poster
        },
        captions: {
            active: true,
            update: true
        }
    });

    instance.player = plyr.player;

    instance.player.addEventListener('provider-change', (event) => {
        const provider = event.detail;

        if (provider?.type === 'hls') {
            provider.library = '_content/Blazorise.Video/vendors/hls.js?v=1.5.3.0';
        }
        else if (provider?.type === 'dash') {
            provider.library = '_content/Blazorise.Video/vendors/dash.js?v=1.5.3.0';
        }
    });

    instance.player.addEventListener('provider-setup', (event) => {
        const provider = event.detail;

        if (provider.type === 'dash' && provider.instance) {
            applyDashProtectionData(provider.instance, options.protection);

            instance.dash = provider.instance;
        } else if (provider.type === 'hls' && provider.instance) {
            instance.hls = provider.instance;
        }
    });

    instance.player.addEventListener('hls-manifest-loaded', (event) => {
        const levelLoadedData = event.detail; // `HLS.ManifestLoadedData`
        // ...
    });


    //registerToEvents(dotNetAdapter, instance.player);

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        if (instance.player) {
            instance.player.destroy();
        }

        if (instance.dash) {
            instance.dash.destroy();
        }

        if (instance.hls) {
            instance.hls.destroy();
        }

        delete instances[elementId];
    }
}

export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];

    if (instance && instance.player && options) {
        if (options.source.changed) {
            updateSource(element, elementId, options.source.value);
        }

        if (options.protectionType.changed || options.protectionServerUrl.changed || options.protectionHttpRequestHeaders.changed) {
            updateProtection(element, elementId, {
                data: options.protectionData ? options.protectionData.value : null,
                type: options.protectionType ? options.protectionType.value : null,
                serverUrl: options.protectionServerUrl ? options.protectionServerUrl.value : null,
                httpRequestHeaders: options.protectionHttpRequestHeaders ? options.protectionHttpRequestHeaders.value : null
            });
        }

        if (options.currentTime.changed) {
            instance.player.currentTime = options.currentTime.value;
        }

        if (options.volume.changed) {
            instance.player.volume = options.volume.value;
        }
    }
}

export function updateSource(element, elementId, source, protection) {
    const instance = _instances[elementId];

    if (instance) {
        const sourceUrl = extractSingleSourceUrl(source);

        if (instance.player) {
            instance.player.src = sourceUrl;
        }

        if (instance.dash) {
            instance.dash.attachSource(sourceUrl);
        }

        if (instance.hls) {
            instance.hls.loadSource(sourceUrl);
        }
    }

    if (protection) {
        updateProtection(element, elementId, protection);
    }
}

export function updateProtection(element, elementId, protection) {
    const instance = _instances[elementId];

    if (instance) {
        if (instance.dash) {
            applyDashProtectionData(instance.dash, protection);
        }
    }
}

export function play(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.play();
    }
}

export function pause(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.pause();
    }
}

export function togglePlay(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.togglePlay();
    }
}

export function stop(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.stop();
    }
}

export function restart(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.restart();
    }
}

export function rewind(element, elementId, seekTime) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.rewind(seekTime);
    }
}

export function forward(element, elementId, seekTime) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.forward(seekTime);
    }
}

export function increaseVolume(element, elementId, step) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.increaseVolume(step);
    }
}

export function decreaseVolume(element, elementId, step) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.decreaseVolume(step);
    }
}

export function toggleCaptions(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.toggleCaptions();
    }
}

export function enterFullscreen(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.fullscreen.enter();
    }
}

export function exitFullscreen(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.fullscreen.exit();
    }
}

export function toggleFullscreen(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.fullscreen.toggle();
    }
}

export function airplay(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.airplay();
    }
}

export function toggleControls(element, elementId, toggle) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.toggleControls(toggle);
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

function applyDashProtectionData(dash, protection) {
    if (dash && protection) {
        if (protection.type === "PlayReady") {
            const protectionData = protection.data ? protection.data : {
                "com.microsoft.playready": {
                    "serverURL": protection.serverUrl,
                    "httpRequestHeaders": protection.httpRequestHeaders ? {
                        "X-AxDRM-Message": protection.httpRequestHeaders
                    } : null
                }
            };

            dash.setProtectionData(protectionData);
        }
        else if (protection.type === "Widevine") {
            const protectionData = protection.data ? protection.data : {
                "com.widevine.alpha": {
                    "serverURL": protection.serverUrl,
                    "httpRequestHeaders": protection.httpRequestHeaders ? {
                        "X-AxDRM-Message": protection.httpRequestHeaders
                    } : null
                }
            };

            dash.setProtectionData(protectionData);
        }
    }
}

function invokeDotNetMethodAsync(dotNetAdapter, methodName, ...args) {
    dotNetAdapter.invokeMethodAsync(methodName, ...args)
        .catch((reason) => {
            console.error(reason);
        });
}

function registerToEvents(dotNetAdapter, player) {
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

    player.on('qualitychange', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyQualityChange", event.detail.quality);
    });
}