let loadPromise;

export function load() {
    if (globalThis.grecaptcha)
        return Promise.resolve();

    if (loadPromise)
        return loadPromise;

    loadPromise = new Promise((resolve, reject) => {
        const script = document.createElement("script");
        script.src = "https://www.google.com/recaptcha/api.js";
        script.async = true;
        script.defer = true;
        script.onload = resolve;
        script.onerror = () => {
            loadPromise = undefined;
            reject(new Error("Unable to load Google reCAPTCHA."));
        };

        document.head.appendChild(script);
    });

    return loadPromise;
}