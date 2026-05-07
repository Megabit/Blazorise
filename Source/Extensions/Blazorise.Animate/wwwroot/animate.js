import { animate as motionAnimate, inView as motionInView } from "./vendors/motion.js?v=2.1.2.0";

const _instances = new WeakMap();
const defaultOptions = {
    animation: "fade",
    keyframes: null,
    easing: "ease",
    easingValue: [0.25, 0.1, 0.25, 1],
    duration: 400,
    delay: 0,
    mirror: false,
    once: false,
    offset: 120,
    anchor: null,
    anchorPlacement: "top-bottom",
    trigger: "InView",
    direction: "in",
    waitForCompletion: false,
    animatedSize: "None"
};

function normalizeOptions(options) {
    options = options || {};

    return {
        animation: options.animation || defaultOptions.animation,
        keyframes: normalizeKeyframes(options.keyframes),
        easing: options.easing || defaultOptions.easing,
        easingValue: normalizeEasing(options.easingValue),
        duration: toNumber(options.duration, defaultOptions.duration),
        delay: toNumber(options.delay, defaultOptions.delay),
        mirror: toBoolean(options.mirror, defaultOptions.mirror),
        once: toBoolean(options.once, defaultOptions.once),
        offset: toNumber(options.offset, defaultOptions.offset),
        anchor: options.anchor || defaultOptions.anchor,
        anchorPlacement: options.anchorPlacement || defaultOptions.anchorPlacement,
        trigger: options.trigger || defaultOptions.trigger,
        direction: options.direction || defaultOptions.direction,
        waitForCompletion: toBoolean(options.waitForCompletion, defaultOptions.waitForCompletion),
        animatedSize: options.animatedSize || options.layout || defaultOptions.animatedSize
    };
}

function toNumber(value, fallback) {
    if (value === null || value === undefined || value === "") {
        return fallback;
    }

    const number = Number(value);

    return isNaN(number) ? fallback : number;
}

function toBoolean(value, fallback) {
    if (value === true || value === false) {
        return value;
    }

    if (value === "true") {
        return true;
    }

    if (value === "false") {
        return false;
    }

    return fallback;
}

function resolveEasing(easing) {
    return normalizeEasing(easing);
}

function normalizeEasing(easing) {
    if (typeof easing === "string" && easing.length > 0) {
        return easing;
    }

    if (Array.isArray(easing) && easing.length === 4 && easing.every(isFiniteNumber)) {
        return easing;
    }

    return defaultOptions.easingValue;
}

function isFiniteNumber(value) {
    return typeof value === "number" && isFinite(value);
}

function normalizeKeyframes(keyframes) {
    if (!Array.isArray(keyframes) || keyframes.length < 2) {
        return defaultOptions.keyframes;
    }

    const normalized = keyframes
        .map(normalizeFrame)
        .filter((frame) => Object.keys(frame).length > 0);

    return normalized.length >= 2 ? normalized : defaultOptions.keyframes;
}

function normalizeFrame(frame) {
    const normalized = {};

    if (!frame) {
        return normalized;
    }

    Object.keys(frame).forEach((key) => {
        const value = frame[key];

        if (value !== null && value !== undefined) {
            normalized[key] = value;
        }
    });

    return normalized;
}

function readOriginalStyles(element) {
    return {
        opacity: element.style.opacity,
        transform: element.style.transform,
        transformOrigin: element.style.transformOrigin,
        visibility: element.style.visibility,
        backfaceVisibility: element.style.backfaceVisibility,
        overflow: element.style.overflow,
        width: element.style.width,
        height: element.style.height,
        minWidth: element.style.minWidth,
        minHeight: element.style.minHeight,
        flexShrink: element.style.flexShrink
    };
}

