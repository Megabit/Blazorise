import { getRequiredElement, insertCSSIntoDocumentHead, isString, firstNonNull, registerDisconnectCleanup, unregisterDisconnectCleanup } from "../Blazorise/utilities.js?v=2.3.0.0";

const videoJsVendorRoot = "./vendors/videojs";
const videoJsPlayerUrl = `${videoJsVendorRoot}/videojs.js`;
const videoJsHlsUrl = `${videoJsVendorRoot}/hlsjs-video.js`;
const videoJsDashUrl = `${videoJsVendorRoot}/dash-video.js`;
const videoJsYouTubeUrl = `${videoJsVendorRoot}/youtube-video.js`;
const videoJsVimeoUrl = `${videoJsVendorRoot}/vimeo-video.js`;

insertCSSIntoDocumentHead("_content/Blazorise.Video/vendors/videojs/videojs.css?v=2.3.0.0");

const instances = new Map();
const modulePromises = new Map();
const configuredCaptionsRadioGroups = new WeakSet();

export async function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = {
        options: options,
        player: element,
        media: null,
        controls: null,
        controlsVisible: false,
        controlsTimer: null,
        textTracks: null,
        activeLanguage: null,
        qualityRenditions: null,
        qualityObserver: null,
        abortController: new AbortController(),
        mediaAbortController: null,
        disconnectCleanupId: null,
        destroyed: false,
        protection: options.protection,
        protectionGeneration: 0,
        fullscreen: false,
        fullWindow: false,
        dotNetAdapter: dotNetAdapter,
    };

    instances.set(elementId, instance);
    instance.disconnectCleanupId = registerDisconnectCleanup(element, () => destroy(null, elementId, false));

    try {
        await loadVideoJs(element, options.streamingLibrary);

        if (instance.destroyed)
            return;

        instance.media = getMediaElement(element);

        if (!instance.media)
            throw new Error(`Unable to find the Video.js media element for '${elementId}'.`);

        instance.mediaAbortController = new AbortController();
        applyMediaOptions(instance);
        registerToEvents(dotNetAdapter, instance);
        registerPlayerEvents(dotNetAdapter, instance);
        connectTextTrackEvents(dotNetAdapter, instance);
        setupControls(dotNetAdapter, instance);
        setupCompatibilityControls(dotNetAdapter, instance);
        applyDefaultQualityWhenAvailable(dotNetAdapter, instance);
        applyProtectionWhenAvailable(instance);

        if (instance.media.readyState >= 3)
            invokeDotNetMethodAsync(dotNetAdapter, "NotifyReady");
    } catch (error) {
        destroy(null, elementId);
        throw error;
    }
}

export function destroy(element, elementId, unregisterCleanup = true) {
    const instance = instances.get(elementId);

    if (!instance)
        return;

    instance.destroyed = true;
    instance.protectionGeneration++;
    instance.mediaAbortController?.abort();
    instance.abortController.abort();
    clearTimeout(instance.controlsTimer);
    instance.qualityObserver?.disconnect();

    if (instance.fullWindow)
        exitFullWindow(instance, false);

    if (unregisterCleanup)
        unregisterDisconnectCleanup(instance.disconnectCleanupId);

    try {
        instance.media?.pause();
    } catch (error) {
        console.error(error);
    }

    instance.player = null;
    instance.media = null;
    instance.controls = null;
    instance.controlsTimer = null;
    instance.textTracks = null;
    instance.qualityRenditions = null;
    instance.qualityObserver = null;
    instance.mediaAbortController = null;
    instance.disconnectCleanupId = null;

    instances.delete(elementId);
}

