import { getRequiredElement } from "../Blazorise/utilities.js?v=1.6.2.0";

const _instances = [];

export function initialize(dotNetObjectRef, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = {
        grecaptchaId: null,
    }
    grecaptcha.ready(function () {
        instance.grecaptchaId = grecaptcha.render(element, {
            'sitekey': options.siteKey,
            'callback': (response) => { dotNetObjectRef.invokeMethodAsync('OnSuccessHandler', response); },
            'expired-callback': () => { dotNetObjectRef.invokeMethodAsync('OnExpiredHandler'); },
            'action': elementId,
            'theme': options.theme,
            'size': options.size,
            'badge': options.badge,
            'hl': options.language
        });
    });

    _instances[elementId] = instance;
};

export function execute(element, elementId) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    grecaptcha.execute(instance.grecaptchaId);
}

export function reset(element, elementId) {
    const instance = _instances[elementId];

    if (!instance)
        return;

    grecaptcha.reset(instance.grecaptchaId);
}

export function destroy(element, elementId) {
    reset(element, elementId);
    const instances = _instances || {};
    delete instances[elementId];
}