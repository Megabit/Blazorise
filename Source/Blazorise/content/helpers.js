if (!window.blazorise) {
    window.blazorise = {};
}

window.blazorise = {
    init: (element, componentReference) => {
        return true;
    },

    // adds a classname to the specified element
    addClass: (element, classname) => {
        element.classList.add(classname);
        return true;
    },

    // removes a classname from the specified element
    removeClass: (element, classname) => {
        if (element.classList.contains(classname)) {
            element.classList.remove(classname);
        }
        return true;
    },
    // toggles a classname on the given element id
    toggleClass: (element, classname) => {
        if (element) {
            if (element.classList.contains(classname)) {
                element.classList.remove(classname);
            } else {
                element.classList.add(classname);
            }
        }
        return true;
    },

    // adds a classname to the body element
    addClassToBody: (classname) => {
        return blazorise.addClass(document.body, classname);
    },

    // removes a classname from the body element
    removeClassFromBody: (classname) => {
        return blazorise.removeClass(document.body, classname);
    },

    // indicates if parent node has a specified classname
    parentHasClass: (element, classname) => {
        if (element && element.parentElement) {
            return element.parentElement.classList.contains(classname);
        }
        return false;
    },

    setTextValue(element, value) {
        element.value = value;
        return true;
    },

    getFilePaths: (element) => {
        var paths = [];

        if (element) {
            var files = element.files;

            for (var i = 0; i < files.length; i++) {
                paths.push(files[i].name);
            }
        }

        return paths;
    },

    getSelectedOptions: (elementId) => {
        const element = document.getElementById(elementId);
        const len = element.options.length;
        var opts = [], opt;

        for (var i = 0; i < len; i++) {
            opt = element.options[i];

            if (opt.selected) {
                opts.push(opt.value);
            }
        }

        return opts;
    },

    // holds the list of components that are triggers to close other components
    closableComponents: [],

    addClosableComponent: (elementId, dotnetAdapter) => {
        window.blazorise.closableComponents.push({ elementId: elementId, dotnetAdapter: dotnetAdapter });
    },

    findClosableComponent: (elementId) => {
        for (index = 0; index < window.blazorise.closableComponents.length; ++index) {
            if (window.blazorise.closableComponents[index].elementId === elementId)
                return window.blazorise.closableComponents[index];
        }
        return null;
    },

    isClosableComponent: (elementId) => {
        for (index = 0; index < window.blazorise.closableComponents.length; ++index) {
            if (window.blazorise.closableComponents[index].elementId === elementId)
                return true;
        }
        return false;
    },

    registerClosableComponent: (elementId, dotnetAdapter) => {
        if (window.blazorise.isClosableComponent(elementId) !== true) {
            window.blazorise.addClosableComponent(elementId, dotnetAdapter);
        }
    },

    unregisterClosableComponent: (elementId) => {
        const closable = window.blazorise.findClosableComponent(elementId);
        if (closable) {
            window.blazorise.closableComponents.splice(closable, 1);
        }
    }
};

document.addEventListener('click', function handler(evt) {
    const clickedElementId = evt.target.id;

    let requests = window.blazorise.closableComponents.map(closable => {
        return new Promise((resolve, reject) => {
            closable.dotnetAdapter.invokeMethodAsync('SafeToClose', clickedElementId)
                .then((result) => resolve({ elementId: closable.elementId, dotnetAdapter: closable.dotnetAdapter, status: result === true ? 'ok' : 'cancelled' }))
                .catch(() => resolve({ elementId: closable.elementId, status: 'error' }));
        });
    });

    Promise.all(requests)
        .then(responses => responses.forEach((response) => {
            if (response.status === 'ok') {
                response.dotnetAdapter.invokeMethodAsync('Close')
                    //.then(() => window.blazorise.unregisterClosableComponent(response.elementId))
                    // If the user navigates to another page then it will raise exception because the reference to the component cannot be found.
                    // In that case just remove the elementId from the list.
                    .catch(() => window.blazorise.unregisterClosableComponent(response.elementId));
            }
        }));
});