export async function updateOptions(element, elementId, options) {
    const instance = instances.get(elementId);

    if (!instance || !instance.media || !options)
        return;

    const sourceChanged = options.source?.changed === true;
    const streamingLibraryChanged = options.streamingLibrary?.changed === true;

    applyChangedOption(instance, options, "thumbnails");
    applyChangedOption(instance, options, "streamingLibrary");

    if (sourceChanged)
        await updateSource(element, elementId, options.source.value);
    else if (streamingLibraryChanged) {
        await loadVideoJs(instance.player, instance.options.streamingLibrary);

        if (instance.destroyed || instances.get(elementId) !== instance)
            return;

        rebindRenderedMedia(instance);
        await updateSource(element, elementId, instance.options.source);
    }

    if (instance.destroyed || instances.get(elementId) !== instance || !instance.media)
        return;

    if (options.protectionType?.changed
        || options.protectionData?.changed
        || options.protectionServerUrl?.changed
        || options.protectionServerCertificateUrl?.changed
        || options.protectionHttpRequestHeaders?.changed) {
        const protection = { ...instance.protection };

        if (options.protectionType?.changed)
            protection.type = normalizeProtectionType(options.protectionType.value);
        if (options.protectionData?.changed)
            protection.data = options.protectionData.value;
        if (options.protectionServerUrl?.changed)
            protection.serverUrl = options.protectionServerUrl.value;
        if (options.protectionServerCertificateUrl?.changed)
            protection.serverCertificateUrl = options.protectionServerCertificateUrl.value;
        if (options.protectionHttpRequestHeaders?.changed)
            protection.httpRequestHeaders = options.protectionHttpRequestHeaders.value;

        updateProtection(element, elementId, protection);
    }

    if (options.currentTime?.changed)
        setCurrentTime(instance.media, options.currentTime.value);

    if (options.volume?.changed)
        instance.media.volume = clamp(options.volume.value, 0, 1);

    applyChangedOption(instance, options, "controls");
    applyChangedOption(instance, options, "controlsDelay");
    applyChangedOption(instance, options, "automaticallyHideControls");
    applyChangedOption(instance, options, "autoPause");
    applyChangedOption(instance, options, "autoPlay");
    applyChangedOption(instance, options, "muted");
    applyChangedOption(instance, options, "clickToPlay");
    applyChangedOption(instance, options, "disableContextMenu");
    applyChangedOption(instance, options, "resetOnEnd");
    applyChangedOption(instance, options, "aspectRatio");
    applyChangedOption(instance, options, "invertTime");
    applyChangedOption(instance, options, "controlsList");
    applyChangedOption(instance, options, "settingsList");
    applyChangedOption(instance, options, "doubleClickToFullscreen");
    applyChangedOption(instance, options, "availableQualities");
    applyChangedOption(instance, options, "defaultQuality");

    if (options.autoPlay?.changed)
        instance.media.autoplay = options.autoPlay.value;

    if (options.muted?.changed)
        instance.media.muted = options.muted.value;

    if (options.thumbnails?.changed && !sourceChanged)
        updateTextTrackElements(instance.media, instance.options.source?.tracks, options.thumbnails.value);

    const container = getFullscreenTarget(instance);

    if (options.aspectRatio?.changed && container) {
        if (options.aspectRatio.value)
            container.style.aspectRatio = `${options.aspectRatio.value}`;
        else
            container.style.removeProperty("aspect-ratio");
    }

    refreshControls(instance, instance.dotNetAdapter);
    applyDefaultQualityWhenAvailable(instance.dotNetAdapter, instance);
}

export async function updateSource(element, elementId, source, protection) {
    const instance = instances.get(elementId);

    if (!instance || !instance.media)
        return;

    instance.options.source = source;

    await loadVideoJs(instance.player, instance.options.streamingLibrary);

    if (instance.destroyed || instances.get(elementId) !== instance)
        return;

    rebindRenderedMedia(instance);

    if (!instance.media)
        return;

    if (instance.media.localName === "video" || instance.media.localName === "audio")
        updateNativeSources(instance.media, source);
    else
        instance.media.src = extractSingleSourceUrl(source) || "";

    updateTextTrackElements(instance.media, source?.tracks, instance.options.thumbnails);
    connectTextTrackEvents(instance.dotNetAdapter, instance);
    applyDefaultQualityWhenAvailable(instance.dotNetAdapter, instance);

    if (arguments.length >= 4)
        instance.protection = protection?.type === "None" ? null : protection;

    if (!instance.protection)
        clearProtection(instance.media);

    applyProtectionWhenAvailable(instance);
}

export function updateProtection(element, elementId, protection) {
    const instance = instances.get(elementId);

    if (!instance)
        return;

    instance.protection = protection?.type === "None" ? null : protection;

    if (!instance.protection)
        clearProtection(instance.media);

    applyProtectionWhenAvailable(instance);
}

export function play(element, elementId) {
    return instances.get(elementId)?.media?.play();
}

export function pause(element, elementId) {
    instances.get(elementId)?.media?.pause();
}

export function togglePlay(element, elementId) {
    const media = instances.get(elementId)?.media;

    if (!media)
        return;

    return media.paused ? media.play() : media.pause();
}

export function stop(element, elementId) {
    const media = instances.get(elementId)?.media;

    if (!media)
        return;

    media.pause();
    setCurrentTime(media, 0);
}

export function restart(element, elementId) {
    const media = instances.get(elementId)?.media;

    if (media)
        setCurrentTime(media, 0);
}

export function rewind(element, elementId, seekTime) {
    const media = instances.get(elementId)?.media;

    if (media)
        setCurrentTime(media, Math.max(0, media.currentTime - seekTime));
}

export function forward(element, elementId, seekTime) {
    const media = instances.get(elementId)?.media;

    if (media)
        setCurrentTime(media, Math.min(media.duration || Number.MAX_VALUE, media.currentTime + seekTime));
}

export function increaseVolume(element, elementId, step) {
    const media = instances.get(elementId)?.media;

    if (media)
        media.volume = clamp(media.volume + step, 0, 1);
}

