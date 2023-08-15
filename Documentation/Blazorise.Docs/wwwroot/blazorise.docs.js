window.blazoriseDocs = {
    code: {
        copyToClipboard: (text) => {
            navigator.clipboard.writeText(text);
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
            document.querySelectorAll('.b-docs-page>h2, .b-docs-page>h3, .b-docs-page>h4, .b-docs-page-section-header>h3').forEach(function (el) {
                if (el && !el.id && el.textContent) {
                    const textContent = el.textContent.trim();
                    el.id = 'toc_' + textContent.replace(/[^A-Za-z0-9]/g, '-');
                }
            });

            tocbot.destroy()
            tocbot.init({
                basePath: options.basePath,
                tocSelector: '#TableOfContents',
                contentSelector: '.b-docs-page',
                headingSelector: '.b-docs-page>h2, .b-docs-page>h3, .b-docs-page>h4, .b-docs-page-section-header>h3',
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
        openCheckout: (product, quantity, upsell) => {
            if (upsell) {
                Paddle.Checkout.open({
                    product: product,
                    quantity: quantity,
                    upsell: upsell.id,
                    upsellTitle: upsell.title,
                    upsellText: upsell.text,
                    upsellAction: upsell.action,
                });
            }
            else {
                Paddle.Checkout.open({
                    product: product,
                    quantity: quantity
                });
            }
        }
    },
    wspay: {
        submit: (elementId, data) => {
            const form = document.getElementById(elementId);

            if (form) {
                const shoppingCartIDInput = document.getElementsByName("ShoppingCartID")[0];
                const signatureInput = document.getElementsByName("Signature")[0];

                if (shoppingCartIDInput && signatureInput) {
                    shoppingCartIDInput.value = data.shoppingCartID;
                    signatureInput.value = data.signature;

                    form.submit();
                }
            }
        }
    }
};
