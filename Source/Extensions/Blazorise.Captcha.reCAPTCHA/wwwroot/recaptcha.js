function renderReCAPTCHA(dotNetObj, selector, sitekey, theme, language) {
    grecaptcha.ready(function () {
        grecaptcha.render(selector, {
            'sitekey': sitekey,
            'callback': (response) => { dotNetObj.invokeMethodAsync('OnSuccessHandler', response); },
            'expired-callback': () => { dotNetObj.invokeMethodAsync('OnExpiredHandler'); },
            'theme': theme.replace(/"/g, "'"),
            'hl': language
        });
};