export function decreaseVolume(element, elementId, step) {
    const media = instances.get(elementId)?.media;

    if (media)
        media.volume = clamp(media.volume - step, 0, 1);
}

export function toggleCaptions(element, elementId) {
    const instance = instances.get(elementId);
    const captionsButton = getSkinElement(instance, "media-captions-button");

    if (captionsButton) {
        captionsButton.click();
        return;
    }

    const tracks = instance?.media?.textTracks;

    if (!tracks)
        return;

    const showingTrack = Array.from(tracks).find(track => isCaptionTrack(track) && track.mode === "showing");

    if (showingTrack) {
        showingTrack.mode = "disabled";
        return;
    }

    const firstCaptionTrack = Array.from(tracks).find(isCaptionTrack);

    if (firstCaptionTrack)
        firstCaptionTrack.mode = "showing";
}

export async function enterFullscreen(element, elementId) {
    const instance = instances.get(elementId);
    const fullscreenButton = getSkinElement(instance, "media-fullscreen-button");

    if (fullscreenButton && !isPlayerFullscreen(instance) && supportsFullscreen(getFullscreenTarget(instance))) {
        fullscreenButton.click();
        return;
    }

    const target = getFullscreenTarget(instance);

    if (!target)
        return;

    if (supportsFullscreen(target))
        await requestFullscreen(target);
    else
        enterFullWindow(instance);
}

export async function exitFullscreen(element, elementId) {
    const instance = instances.get(elementId);

    if (instance?.fullWindow) {
        exitFullWindow(instance);
        return;
    }

    const fullscreenButton = getSkinElement(instance, "media-fullscreen-button");

    if (fullscreenButton && isPlayerFullscreen(instance)) {
        fullscreenButton.click();
        return;
    }

    await exitDocumentFullscreen();
}

export async function toggleFullscreen(element, elementId) {
    const instance = instances.get(elementId);
    const fullscreenButton = getSkinElement(instance, "media-fullscreen-button");

    if (fullscreenButton) {
        fullscreenButton.click();
        return;
    }

    if (isPlayerFullscreen(instance))
        await exitFullscreen(element, elementId);
    else
        await enterFullscreen(element, elementId);
}

export async function airplay(element, elementId) {
    const instance = instances.get(elementId);
    const airplayButton = getSkinElement(instance, "media-airplay-button");

    if (airplayButton) {
        airplayButton.click();
        return;
    }

    const media = instance?.media;

    if (!media)
        return;

    if (typeof media.webkitShowPlaybackTargetPicker === "function")
        media.webkitShowPlaybackTargetPicker();
    else if (media.remote && typeof media.remote.prompt === "function")
        await media.remote.prompt();
}

export function toggleControls(element, elementId) {
    const instance = instances.get(elementId);

    if (!instance)
        return;

    setControlsVisible(instance, !instance.controlsVisible, instance.dotNetAdapter);
}

export function showTextTrack(element, elementId, textTrackId) {
    setTextTrackMode(instances.get(elementId)?.media, textTrackId, "showing");
}

export function hideTextTrack(element, elementId, textTrackId) {
    setTextTrackMode(instances.get(elementId)?.media, textTrackId, "hidden");
}

export function addTextTrack(element, elementId, track) {
    const media = instances.get(elementId)?.media;

    if (!media || !track)
        return;

    const trackElement = createTextTrackElement(track);
    trackElement.dataset.blazoriseDynamic = "true";
    media.appendChild(trackElement);
}

export function clearTextTracks(element, elementId) {
    const media = instances.get(elementId)?.media;

    if (!media)
        return;

    Array.from(media.querySelectorAll("track")).forEach(track => track.remove());

    if (media.textTracks) {
        for (const track of Array.from(media.textTracks))
            track.mode = "disabled";
    }
}

export function setPlaybackRate(element, elementId, rate) {
    const media = instances.get(elementId)?.media;

    if (media)
        media.playbackRate = rate;
}

async function loadVideoJs(player, streamingLibrary) {
    await importOnce(videoJsPlayerUrl);
    await customElements.whenDefined("video-player");
    await customElements.whenDefined("media-container");

    const renderedMedia = getMediaElement(player);

    if (renderedMedia?.localName === "youtube-video") {
        await importOnce(videoJsYouTubeUrl);
        await customElements.whenDefined("youtube-video");
    } else if (renderedMedia?.localName === "vimeo-video") {
        await importOnce(videoJsVimeoUrl);
        await customElements.whenDefined("vimeo-video");
    } else if (streamingLibrary === "Hls") {
        await importOnce(videoJsHlsUrl);
        await customElements.whenDefined("hlsjs-video");
    } else if (streamingLibrary === "Dash") {
        await importOnce(videoJsDashUrl);
        await customElements.whenDefined("dash-video");
    }
}

