const zoomWheelHandlers = new WeakMap();
const animationStates = new WeakMap();

export function initializeZoomWheel(element) {
    if (!element || zoomWheelHandlers.has(element)) {
        return;
    }

    const handler = event => {
        if (event.cancelable) {
            event.preventDefault();
        }
    };

    element.addEventListener("wheel", handler, { passive: false });
    zoomWheelHandlers.set(element, handler);
}

export function destroyZoomWheel(element) {
    const handler = zoomWheelHandlers.get(element);

    if (!element || !handler) {
        return;
    }

    element.removeEventListener("wheel", handler, { passive: false });
    zoomWheelHandlers.delete(element);
}

export function runAnimations(element) {
    if (!element) {
        return;
    }

    const selector = animationAttributes()
        .map(attribute => `[data-svg-chart-animation-${attribute}='true']`)
        .join(",");
    const items = element.querySelectorAll(selector);

    for (const item of items) {
        runElementAnimation(item);
    }
}

function runElementAnimation(element) {
    const attributes = animationAttributes();
    let started = false;

    for (const attribute of attributes) {
        const name = toDatasetName(attribute);

        if (element.dataset[`svgChartAnimation${name}`] !== "true") {
            continue;
        }

        const duration = parseDuration(element.dataset[`svgChartAnimation${name}Duration`]);
        const delay = parseDuration(element.dataset[`svgChartAnimation${name}Delay`]);
        const version = element.dataset[`svgChartAnimation${name}Version`] || "0";
        const keySplines = parseKeySplines(element.dataset[`svgChartAnimation${name}KeySplines`]);
        const from = parseFloat(element.dataset[`svgChartAnimation${name}From`]);
        const to = parseFloat(element.dataset[`svgChartAnimation${name}To`]);

        if (duration <= 0 || !Number.isFinite(from) || !Number.isFinite(to) || from === to) {
            continue;
        }

        const key = `${version}:${attribute}:${from}:${to}`;
        const current = animationStates.get(element);

        if (current?.[attribute] === key) {
            continue;
        }

        animationStates.set(element, { ...current, [attribute]: key });
        animateAttribute(element, attribute, from, to, duration, delay, keySplines);
        started = true;
    }

    if (!started && element.dataset.svgChartAnimationInitial === "true") {
        revealInitialAnimatedElement(element);
    }
}

function animateAttribute(element, attribute, from, to, duration, delay, keySplines) {
    let start = null;

    setAnimatedAttribute(element, attribute, from);
    revealInitialAnimatedElement(element);

    const step = timestamp => {
        if (animationStates.get(element)?.[attribute] !== `${attributeVersionKey(element, attribute)}:${attribute}:${from}:${to}`) {
            return;
        }

        start ??= timestamp + delay;

        if (timestamp < start) {
            requestAnimationFrame(step);
            return;
        }

        const progress = Math.min((timestamp - start) / duration, 1);
        const eased = keySplines ? cubicBezier(progress, keySplines[0], keySplines[1], keySplines[2], keySplines[3]) : progress;
        const value = from + (to - from) * eased;

        setAnimatedAttribute(element, attribute, value);

        if (progress < 1) {
            requestAnimationFrame(step);
        } else {
            setAnimatedAttribute(element, attribute, to);
        }
    };

    requestAnimationFrame(step);
}

function animationAttributes() {
    return ["x", "y", "width", "height", "cx", "cy", "r", "opacity"];
}

function attributeVersionKey(element, attribute) {
    return element.dataset[`svgChartAnimation${toDatasetName(attribute)}Version`] || "0";
}

function setAnimatedAttribute(element, attribute, value) {
    element.setAttribute(attribute, formatNumber(value));
}

function revealInitialAnimatedElement(element) {
    if (element.dataset.svgChartAnimationInitial !== "true") {
        return;
    }

    element.style.visibility = "visible";
    element.removeAttribute("data-svg-chart-animation-initial");
}

function parseDuration(value) {
    if (!value) {
        return 0;
    }

    if (value.endsWith("ms")) {
        return parseFloat(value);
    }

    if (value.endsWith("s")) {
        return parseFloat(value) * 1000;
    }

    return parseFloat(value) || 0;
}

function parseKeySplines(value) {
    if (!value) {
        return null;
    }

    const parts = value.split(/\s+/).map(Number);

    return parts.length === 4 && parts.every(Number.isFinite) ? parts : null;
}

function toDatasetName(attribute) {
    return attribute.charAt(0).toUpperCase() + attribute.slice(1);
}

function cubicBezier(progress, x1, y1, x2, y2) {
    let lower = 0;
    let upper = 1;
    let t = progress;

    for (let i = 0; i < 8; i++) {
        const x = bezier(t, x1, x2);

        if (Math.abs(x - progress) < 0.001) {
            break;
        }

        if (x > progress) {
            upper = t;
        } else {
            lower = t;
        }

        t = (lower + upper) / 2;
    }

    return bezier(t, y1, y2);
}

function bezier(t, a, b) {
    const inv = 1 - t;

    return 3 * inv * inv * t * a + 3 * inv * t * t * b + t * t * t;
}

function formatNumber(value) {
    return Number.isFinite(value) ? value.toFixed(3).replace(/\.?0+$/, "") : value;
}