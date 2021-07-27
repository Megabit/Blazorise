window.blazorise.tooltip = {
    _instances: [],

        initialize: (element, elementId, options) => {
            if (!element) {
                return;
            }

            const defaultOptions = {
                theme: 'blazorise',
                content: options.text,
                placement: options.placement,
                maxWidth: options.maxWidth ? options.maxWidth : options.multiline ? "15rem" : null,
                duration: options.fade ? [options.fadeDuration, options.fadeDuration] : [0, 0],
                arrow: options.showArrow,
                allowHTML: true,
                trigger: options.trigger
            };

            const alwaysActiveOptions = options.alwaysActive
                ? {
                    showOnCreate: true,
                    hideOnClick: false,
                    trigger: "manual"
                } : {};

            const instance = tippy(element, {
                ...defaultOptions,
                ...alwaysActiveOptions
            });

            window.blazorise.tooltip._instances[elementId] = instance;
        },
            destroy: (element, elementId) => {
                var instances = window.blazorise.tooltip._instances || {};

                const instance = instances[elementId];

                if (instance) {
                    instance.hide();

                    delete instances[elementId];
                }
            },
                updateContent: (element, elementId, content) => {
                    const instance = window.blazorise.tooltip._instances[elementId];

                    if (instance) {
                        instance.setContent(content);
                    }
                }
};