function importOnce(url) {
    let promise = modulePromises.get(url);

    if (!promise) {
        promise = import(url);
        modulePromises.set(url, promise);
    }

    return promise;
}

function getMediaElement(player) {
    return player?.querySelector("youtube-video, vimeo-video, hlsjs-video, dash-video, video, audio") || null;
}

function rebindRenderedMedia(instance) {
    const renderedMedia = getMediaElement(instance.player);

    if (!renderedMedia || renderedMedia === instance.media)
        return;

    instance.mediaAbortController?.abort();
    instance.mediaAbortController = new AbortController();
    instance.media = renderedMedia;
    instance.textTracks = null;
    instance.activeLanguage = null;
    instance.qualityRenditions = null;
    instance.qualityObserver?.disconnect();
    instance.qualityObserver = null;
    applyMediaOptions(instance);
    registerToEvents(instance.dotNetAdapter, instance);
}

function applyMediaOptions(instance) {
    const media = instance.media;
    const options = instance.options;
    const container = getFullscreenTarget(instance);

    media.autoplay = firstNonNull(options.autoPlay, false);
    media.muted = firstNonNull(options.muted, false);
    media.volume = clamp(firstNonNull(options.volume, 1), 0, 1);
    media.playsInline = true;

    if (options.currentTime)
        setCurrentTimeWhenReady(media, options.currentTime, instance.mediaAbortController.signal);

    if (options.aspectRatio && container)
        container.style.aspectRatio = `${options.aspectRatio}`;
}

function registerToEvents(dotNetAdapter, instance) {
    const media = instance.media;
    const signal = instance.mediaAbortController.signal;

    media.addEventListener("progress", () => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyProgress", getBufferedEnd(media));
    }, { signal });

    media.addEventListener("playing", () => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyPlaying");
        scheduleControlsHide(instance, dotNetAdapter);
    }, { signal });
    media.addEventListener("play", () => {
        if (instance.options.autoPause)
            pauseOtherPlayers(instance);

        invokeDotNetMethodAsync(dotNetAdapter, "NotifyPlay");
    }, { signal });
    media.addEventListener("pause", () => {
        setControlsVisible(instance, true, dotNetAdapter);
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyPause");
    }, { signal });
    media.addEventListener("timeupdate", () => invokeDotNetMethodAsync(dotNetAdapter, "NotifyTimeUpdate", media.currentTime || 0), { signal });
    media.addEventListener("volumechange", () => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyVolumeChange", media.volume || 0, media.muted || false);
    }, { signal });
    media.addEventListener("seeking", () => invokeDotNetMethodAsync(dotNetAdapter, "NotifySeeking", media.currentTime || 0), { signal });
    media.addEventListener("seeked", () => invokeDotNetMethodAsync(dotNetAdapter, "NotifySeeked", media.currentTime || 0), { signal });
    media.addEventListener("ratechange", () => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyRateChange", media.playbackRate || 1);
    }, { signal });
    media.addEventListener("loadedmetadata", () => {
        connectTextTrackEvents(dotNetAdapter, instance);
        applyDefaultQualityWhenAvailable(dotNetAdapter, instance);
        applyProtectionWhenAvailable(instance);
        syncCompatibilityControls(instance);
    }, { signal });
    media.addEventListener("canplay", () => invokeDotNetMethodAsync(dotNetAdapter, "NotifyReady"), { signal });
    media.addEventListener("ended", () => {
        invokeDotNetMethodAsync(dotNetAdapter, "NotifyEnded");
        setControlsVisible(instance, true, dotNetAdapter);

        if (instance.options.resetOnEnd)
            setCurrentTime(media, 0);
    }, { signal });
}

function registerPlayerEvents(dotNetAdapter, instance) {
    const signal = instance.abortController.signal;

    instance.player.addEventListener("contextmenu", event => {
        if (instance.options.disableContextMenu)
            event.preventDefault();
    }, { capture: true, signal });

    const fullscreenChanged = () => {
        const entered = isPlayerFullscreen(instance);

        if (entered === instance.fullscreen)
            return;

        instance.fullscreen = entered;
        invokeDotNetMethodAsync(dotNetAdapter, entered ? "NotifyFullScreenEntered" : "NotifyFullScreenExited");
    };

    document.addEventListener("fullscreenchange", fullscreenChanged, { signal });
    document.addEventListener("webkitfullscreenchange", fullscreenChanged, { signal });
}

