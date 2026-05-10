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
        },

        generateToc: (targetElement, options) => {
            if (!document.getElementById('TableOfContents')) {
                return;
            }

            const contentSelector = '.b-docs-page';
            const headingSelector = [
                '.b-docs-page > h2',
                '.b-docs-page > h3',
                '.b-docs-page > h4',
                '.b-docs-page > div > h2',
                '.b-docs-page > div > h3',
                '.b-docs-page > div > h4',
                '.b-docs-page > div > div > h2',
                '.b-docs-page > div > div > h3',
                '.b-docs-page > div > div > h4'
            ].join(', ');

            document.querySelectorAll(headingSelector).forEach(function (el) {
                if (el && !el.id && el.textContent) {
                    const textContent = el.textContent.trim();
                    el.id = 'toc_' + textContent.replace(/[^A-Za-z0-9]/g, '-');
                }
            });

            tocbot.destroy()
            tocbot.init({
                basePath: options.basePath,
                tocSelector: '#TableOfContents',
                contentSelector: contentSelector,
                headingSelector: headingSelector,
                hasInnerContainers: false,
                orderedList: false,
                activeLinkClass: 'active',
                scrollSmooth: true,
                // Smooth scroll duration.
                scrollSmoothDuration: 420,
                // Smooth scroll offset.
                scrollSmoothOffset: 0,
                // Callback for scroll end.
                scrollEndCallback: function (e) { },
                disableTocScrollSync: false,
                //activeLinkClass: 'is-active-link',
                //activeListItemClass: 'is-active-li',
                onClick: function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    const element = e.target;

                    if (element && element.href) {
                        const url = new URL(element.href);

                        if (url) {
                            const hash = url.hash;
                            if (hash && hash.length > 0) {
                                const section = document.getElementById(hash.substring(1));

                                if (section) {
                                    section.scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'start' });
                                }
                            }
                        }
                    }
                },
                headingObjectCallback: function (object, element) {
                    if (object && object.textContent && !object.id && !element.id) {
                        object.id = 'toc_' + object.textContent.replace(/[^A-Za-z0-9]/g, '-');
                    }

                    return object;
                }
            });
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