function restoreStyles(element, original) {
    element.style.opacity = original.opacity;
    element.style.transform = original.transform;
    element.style.transformOrigin = original.transformOrigin;
    element.style.visibility = original.visibility;
    element.style.backfaceVisibility = original.backfaceVisibility;
    element.style.overflow = original.overflow;
    element.style.width = original.width;
    element.style.height = original.height;
    element.style.minWidth = original.minWidth;
    element.style.minHeight = original.minHeight;
    element.style.flexShrink = original.flexShrink;
}

function applyFrame(element, frame) {
    if (frame.opacity !== undefined) {
        element.style.opacity = frame.opacity;
    }

    const transform = frame.transform !== undefined ? frame.transform : createTransform(frame);

    if (transform !== null) {
        element.style.transform = transform;
    }

    element.style.visibility = "visible";
}

function createTransform(frame) {
    const transforms = [];
    const x = frame.x || "0";
    const y = frame.y || "0";

    if (frame.transformPerspective !== undefined) {
        transforms.push("perspective(" + frame.transformPerspective + ")");
    }

    if (frame.x !== undefined || frame.y !== undefined) {
        transforms.push("translate3d(" + x + ", " + y + ", 0)");
    }

    if (frame.scale !== undefined) {
        transforms.push("scale(" + frame.scale + ")");
    }

    if (frame.rotateX !== undefined) {
        transforms.push("rotateX(" + frame.rotateX + ")");
    }

    if (frame.rotateY !== undefined) {
        transforms.push("rotateY(" + frame.rotateY + ")");
    }

    if (frame.rotate !== undefined) {
        transforms.push("rotate(" + frame.rotate + ")");
    }

    if (frame.rotateZ !== undefined) {
        transforms.push("rotateZ(" + frame.rotateZ + ")");
    }

    return transforms.length > 0 ? transforms.join(" ") : null;
}

function createKeyframes(settings, original, reversed) {
    const frames = settings.keyframes || getDefaultKeyframes();
    const keyframes = frames.map(cloneFrame);
    const to = keyframes[keyframes.length - 1];

    if (to.opacity === undefined) {
        to.opacity = original.opacity || "1";
    }

    return reversed ? keyframes.reverse() : keyframes;
}

function cloneFrame(frame) {
    const clone = {};

    Object.keys(frame).forEach((key) => {
        clone[key] = frame[key];
    });

    return clone;
}

function toMotionKeyframes(keyframes) {
    const target = {};

    keyframes.forEach((frame) => {
        Object.keys(frame).forEach((key) => {
            if (!target[key]) {
                target[key] = [];
            }
        });
    });

    Object.keys(target).forEach((key) => {
        target[key] = keyframes
            .map((frame) => frame[key])
            .filter((value) => value !== undefined);
    });

    return target;
}

function getDefaultKeyframes() {
    return [{ opacity: 0 }, { opacity: 1 }];
}

function getAnchorElement(anchor) {
    if (!anchor) {
        return null;
    }

    try {
        return document.querySelector(anchor);
    } catch (e) {
        return null;
    }
}

function createViewportMargin(settings) {
    const offset = Math.max(settings.offset || 0, 0);

    if (settings.anchorPlacement && settings.anchorPlacement.indexOf("-top") > -1) {
        return offset + "px 0px 0px 0px";
    }

    if (settings.anchorPlacement && settings.anchorPlacement.indexOf("-center") > -1) {
        return "0px 0px -" + Math.max(Math.round(window.innerHeight / 2) - offset, 0) + "px 0px";
    }

    return "0px 0px -" + offset + "px 0px";
}

function hasSizeAnimation(settings) {
    return settings.animatedSize === "Width" || settings.animatedSize === "Height";
}

function getVisualElement(element, settings) {
    if (!hasSizeAnimation(settings)) {
        return element;
    }

    return element.firstElementChild || element;
}