function connectTextTrackEvents(dotNetAdapter, instance) {
    const tracks = instance.media?.textTracks;

    if (!tracks || tracks === instance.textTracks)
        return;

    instance.textTracks = tracks;

    tracks.addEventListener("change", () => {
        if (!dotNetAdapter)
            return;

        const enabled = Array.from(tracks).some(track => isCaptionTrack(track) && track.mode === "showing");
        const showingTrack = Array.from(tracks).find(track => isCaptionTrack(track) && track.mode === "showing");
        const language = showingTrack?.language || showingTrack?.label || "";

        invokeDotNetMethodAsync(dotNetAdapter, enabled ? "NotifyCaptionsEnabled" : "NotifyCaptionsDisabled");

        if (language !== instance.activeLanguage) {
            instance.activeLanguage = language;
            invokeDotNetMethodAsync(dotNetAdapter, "NotifyLanguageChange", language);
        }
    }, { signal: instance.mediaAbortController.signal });
}

function setupControls(dotNetAdapter, instance) {
    refreshControls(instance, dotNetAdapter);

    const container = getFullscreenTarget(instance);

    if (!container)
        return;

    const showControls = () => {
        setControlsVisible(instance, true, dotNetAdapter);
        scheduleControlsHide(instance, dotNetAdapter);
    };

    for (const eventName of ["pointermove", "pointerdown", "touchstart", "keydown", "focusin"])
        container.addEventListener(eventName, showControls, { signal: instance.abortController.signal, passive: eventName === "touchstart" });
}

function refreshControls(instance, dotNetAdapter) {
    instance.controls = getControlsElement(instance);
    syncCompatibilityControls(instance);
    configureCaptionTrackLabels(instance);

    if (!instance.controls)
        return;

    instance.controls.hidden = !instance.options.controls;

    if (!instance.options.controls) {
        setControlsVisible(instance, false, dotNetAdapter);
        return;
    }

    setControlsVisible(instance, true, dotNetAdapter);
    scheduleControlsHide(instance, dotNetAdapter);
}

function configureCaptionTrackLabels(instance) {
    const captionsRadioGroup = getSkinElement(instance, "media-captions-radio-group");

    if (!captionsRadioGroup || configuredCaptionsRadioGroups.has(captionsRadioGroup))
        return;

    const defaultFormatTrack = captionsRadioGroup.formatTrack;

    captionsRadioGroup.formatTrack = track => formatCaptionTrackLabel(track, defaultFormatTrack);
    configuredCaptionsRadioGroups.add(captionsRadioGroup);
    captionsRadioGroup.requestUpdate?.();
}

function formatCaptionTrackLabel(track, defaultFormatTrack) {
    const label = typeof track?.label === "string" ? track.label.trim() : "";
    const language = typeof track?.language === "string" ? track.language.trim() : "";

    if (label && !/^\d+$/.test(label))
        return label;

    if (language)
        return getLanguageDisplayName(language);

    if (label)
        return label;

    return typeof defaultFormatTrack === "function" ? defaultFormatTrack(track) : "Captions";
}

function getLanguageDisplayName(language) {
    if (typeof Intl === "undefined" || typeof Intl.DisplayNames !== "function")
        return language;

    const locale = document.documentElement.lang || navigator.language || "en";

    try {
        return new Intl.DisplayNames([locale], { type: "language" }).of(language) || language;
    } catch {
        return language;
    }
}

function setControlsVisible(instance, visible, dotNetAdapter) {
    const controls = getControlsElement(instance);
    const nextVisible = Boolean(visible && instance.options.controls && controls);

    if (controls) {
        instance.controls = controls;
        controls.hidden = !instance.options.controls;
        controls.toggleAttribute("data-visible", nextVisible);
        controls.toggleAttribute("data-blazorise-visible", nextVisible);
    }

    if (nextVisible === instance.controlsVisible)
        return;

    instance.controlsVisible = nextVisible;
    invokeDotNetMethodAsync(dotNetAdapter, nextVisible ? "NotifyControlsShown" : "NotifyControlsHidden");
}

function scheduleControlsHide(instance, dotNetAdapter) {
    clearTimeout(instance.controlsTimer);
    instance.controlsTimer = null;

    if (!instance.options.controls || !instance.options.automaticallyHideControls || instance.media?.paused)
        return;

    const delay = Math.max(0, firstNonNull(instance.options.controlsDelay, 2000));
    instance.controlsTimer = setTimeout(() => setControlsVisible(instance, false, dotNetAdapter), delay);
}

