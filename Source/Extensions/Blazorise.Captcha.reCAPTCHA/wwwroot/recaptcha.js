function renderReCAPTCHA(dotNetObjectRef, selector, sitekey, theme, size, language) {

    grecaptcha.ready(function () {
        grecaptcha.render(selector, {
            'sitekey': sitekey,
            'callback': (response) => { dotNetObjectRef.invokeMethodAsync('OnSuccessHandler', response); },
            'expired-callback': () => { dotNetObjectRef.invokeMethodAsync('OnExpiredHandler'); },
            'theme': theme,
            'size': size,
            'hl': language
        });
    });
};

function resetReCAPTCHA() {
    grecaptcha.reset();
}