function measureLayoutSize(element, settings) {
    const isWidth = settings.animatedSize === "Width";
    let rect = element.getBoundingClientRect();
    let size = isWidth ? rect.width : rect.height;

    if (!size) {
        size = isWidth ? element.scrollWidth : element.scrollHeight;
    }

    if (!size && element.firstElementChild) {
        rect = element.firstElementChild.getBoundingClientRect();
        size = isWidth ? rect.width : rect.height;
    }

    return Math.max(Math.ceil(size), 0);
}

function createLayoutTarget(element, settings, reversed) {
    if (!hasSizeAnimation(settings)) {
        return null;
    }

    const property = settings.animatedSize === "Width" ? "width" : "height";
    const minProperty = settings.animatedSize === "Width" ? "minWidth" : "minHeight";
    const expanded = measureLayoutSize(element, settings) + "px";
    const collapsed = "0px";
    const target = {};

    target[property] = reversed ? [expanded, collapsed] : [collapsed, expanded];
    target[minProperty] = reversed ? [expanded, collapsed] : [collapsed, expanded];

    return target;
}

function applyLayoutFrame(element, target, index) {
    if (!target) {
        return;
    }

    index = index || 0;

    if (target.width) {
        element.style.width = target.width[index];
    }

    if (target.height) {
        element.style.height = target.height[index];
    }

    if (target.minWidth) {
        element.style.minWidth = target.minWidth[index];
    }

    if (target.minHeight) {
        element.style.minHeight = target.minHeight[index];
    }
}

function restoreExpandedLayoutStyles(element, original, settings) {
    const property = settings.animatedSize === "Width" ? "width" : "height";
    const minProperty = settings.animatedSize === "Width" ? "minWidth" : "minHeight";

    element.removeAttribute("data-blazorise-animate-collapsed");
    element.style.overflow = original.overflow;
    element.style.flexShrink = original.flexShrink;
    element.style[property] = original[property] === "0px" ? "" : original[property];
    element.style[minProperty] = original[minProperty] === "0px" ? "" : original[minProperty];
}

function restoreEnteredVisualStyles(element, original, wasCollapsed) {
    if (!wasCollapsed) {
        restoreStyles(element, original);
        return;
    }

    element.style.opacity = original.opacity;
    element.style.transform = "";
    element.style.transformOrigin = "";
    element.style.visibility = original.visibility;
    element.style.backfaceVisibility = "";
}

function stopAnimation(animation) {
    if (!animation) {
        return;
    }

    if (Array.isArray(animation)) {
        animation.forEach(stopAnimation);
        return;
    }

    if (typeof animation.stop === "function") {
        animation.stop();
    } else if (typeof animation.cancel === "function") {
        animation.cancel();
    }
}

function cleanup(element) {
    const instance = _instances.get(element);

    if (!instance) {
        return;
    }

    if (instance.stop) {
        instance.stop();
    }

    instance.cancelled = true;

    stopAnimation(instance.animation);
    _instances.delete(element);
}