function setupCompatibilityControls(dotNetAdapter, instance) {
    instance.player.addEventListener("click", event => {
        const target = event.target instanceof Element ? event.target : null;

        if (target?.closest("[data-blazorise-video-restart]")) {
            setCurrentTime(instance.media, 0);
            return;
        }

        if (target?.closest("media-fullscreen-button") && !supportsFullscreen(getFullscreenTarget(instance))) {
            event.preventDefault();

            if (instance.fullWindow)
                exitFullWindow(instance);
            else
                enterFullWindow(instance);

            return;
        }

        const loopButton = target?.closest("[data-blazorise-video-loop]");

        if (loopButton) {
            instance.media.loop = !instance.media.loop;
            syncLoopButton(instance, loopButton);
            return;
        }

        const qualityButton = target?.closest("[data-blazorise-video-source]");

        if (qualityButton)
            switchNativeQuality(dotNetAdapter, instance, qualityButton.dataset.blazoriseVideoSource, qualityButton.dataset.blazoriseVideoQuality);
    }, { signal: instance.abortController.signal });

    document.addEventListener("keydown", event => {
        if (event.key === "Escape" && instance.fullWindow)
            exitFullWindow(instance);
    }, { signal: instance.abortController.signal });

    instance.player.addEventListener("dblclick", event => {
        if (instance.options.doubleClickToFullscreen && !supportsFullscreen(getFullscreenTarget(instance))) {
            event.preventDefault();

            if (instance.fullWindow)
                exitFullWindow(instance);
            else
                enterFullWindow(instance);
        }
    }, { signal: instance.abortController.signal });
}

function getControlsElement(instance) {
    return getSkinElement(instance, "media-controls, .media-controls");
}

function applyDefaultQualityWhenAvailable(dotNetAdapter, instance) {
    const requestedHeight = instance.options.defaultQuality?.height;
    const renditions = instance.media?.videoRenditions;

    if (!renditions) {
        if (requestedHeight)
            applyDefaultNativeQuality(dotNetAdapter, instance, requestedHeight);

        return;
    }

    const selectRequestedQuality = () => {
        filterQualityChoices(instance);

        const currentRequestedHeight = instance.options.defaultQuality?.height;

        if (!currentRequestedHeight)
            return;

        const renditionArray = Array.from(renditions);
        const index = renditionArray.findIndex(rendition => rendition.height === currentRequestedHeight);

        if (index >= 0)
            renditions.selectedIndex = index;

        notifyQualityChange();
    };

    const notifyQualityChange = () => {
        const renditionArray = Array.from(renditions);

        if (dotNetAdapter && renditions.selectedIndex >= 0) {
            const selected = renditionArray[renditions.selectedIndex];
            invokeDotNetMethodAsync(dotNetAdapter, "NotifyQualityChange", selected?.height ?? null);
        }
    };

    selectRequestedQuality();

    if (renditions !== instance.qualityRenditions) {
        instance.qualityRenditions = renditions;
        renditions.addEventListener("addrendition", selectRequestedQuality, { signal: instance.mediaAbortController.signal });
        renditions.addEventListener("change", notifyQualityChange, { signal: instance.mediaAbortController.signal });

        instance.qualityObserver?.disconnect();
        instance.qualityObserver = new MutationObserver(() => filterQualityChoices(instance));
        instance.qualityObserver.observe(instance.player, { childList: true, subtree: true });
    }
}

function applyDefaultNativeQuality(dotNetAdapter, instance, requestedHeight) {
    if (instance.media?.localName !== "video")
        return;

    const source = instance.options.source?.sources?.find(mediaSource => mediaSource.height === requestedHeight && isAllowedQuality(instance, mediaSource.height));

    if (!source || instance.media.src === new URL(source.src, document.baseURI).href)
        return;

    switchNativeQuality(dotNetAdapter, instance, source.src, source.height);
}

function switchNativeQuality(dotNetAdapter, instance, source, height) {
    const media = instance.media;
    const numericHeight = height === "" || height == null ? null : Number(height);

    if (!media || !source || !isAllowedQuality(instance, numericHeight))
        return;

    const sourceUrl = new URL(source, document.baseURI).href;

    if (media.currentSrc === sourceUrl || media.src === sourceUrl)
        return;

    const currentTime = media.currentTime;
    const wasPlaying = !media.paused;
    const playbackRate = media.playbackRate;

    media.src = source;
    media.load();
    media.addEventListener("loadedmetadata", () => {
        setCurrentTime(media, currentTime);
        media.playbackRate = playbackRate;

        if (wasPlaying)
            media.play().catch(reason => console.error(reason));
    }, { once: true, signal: instance.mediaAbortController.signal });

    for (const button of instance.player.querySelectorAll("[data-blazorise-video-quality]"))
        button.toggleAttribute("data-active", button.dataset.blazoriseVideoQuality === `${height}`);

    invokeDotNetMethodAsync(dotNetAdapter, "NotifyQualityChange", Number.isFinite(numericHeight) ? numericHeight : null);
}

function filterQualityChoices(instance) {
    const available = getAvailableQualityHeights(instance);

    if (!available)
        return;

    for (const item of instance.player.querySelectorAll("media-quality-radio-group media-menu-radio-item")) {
        const match = item.textContent?.match(/(\d+)p/i);
        item.hidden = Boolean(match && !available.has(Number(match[1])));
    }
}

function isAllowedQuality(instance, height) {
    const available = getAvailableQualityHeights(instance);
    return !available || !height || available.has(Number(height));
}

