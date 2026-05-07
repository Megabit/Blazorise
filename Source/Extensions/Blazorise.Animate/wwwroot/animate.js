"use strict";

var global = window;

var instances = new WeakMap();
var motionPromise = null;
var motionScriptId = "blazorise-motion-script";
var motionScriptUrl = "_content/Blazorise.Animate/motion.js";
var defaultOptions = {
    animation: "fade",
    easing: "ease",
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
var easingMap = {
    "Ease": [0.25, 0.1, 0.25, 1],
    "linear": "linear",
    "ease": [0.25, 0.1, 0.25, 1],
    "ease-in": "easeIn",
    "ease-out": "easeOut",
    "ease-in-out": "easeInOut",
    "ease-in-back": [0.6, -0.28, 0.735, 0.045],
    "ease-out-back": [0.175, 0.885, 0.32, 1.275],
    "ease-in-out-back": [0.68, -0.55, 0.265, 1.55],
    "ease-in-sine": [0.47, 0, 0.745, 0.715],
    "ease-out-sine": [0.39, 0.575, 0.565, 1],
    "ease-in-out-sine": [0.445, 0.05, 0.55, 0.95],
    "ease-in-quad": [0.55, 0.085, 0.68, 0.53],
    "ease-out-quad": [0.25, 0.46, 0.45, 0.94],
    "ease-in-out-quad": [0.455, 0.03, 0.515, 0.955],
    "ease-in-cubic": [0.55, 0.085, 0.68, 0.53],
    "ease-out-cubic": [0.25, 0.46, 0.45, 0.94],
    "ease-in-out-cubic": [0.455, 0.03, 0.515, 0.955],
    "ease-in-quart": [0.55, 0.085, 0.68, 0.53],
    "ease-out-quart": [0.25, 0.46, 0.45, 0.94],
    "ease-in-out-quart": [0.455, 0.03, 0.515, 0.955]
};

function loadMotion() {
    if (global.Motion) {
        return Promise.resolve(global.Motion);
    }

    if (motionPromise) {
        return motionPromise;
    }

    motionPromise = new Promise(function (resolve, reject) {
        var existingScript = document.getElementById(motionScriptId);

        if (existingScript) {
            existingScript.addEventListener("load", function () { resolve(global.Motion); });
            existingScript.addEventListener("error", reject);
            return;
        }

        var script = document.createElement("script");
        script.id = motionScriptId;
        script.src = motionScriptUrl;
        script.async = true;
        script.onload = function () { resolve(global.Motion); };
        script.onerror = reject;

        document.head.appendChild(script);
    });

    return motionPromise;
}

function normalizeOptions(options) {
    options = options || {};

    return {
        animation: options.animation || defaultOptions.animation,
        easing: options.easing || defaultOptions.easing,
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

    var number = Number(value);

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
    return easingMap[easing] || easingMap[String(easing).toLowerCase()] || easingMap.ease;
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

    var transform = frame.transform !== undefined ? frame.transform : createTransform(frame);

    if (transform !== null) {
        element.style.transform = transform;
    }

    element.style.visibility = "visible";
}

function createTransform(frame) {
    var transforms = [];
    var x = frame.x || "0";
    var y = frame.y || "0";

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

    return transforms.length > 0 ? transforms.join(" ") : null;
}

function createKeyframes(animation, original, reversed) {
    var frames = getAnimationFrames(animation);
    var from = cloneFrame(frames[0]);
    var to = cloneFrame(frames[1]);

    if (to.opacity === undefined) {
        to.opacity = original.opacity || "1";
    }

    if (reversed) {
        return [to, from];
    }

    return [from, to];
}

function cloneFrame(frame) {
    var clone = {};

    Object.keys(frame).forEach(function (key) {
        clone[key] = frame[key];
    });

    return clone;
}

function toMotionKeyframes(keyframes) {
    var target = {};

    keyframes.forEach(function (frame) {
        Object.keys(frame).forEach(function (key) {
            if (!target[key]) {
                target[key] = [];
            }
        });
    });

    Object.keys(target).forEach(function (key) {
        target[key] = keyframes.map(function (frame) {
            return frame[key];
        }).filter(function (value) {
            return value !== undefined;
        });
    });

    return target;
}

function getAnimationFrames(animation) {
    switch (animation) {
        case "fade-up":
            return [{ opacity: 0, y: "100px" }, { opacity: 1, y: "0px" }];
        case "fade-down":
            return [{ opacity: 0, y: "-100px" }, { opacity: 1, y: "0px" }];
        case "fade-left":
            return [{ opacity: 0, x: "100px" }, { opacity: 1, x: "0px" }];
        case "fade-right":
            return [{ opacity: 0, x: "-100px" }, { opacity: 1, x: "0px" }];
        case "fade-up-right":
            return [{ opacity: 0, x: "-100px", y: "100px" }, { opacity: 1, x: "0px", y: "0px" }];
        case "fade-up-left":
            return [{ opacity: 0, x: "100px", y: "100px" }, { opacity: 1, x: "0px", y: "0px" }];
        case "fade-down-right":
            return [{ opacity: 0, x: "-100px", y: "-100px" }, { opacity: 1, x: "0px", y: "0px" }];
        case "fade-down-left":
            return [{ opacity: 0, x: "100px", y: "-100px" }, { opacity: 1, x: "0px", y: "0px" }];
        case "flip-up":
            return [{ opacity: 0, transformPerspective: "2500px", rotateX: "-100deg" }, { opacity: 1, transformPerspective: "2500px", rotateX: "0deg" }];
        case "flip-down":
            return [{ opacity: 0, transformPerspective: "2500px", rotateX: "100deg" }, { opacity: 1, transformPerspective: "2500px", rotateX: "0deg" }];
        case "flip-left":
            return [{ opacity: 0, transformPerspective: "2500px", rotateY: "-100deg" }, { opacity: 1, transformPerspective: "2500px", rotateY: "0deg" }];
        case "flip-right":
            return [{ opacity: 0, transformPerspective: "2500px", rotateY: "100deg" }, { opacity: 1, transformPerspective: "2500px", rotateY: "0deg" }];
        case "slide-up":
            return [{ y: "100%" }, { y: "0%" }];
        case "slide-down":
            return [{ y: "-100%" }, { y: "0%" }];
        case "slide-left":
            return [{ x: "100%" }, { x: "0%" }];
        case "slide-right":
            return [{ x: "-100%" }, { x: "0%" }];
        case "zoom-in":
            return [{ opacity: 0, scale: 0.6 }, { opacity: 1, scale: 1 }];
        case "zoom-in-up":
            return [{ opacity: 0, y: "100px", scale: 0.6 }, { opacity: 1, y: "0px", scale: 1 }];
        case "zoom-in-down":
            return [{ opacity: 0, y: "-100px", scale: 0.6 }, { opacity: 1, y: "0px", scale: 1 }];
        case "zoom-in-left":
            return [{ opacity: 0, x: "100px", scale: 0.6 }, { opacity: 1, x: "0px", scale: 1 }];
        case "zoom-in-right":
            return [{ opacity: 0, x: "-100px", scale: 0.6 }, { opacity: 1, x: "0px", scale: 1 }];
        case "zoom-out":
            return [{ opacity: 0, scale: 1.2 }, { opacity: 1, scale: 1 }];
        case "zoom-out-up":
            return [{ opacity: 0, y: "100px", scale: 1.2 }, { opacity: 1, y: "0px", scale: 1 }];
        case "zoom-out-down":
            return [{ opacity: 0, y: "-100px", scale: 1.2 }, { opacity: 1, y: "0px", scale: 1 }];
        case "zoom-out-left":
            return [{ opacity: 0, x: "100px", scale: 1.2 }, { opacity: 1, x: "0px", scale: 1 }];
        case "zoom-out-right":
            return [{ opacity: 0, x: "-100px", scale: 1.2 }, { opacity: 1, x: "0px", scale: 1 }];
        case "fade":
        case "fade-in":
        default:
            return [{ opacity: 0 }, { opacity: 1 }];
    }
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
    var offset = Math.max(settings.offset || 0, 0);

    if (settings.anchorPlacement && settings.anchorPlacement.indexOf("-top") > -1) {
        return offset + "px 0px 0px 0px";
    }

    if (settings.anchorPlacement && settings.anchorPlacement.indexOf("-center") > -1) {
        return "0px 0px -" + Math.max(Math.round(global.innerHeight / 2) - offset, 0) + "px 0px";
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
    var isWidth = settings.animatedSize === "Width";
    var rect = element.getBoundingClientRect();
    var size = isWidth ? rect.width : rect.height;

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

    var property = settings.animatedSize === "Width" ? "width" : "height";
    var minProperty = settings.animatedSize === "Width" ? "minWidth" : "minHeight";
    var expanded = measureLayoutSize(element, settings) + "px";
    var collapsed = "0px";
    var target = {};

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
    var property = settings.animatedSize === "Width" ? "width" : "height";
    var minProperty = settings.animatedSize === "Width" ? "minWidth" : "minHeight";

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
    var instance = instances.get(element);

    if (!instance) {
        return;
    }

    if (instance.stop) {
        instance.stop();
    }

    instance.cancelled = true;

    stopAnimation(instance.animation);
    instances.delete(element);
}

function runAnimation(motion, element, settings, original, reversed, completed, isCancelled) {
    var visualElement = getVisualElement(element, settings);
    var visualOriginal = visualElement === element ? original : readOriginalStyles(visualElement);
    var keyframes = createKeyframes(settings.animation, visualOriginal, reversed);
    var target = toMotionKeyframes(keyframes);
    var layoutTarget = createLayoutTarget(element, settings, reversed);
    var animations = [];
    var pendingAnimations = 0;
    var resolved = false;
    var resolveFinished;
    var finished = new Promise(function (resolve) {
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

    var finish = function () {
        if (resolved) {
            return;
        }

        resolved = true;

        if (isCancelled && isCancelled()) {
            resolveFinished(false);
            return;
        }

        if (reversed) {
            applyFrame(visualElement, keyframes[1]);
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

    var completeAnimation = function () {
        pendingAnimations--;

        if (pendingAnimations <= 0) {
            finish();
        }
    };

    var startAnimation = function (targetElement, animationTarget) {
        var keys = Object.keys(animationTarget);

        if (keys.length === 0) {
            return;
        }

        pendingAnimations++;

        var animationCompleted = false;
        var markAnimationComplete = function () {
            if (animationCompleted) {
                return;
            }

            animationCompleted = true;
            completeAnimation();
        };
        var animation = motion.animate(targetElement, animationTarget, {
            duration: settings.duration / 1000,
            delay: settings.delay / 1000,
            ease: resolveEasing(settings.easing),
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

    global.setTimeout(finish, settings.delay + settings.duration + 100);

    return {
        animation: animations,
        finished: finished
    };
}

function setupInView(motion, element, settings, original, instance) {
    var target = getAnchorElement(settings.anchor) || element;

    applyFrame(element, createKeyframes(settings.animation, original, false)[0]);

    instance.stop = motion.inView(target, function () {
        if (!instance.animated) {
            stopAnimation(instance.animation);

            var enterResult = runAnimation(motion, element, settings, original, false, function () {
                instance.animated = true;
            }, function () {
                return instance.cancelled;
            });
            instance.animation = enterResult.animation;
        }

        if (settings.once) {
            return;
        }

        return function (leaveInfo) {
            if (settings.mirror && leaveInfo.boundingClientRect.top < 0) {
                stopAnimation(instance.animation);

                var exitResult = runAnimation(motion, element, settings, original, true, function () {
                    instance.animated = false;
                }, function () {
                    return instance.cancelled;
                });
                instance.animation = exitResult.animation;
            } else if (leaveInfo.boundingClientRect.top > 0) {
                stopAnimation(instance.animation);
                applyFrame(element, createKeyframes(settings.animation, original, false)[0]);
                instance.animated = false;
            }
        };
    }, {
        margin: createViewportMargin(settings),
        amount: "some"
    });
}

export function init() {
    return loadMotion().then(function () {
        return true;
    });
}

export function refresh() {
    return true;
}

export function animate(element, options) {
    if (!element) {
        return true;
    }

    cleanup(element);

    var settings = normalizeOptions(options);
    var original = readOriginalStyles(element);
    var instance = {
        animation: null,
        stop: null,
        animated: false,
        cancelled: false
    };

    instances.set(element, instance);

    return loadMotion().then(function (motion) {
        if (!motion || !motion.animate || !motion.inView) {
            return true;
        }

        if (settings.trigger === "Render" || settings.direction === "out") {
            var result = runAnimation(motion, element, settings, original, settings.direction === "out", function () {
                instance.animated = settings.direction !== "out";
            }, function () {
                return instance.cancelled;
            });
            instance.animation = result.animation;

            return settings.waitForCompletion ? result.finished : true;
        }

        setupInView(motion, element, settings, original, instance);

        return true;
    });
}

export function dispose(element) {
    cleanup(element);
}