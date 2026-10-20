window.blazoriseDocs = {
    code: {
        copyToClipboard: (text) => {
            navigator.clipboard.writeText(text);
        },
        hasVerticalOverflow: (elementId) => {
            const element = document.getElementById(elementId);

            return element ? element.scrollHeight > element.clientHeight : false;
        }
    },

    navigation: {
        scrollToTop: () => {
            var element = document.getElementById("b-docs-layout-header");

            if (element) {
                element.scrollIntoView({
                    behavior: 'auto'
                });
            }
        }
    }
}

window.myComponent = {
    configureQuillJs: () => {
        var link = Quill.import("formats/link");

        link.sanitize = url => {
            let newUrl = window.decodeURIComponent(url);
            newUrl = newUrl.trim().replace(/\s/g, "");

            if (/^(:\/\/)/.test(newUrl)) {
                return `http${newUrl}`;
            }

            if (!/^(f|ht)tps?:\/\//i.test(newUrl)) {
                return `http://${newUrl}`;
            }

            return newUrl;
        }
    }
}

window.blazorisePRO = {
    paddle: {
        openCheckout: (product, quantity) => {
            Paddle.Checkout.open({
                items: [
                    {
                        priceId: product,
                        quantity: quantity
                    }
                ]
            });
        }
    }
};

const blazoriseDocsThemeCookieName = "bdocs-theme";
const blazoriseDocsSystemCookieName = "bdocs-system";

window.blazoriseDocs.theme = {
    getStoredTheme: () => {
        const cookie = document.cookie.split("; ").find((row) => row.startsWith(blazoriseDocsThemeCookieName + "="));

        return cookie ? decodeURIComponent(cookie.split("=")[1]) : null;
    },
    isSystemDark: () => window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches,
    setStoredTheme: (theme) => {
        const normalizedTheme = theme && theme.length ? theme : "System";

        document.cookie = `${blazoriseDocsThemeCookieName}=${encodeURIComponent(normalizedTheme)};path=/;max-age=31536000;SameSite=Lax`;
    },
    setSystemPreference: (isDark) => {
        document.cookie = `${blazoriseDocsSystemCookieName}=${isDark};path=/;max-age=31536000;SameSite=Lax`;
    },
    applyTheme: (theme, systemIsDarkMode) => {
        const normalizedTheme = theme && theme.length ? theme : "System";
        const prefersDark = normalizedTheme === "Dark" || (normalizedTheme === "System" && (typeof systemIsDarkMode === "boolean" ? systemIsDarkMode : window.blazoriseDocs.theme.isSystemDark()));
        const body = document.body;
        const html = document.documentElement;

        if (prefersDark) {
            html && html.setAttribute("data-bs-theme", "dark");
            body && body.setAttribute("data-bs-theme", "dark");
        } else {
            html && html.removeAttribute("data-bs-theme");
            body && body.removeAttribute("data-bs-theme");
        }
    },
    save: (theme, systemIsDarkMode) => {
        const prefersDark = typeof systemIsDarkMode === "boolean" ? systemIsDarkMode : window.blazoriseDocs.theme.isSystemDark();

        window.blazoriseDocs.theme.setStoredTheme(theme);
        window.blazoriseDocs.theme.setSystemPreference(prefersDark);
        window.blazoriseDocs.theme.applyTheme(theme, prefersDark);
    }
};