function getAvailableQualityHeights(instance) {
    if (!Array.isArray(instance.options.availableQualities) || instance.options.availableQualities.length === 0)
        return null;

    return new Set(instance.options.availableQualities.map(quality => Number(quality?.height)).filter(Number.isFinite));
}

async function applyProtectionWhenAvailable(instance) {
    const generation = ++instance.protectionGeneration;

    for (let attempt = 0; attempt < 120; attempt++) {
        if (instance.destroyed || generation !== instance.protectionGeneration)
            return;

        if (applyProtection(instance.media, instance.protection))
            return;

        await nextAnimationFrame();
    }
}

function applyProtection(media, protection) {
    if (!media || !protection)
        return true;

    if (media.localName === "dash-video") {
        if (!media.engine)
            return false;

        applyDashProtectionData(media.engine, protection);
        return true;
    }

    if (media.localName === "hlsjs-video") {
        if (!media.engine)
            return false;

        applyHlsProtectionData(media.engine, protection);
        return true;
    }

    return true;
}

function syncCompatibilityControls(instance) {
    const loopButton = instance.player.querySelector("[data-blazorise-video-loop]");

    if (loopButton)
        syncLoopButton(instance, loopButton);

    const currentSource = instance.media?.currentSrc || instance.media?.src;

    for (const button of instance.player.querySelectorAll("[data-blazorise-video-source]")) {
        const source = button.dataset.blazoriseVideoSource;
        const active = Boolean(source && currentSource === new URL(source, document.baseURI).href);
        button.toggleAttribute("data-active", active);
    }
}

function syncLoopButton(instance, loopButton) {
    const loop = instance.media?.loop === true;

    loopButton.setAttribute("aria-pressed", `${loop}`);
    loopButton.querySelector(".b-video-loop-value--off")?.toggleAttribute("hidden", loop);
    loopButton.querySelector(".b-video-loop-value--on")?.toggleAttribute("hidden", !loop);
}

function clearProtection(media) {
    if (media?.localName === "dash-video" && media.engine?.setProtectionData)
        media.engine.setProtectionData(null);

    if (media?.localName === "hlsjs-video" && media.engine?.config) {
        media.engine.config.emeEnabled = false;
        media.engine.config.drmSystems = {};
    }
}

function updateNativeSources(media, source) {
    media.removeAttribute("src");
    Array.from(media.querySelectorAll("source")).forEach(sourceElement => sourceElement.remove());

    const firstTrack = media.querySelector("track");

    for (const mediaSource of source?.sources || []) {
        const sourceElement = document.createElement("source");
        sourceElement.src = mediaSource.src;

        if (mediaSource.type)
            sourceElement.type = mediaSource.type;

        if (mediaSource.width != null)
            sourceElement.dataset.width = mediaSource.width;

        if (mediaSource.height != null)
            sourceElement.dataset.height = mediaSource.height;

        media.insertBefore(sourceElement, firstTrack);
    }

    media.load();
}

function updateTextTrackElements(media, tracks, thumbnails) {
    Array.from(media.querySelectorAll("track")).forEach(track => track.remove());

    for (const track of tracks || [])
        media.appendChild(createTextTrackElement(track));

    if (thumbnails) {
        const thumbnailTrack = document.createElement("track");
        thumbnailTrack.src = thumbnails;
        thumbnailTrack.kind = "metadata";
        thumbnailTrack.label = "thumbnails";
        thumbnailTrack.default = true;
        media.appendChild(thumbnailTrack);
    }
}

function createTextTrackElement(track) {
    const element = document.createElement("track");

    if (track.src)
        element.src = track.src;
    if (track.kind)
        element.kind = track.kind;
    if (track.label)
        element.label = track.label;
    if (track.language)
        element.srclang = track.language;
    if (track.default)
        element.default = true;

    return element;
}

function setTextTrackMode(media, textTrackId, mode) {
    const track = media?.textTracks?.[textTrackId];

    if (track)
        track.mode = mode;
}

function extractSingleSourceUrl(source) {
    if (!source)
        return null;

    if (isString(source))
        return source;

    return source.sources?.[0]?.src || null;
}

function normalizeProtectionType(protectionType) {
    if (typeof protectionType === "string")
        return protectionType;

    return ["None", "PlayReady", "Widevine", "FairPlay"][protectionType] || null;
}

function applyDashProtectionData(dash, protection) {
    if (!dash || !protection)
        return;

    if (protection.data) {
        dash.setProtectionData(protection.data);
        return;
    }

    const system = protection.type === "PlayReady"
        ? "com.microsoft.playready"
        : protection.type === "Widevine"
            ? "com.widevine.alpha"
            : null;

    if (!system)
        return;

    dash.setProtectionData({
        [system]: {
            serverURL: protection.serverUrl,
            httpRequestHeaders: protection.httpRequestHeaders ? {
                "X-AxDRM-Message": protection.httpRequestHeaders,
            } : null,
        },
    });
}

