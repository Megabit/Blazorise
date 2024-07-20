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
        hideControlsOnMouseLeave: options.automaticallyHideControls,
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

    plyr.layout.controls = options.controlsList;

    if (options.source.tracks && options.source.tracks.length > 0) {
        plyr.player.textTracks.clear();

        for (const track of options.source.tracks) {
            plyr.player.textTracks.add(track);
        }
    }

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
            applyHlsProtectionData(provider.instance, options.protection);

            instance.hls = provider.instance;
        }
    });

    registerToEvents(dotNetAdapter, instance.player);

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

        if (options.protectionType.changed || options.protectionServerUrl.changed || options.protectionServerCertificateUrl.changed || options.protectionHttpRequestHeaders.changed) {
            updateProtection(element, elementId, {
                data: options.protectionData ? options.protectionData.value : null,
                type: options.protectionType ? options.protectionType.value : null,
                serverUrl: options.protectionServerUrl ? options.protectionServerUrl.value : null,
                serverCertificateUrl: options.protectionServerCertificateUrl ? options.protectionServerCertificateUrl.value : null,
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

            instance.player.textTracks.clear();

            if (source.tracks && source.tracks.length > 0) {
                for (const track of source.tracks) {
                    instance.player.textTracks.add(track);
                }
            }
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

        if (instance.hls) {
            applyHlsProtectionData(instance.hls, protection);
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
        const toggle = instance.player.paused;

        if (toggle) {
            instance.player.play();
        } else {
            instance.player.pause();
        }
    }
}

export function stop(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.pause();
        instance.player.currentTime = 0;
    }
}

export function restart(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.currentTime = 0;
    }
}

export function rewind(element, elementId, seekTime) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.currentTime -= seekTime;
    }
}

export function forward(element, elementId, seekTime) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.currentTime += seekTime;
    }
}

export function increaseVolume(element, elementId, step) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.volume += step;
    }
}

export function decreaseVolume(element, elementId, step) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.volume -= step;
    }
}

export function toggleCaptions(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        const toggle = !instance.player.textTracks.selected;
        const controller = instance.player.remoteControl;

        if (toggle) {
            controller.showCaptions();
        } else {
            controller.disableCaptions();
        }
    }
}

export function enterFullscreen(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.enterFullscreen();
    }
}

export function exitFullscreen(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.exitFullscreen();
    }
}

export function toggleFullscreen(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        const toggle = !instance.player.fullscreen;

        if (toggle) {
            instance.player.enterFullscreen();
        }
        else {
            instance.player.exitFullscreen();
        }
    }
}

export function airplay(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.requestAirPlay();
    }
}

export function toggleControls(element, elementId, toggle) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        const controls = instance.player.controls;

        if (toggle) {
            controls.show();
        } else {
            controls.hide();
        }
    }
}

export function showTextTrack(element, elementId, textTrackId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        const remote = instance.player.remoteControl;

        remote.changeTextTrackMode(textTrackId, 'showing');
    }
}

export function hideTextTrack(element, elementId, textTrackId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        const remote = instance.player.remoteControl;

        remote.changeTextTrackMode(textTrackId, 'hidden');
    }
}

export function addTextTrack(element, elementId, track) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.textTracks.add(track);
    }
}

export function clearTextTracks(element, elementId) {
    const instance = _instances[elementId];

    if (instance && instance.player) {
        instance.player.textTracks.clear();
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

function applyHlsProtectionData(hls, protection) {
    if (hls && protection) {
        if (protection.type === "FairPlay") {
            hls.config.emeEnabled = true;
            hls.config.drmSystems = {
                'com.apple.fps': {
                    licenseUrl: protection.serverUrl,
                    serverCertificateUrl: protection.serverCertificateUrl,
                    httpRequestHeaders: protection.httpRequestHeaders ? {
                        'X-AxDRM-Message': protection.httpRequestHeaders
                    } : null
                }
            };
        }
        else if (protection.type === "PlayReady") {
            hls.config.emeEnabled = true;
            hls.config.drmSystems = {
                'com.microsoft.playready': {
                    licenseUrl: protection.serverUrl,
                    httpRequestHeaders: protection.httpRequestHeaders ? {
                        'X-AxDRM-Message': protection.httpRequestHeaders
                    } : null
                }
            };
        }
        else if (protection.type === "Widevine") {
            hls.config.emeEnabled = true;
            hls.config.drmSystems = {
                'com.widevine.alpha': {
                    licenseUrl: protection.serverUrl,
                    httpRequestHeaders: protection.httpRequestHeaders ? {
                        'X-AxDRM-Message': protection.httpRequestHeaders
                    } : null
                }
            };
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
    player.addEventListener('progress', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyProgress", event.detail.timeStamp || 0);
    });

    player.addEventListener('playing', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyPlaying");
    });

    player.addEventListener('play', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyPlay");
    });

    player.addEventListener('pause', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyPause");
    });

    player.addEventListener('time-update', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyTimeUpdate", event.detail.currentTime || 0);
    });

    player.addEventListener('volume-change', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyVolumeChange", event.detail.volume || 0, event.detail.muted || false);
    });

    player.addEventListener('seeking', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifySeeking");
    });

    player.addEventListener('seeked', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifySeeked");
    });

    player.addEventListener('rate-change', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyRateChange", event.detail || 0);
    });

    player.addEventListener('ended', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyEnded");
    });

    player.addEventListener('enter-fullscreen', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyFullScreenEntered");
    });

    player.addEventListener('exit-fullscreen', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyFullScreenExited");
    });

    player.addEventListener('captionsenabled', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyCaptionsEnabled");
    });

    player.addEventListener('text-track-change', (event) => {
        if (event.detail) {
            invokeDotNetMethodAsync(dotNetAdapter, "NotifyCaptionsEnabled");
        }
        else {
            invokeDotNetMethodAsync(dotNetAdapter, "NotifyCaptionsDisabled");
        }
    });

    // Disabled because it's not supported at the moment.
    //player.addEventListener('language-change', (event) => {
    //    invokeDotNetMethodAsync(dotNetAdapter, "NotifyLanguageChange", event.detail.plyr.language);
    //});

    player.addEventListener('controls-change', (event) => {
        if (event.detail) {
            invokeDotNetMethodAsync(dotNetAdapter, "NotifyControlsShown");
        }
        else {
            invokeDotNetMethodAsync(dotNetAdapter, "NotifyControlsHidden");
        }
    });

    player.addEventListener('can-play', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyReady");
    });

    player.addEventListener('quality-change', (event) => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyQualityChange", event.detail.height);
    });
}