function runAnimation(element, settings, original, reversed, completed, isCancelled) {
    const visualElement = getVisualElement(element, settings);
    const visualOriginal = visualElement === element ? original : readOriginalStyles(visualElement);
    const keyframes = createKeyframes(settings, visualOriginal, reversed);
    const target = toMotionKeyframes(keyframes);
    const layoutTarget = createLayoutTarget(element, settings, reversed);
    const animations = [];
    let pendingAnimations = 0;
    let resolved = false;
    let resolveFinished;
    const finished = new Promise((resolve) => {
        resolveFinished = resolve;
    });

    if (layoutTarget) {
        element.style.overflow = "hidden";
        element.style.flexShrink = "0";
        applyLayoutFrame(element, layoutTarget);
    }

    applyFrame(visualElement, keyframes[0]);
    visualElement.style.transformOrigin = visualOriginal.transformOrigin || "center center";
    visualElement.style.backfaceVisibility = visualOriginal.backfaceVisibility || "hidden";

    const finish = () => {
        if (resolved) {
            return;
        }

        resolved = true;

        if (isCancelled && isCancelled()) {
            resolveFinished(false);
            return;
        }

        if (reversed) {
            applyFrame(visualElement, keyframes[keyframes.length - 1]);
            applyLayoutFrame(element, layoutTarget, 1);

            if (layoutTarget) {
                element.setAttribute("data-blazorise-animate-collapsed", "true");
            }
        } else {
            restoreEnteredVisualStyles(visualElement, visualOriginal, element.getAttribute("data-blazorise-animate-collapsed") === "true");

            if (layoutTarget) {
                restoreExpandedLayoutStyles(element, original, settings);
            }
        }

        if (completed) {
            completed();
        }

        resolveFinished(true);
    };

    const completeAnimation = () => {
        pendingAnimations--;

        if (pendingAnimations <= 0) {
            finish();
        }
    };

    const startAnimation = (targetElement, animationTarget) => {
        const keys = Object.keys(animationTarget);

        if (keys.length === 0) {
            return;
        }

        pendingAnimations++;

        let animationCompleted = false;
        const markAnimationComplete = () => {
            if (animationCompleted) {
                return;
            }

            animationCompleted = true;
            completeAnimation();
        };
        const animation = motionAnimate(targetElement, animationTarget, {
            duration: settings.duration / 1000,
            delay: settings.delay / 1000,
            ease: resolveEasing(settings.easingValue || settings.easing),
            onComplete: markAnimationComplete
        });

        animations.push(animation);

        if (animation && animation.finished && typeof animation.finished.then === "function") {
            animation.finished.then(markAnimationComplete).catch(markAnimationComplete);
        } else if (animation && typeof animation.then === "function") {
            animation.then(markAnimationComplete).catch(markAnimationComplete);
        }
    };

    startAnimation(visualElement, target);

    if (layoutTarget) {
        startAnimation(element, layoutTarget);
    }

    window.setTimeout(finish, settings.delay + settings.duration + 100);

    return {
        animation: animations,
        finished: finished
    };
}

function setupInView(element, settings, original, instance) {
    const target = getAnchorElement(settings.anchor) || element;

    applyFrame(element, createKeyframes(settings, original, false)[0]);

    instance.stop = motionInView(target, () => {
        if (!instance.animated) {
            stopAnimation(instance.animation);

            const enterResult = runAnimation(element, settings, original, false, () => {
                instance.animated = true;
            }, () => instance.cancelled);
            instance.animation = enterResult.animation;
        }

        if (settings.once) {
            return;
        }

        return (leaveInfo) => {
            if (settings.mirror && leaveInfo.boundingClientRect.top < 0) {
                stopAnimation(instance.animation);

                const exitResult = runAnimation(element, settings, original, true, () => {
                    instance.animated = false;
                }, () => instance.cancelled);
                instance.animation = exitResult.animation;
            } else if (leaveInfo.boundingClientRect.top > 0) {
                stopAnimation(instance.animation);
                applyFrame(element, createKeyframes(settings, original, false)[0]);
                instance.animated = false;
            }
        };
    }, {
        margin: createViewportMargin(settings),
        amount: "some"
    });
}

export function init() {
    return true;
}

export function refresh() {
    return true;
}

export function animate(element, options) {
    if (!element) {
        return true;
    }

    cleanup(element);

    const settings = normalizeOptions(options);
    const original = readOriginalStyles(element);
    const instance = {
        animation: null,
        stop: null,
        animated: false,
        cancelled: false
    };

    _instances.set(element, instance);

    if (settings.trigger === "Render" || settings.direction === "out") {
        const result = runAnimation(element, settings, original, settings.direction === "out", () => {
            instance.animated = settings.direction !== "out";
        }, () => instance.cancelled);
        instance.animation = result.animation;

        return settings.waitForCompletion ? result.finished : true;
    }

    setupInView(element, settings, original, instance);

    return true;
}

export function dispose(element) {
    cleanup(element);
}