function applyHlsProtectionData(hls, protection) {
    if (!hls || !protection)
        return;

    if (protection.data) {
        if (protection.data.drmSystems || protection.data.emeEnabled != null)
            Object.assign(hls.config, protection.data);
        else {
            hls.config.emeEnabled = true;
            hls.config.drmSystems = protection.data;
        }

        return;
    }

    const system = protection.type === "FairPlay"
        ? "com.apple.fps"
        : protection.type === "PlayReady"
            ? "com.microsoft.playready"
            : protection.type === "Widevine"
                ? "com.widevine.alpha"
                : null;

    if (!system)
        return;

    hls.config.emeEnabled = true;
    hls.config.drmSystems = {
        [system]: {
            licenseUrl: protection.serverUrl,
            serverCertificateUrl: protection.serverCertificateUrl,
            httpRequestHeaders: protection.httpRequestHeaders ? {
                "X-AxDRM-Message": protection.httpRequestHeaders,
            } : null,
        },
    };
}

function getFullscreenTarget(instance) {
    return instance?.player?.querySelector("media-container") || instance?.media || null;
}

function getSkinElement(instance, selector) {
    return instance?.player?.querySelector(selector) || null;
}

function isPlayerFullscreen(instance) {
    if (instance?.fullWindow)
        return true;

    const fullscreenElement = getFullscreenElement();
    const container = getFullscreenTarget(instance);

    if (!fullscreenElement || !container)
        return false;

    return fullscreenElement === container || container.contains(fullscreenElement);
}

function getFullscreenElement() {
    return document.fullscreenElement || document.webkitFullscreenElement || null;
}

function requestFullscreen(element) {
    if (element.requestFullscreen)
        return element.requestFullscreen();
    if (element.webkitRequestFullscreen)
        return element.webkitRequestFullscreen();

    return Promise.resolve();
}

function supportsFullscreen(element) {
    return Boolean(element?.requestFullscreen || element?.webkitRequestFullscreen);
}

function enterFullWindow(instance) {
    const target = getFullscreenTarget(instance);

    if (!target || instance.fullWindow)
        return;

    instance.fullWindow = true;
    instance.fullscreen = true;
    instance.documentOverflow = document.documentElement.style.overflow;
    target.classList.add("b-video-full-window");
    document.documentElement.style.overflow = "hidden";
    invokeDotNetMethodAsync(instance.dotNetAdapter, "NotifyFullScreenEntered");
}

function exitFullWindow(instance, notify = true) {
    if (!instance?.fullWindow)
        return;

    getFullscreenTarget(instance)?.classList.remove("b-video-full-window");
    document.documentElement.style.overflow = instance.documentOverflow || "";
    instance.fullWindow = false;
    instance.fullscreen = false;

    if (notify) {
        invokeDotNetMethodAsync(instance.dotNetAdapter, "NotifyFullScreenExited");
    }
}

function exitDocumentFullscreen() {
    if (document.exitFullscreen)
        return document.exitFullscreen();
    if (document.webkitExitFullscreen)
        return document.webkitExitFullscreen();

    return Promise.resolve();
}

function setCurrentTimeWhenReady(media, currentTime, signal) {
    if (media.readyState > 0) {
        setCurrentTime(media, currentTime);
        return;
    }

    media.addEventListener("loadedmetadata", () => setCurrentTime(media, currentTime), { once: true, signal });
}

function setCurrentTime(media, currentTime) {
    try {
        media.currentTime = firstNonNull(currentTime, 0);
    } catch (error) {
        console.error(error);
    }
}

function getBufferedEnd(media) {
    if (!media.buffered?.length)
        return 0;

    return media.buffered.end(media.buffered.length - 1);
}

function isCaptionTrack(track) {
    return track.kind === "captions" || track.kind === "subtitles";
}

function pauseOtherPlayers(activeInstance) {
    for (const instance of instances.values()) {
        if (instance && instance !== activeInstance && !instance.media?.paused)
            instance.media.pause();
    }
}

function applyChangedOption(instance, changes, optionName) {
    const change = changes[optionName];

    if (change?.changed)
        instance.options[optionName] = change.value;
}

function clamp(value, minimum, maximum) {
    return Math.min(Math.max(firstNonNull(value, minimum), minimum), maximum);
}

function nextAnimationFrame() {
    return new Promise(resolve => requestAnimationFrame(resolve));
}

function invokeDotNetMethodAsync(dotNetAdapter, methodName, ...args) {
    if (!dotNetAdapter)
        return;

    dotNetAdapter.invokeMethodAsync(methodName, ...args)
        .catch(reason => console.error(reason));
}