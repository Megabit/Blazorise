const waveHandlers = new WeakMap();
const waveCleanupTimers = new WeakMap();

function isValidWaveColor(color) {
    return !!color
        && color !== '#fff'
        && color !== '#ffffff'
        && color !== 'rgb(255, 255, 255)'
        && color !== 'rgba(255, 255, 255, 1)'
        && !/rgba\((?:\d*,\s*){3}0(?:\.0+)?\)/.test(color)
        && color !== 'transparent'
        && color !== 'canvastext';
}

function getTargetWaveColor(node) {
    const style = getComputedStyle(node);
    const colors = [style.borderTopColor, style.borderColor, style.backgroundColor];

    for (const color of colors) {
        if (isValidWaveColor(color)) {
            return color;
        }
    }

    return null;
}

function cleanupWave(target) {
    const currentWave = target.querySelector(':scope > .ant-wave');

    if (currentWave) {
        currentWave.remove();
    }

    const currentTimer = waveCleanupTimers.get(target);

    if (currentTimer) {
        clearTimeout(currentTimer);
        waveCleanupTimers.delete(target);
    }
}

function showWave(root, targetSelector) {
    const target = targetSelector
        ? root.querySelector(targetSelector) || root
        : root;

    if (!target) {
        return;
    }

    if (target.hasAttribute('disabled')
        || target.getAttribute('aria-disabled') === 'true'
        || target.className?.includes('disabled')) {
        return;
    }

    cleanupWave(target);

    target.classList.add('b-ant-wave-host');

    const wave = document.createElement('div');
    const waveColor = getTargetWaveColor(target);
    const targetStyle = getComputedStyle(target);
    const isSmallTarget = target.classList.contains('ant-wave-target');

    wave.className = `ant-wave${isSmallTarget ? ' ant-wave-quick' : ''}`;
    wave.style.left = '0px';
    wave.style.top = '0px';
    wave.style.width = `${target.offsetWidth}px`;
    wave.style.height = `${target.offsetHeight}px`;
    wave.style.borderRadius = [
        targetStyle.borderTopLeftRadius,
        targetStyle.borderTopRightRadius,
        targetStyle.borderBottomRightRadius,
        targetStyle.borderBottomLeftRadius,
    ].join(' ');

    if (waveColor) {
        wave.style.setProperty('--ant-wave-color', waveColor);
    }

    target.insertBefore(wave, target.firstChild);

    requestAnimationFrame(() => {
        wave.classList.add('ant-wave-motion-appear');

        requestAnimationFrame(() => {
            wave.classList.add('ant-wave-motion-appear-active');
        });
    });

    const removeWave = () => cleanupWave(target);

    wave.addEventListener('transitionend', removeWave, { once: true });

    const cleanupTimer = setTimeout(removeWave, isSmallTarget ? 500 : 2200);
    waveCleanupTimers.set(target, cleanupTimer);
}

export function initialize(element, targetSelector) {
    if (!element || waveHandlers.has(element)) {
        return;
    }

    const handler = () => showWave(element, targetSelector);
    element.addEventListener('click', handler, true);
    waveHandlers.set(element, handler);
}

export function destroy(element) {
    if (!element) {
        return;
    }

    const handler = waveHandlers.get(element);

    if (handler) {
        element.removeEventListener('click', handler, true);
        waveHandlers.delete(element);
    }
}