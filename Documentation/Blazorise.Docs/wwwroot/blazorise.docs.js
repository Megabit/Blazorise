window.blazoriseDocs = {
    code: {
        copyToClipboard: (text) => {
            navigator.clipboard.writeText(text);
        }
    },

    navigation: {
        scrollToTop: () => {
            var element = document.getElementById("docs-layout-header");

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