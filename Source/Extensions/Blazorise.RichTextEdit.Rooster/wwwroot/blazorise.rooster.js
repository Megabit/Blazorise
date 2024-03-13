import { getRequiredElement } from "../Blazorise/utilities.js?v=1.4.2.0";

const _instances = [];

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"_content/Blazorise.RichTextEdit.Rooster/blazorise.rooster.css?v=1.4.2.0\" />");

export async function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    if (typeof roosterjs === 'undefined') {
        await loadRoosterJs();
    }

    const instance = {
        options: options,
        adapter: dotNetAdapter,
        editor: null,
    };

    let plugins = [
        new BlazoriseRichTextEditPlugin(dotNetAdapter)
    ];

    instance.editor = roosterjs.createEditor(element, plugins);    

    if (options.content) {
        instance.editor.setContent(options.content);
    }

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (!instance)
        return;

    instance.editor.dispose();
    delete instances[elementId];
}

export function roosterApi(element, elementId, action, args) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (!instance)
        return;

    if (typeof roosterjs[action] == 'function') {
        roosterjs[action](instance.editor, args);
    }
    else {
        instance.editor[action](args);
    }
}

export function setContent(element, elementId, content) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (!instance)
        return;

    instance.editor.setContent(content);
}

export function getContent(element, elementId, mode) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (!instance)
        return;

    return instance.editor.getContent(mode);
}

function loadRoosterJs() {
    return new Promise((resolve, reject) => {
        try {
            const scriptEle = document.createElement("script");
            scriptEle.type = "text/javascript";
            scriptEle.async = true;
            scriptEle.src = "_content/Blazorise.RichTextEdit.Rooster/vendors/rooster.js?v=1.4.2.0";

            scriptEle.addEventListener("load", (ev) => {
                resolve({ status: true });
            });

            scriptEle.addEventListener("error", (ev) => {
                reject({
                    status: false,
                    message: `Failed to load roosterjs`
                });
            });

            document.body.appendChild(scriptEle);
        } catch (error) {
            reject(error);
        }
    });
}

// Throttle function => utilities.js?
function throttle(cb, delay) {
    let wait = false;
    let storedArgs = null;

    function checkStoredArgs() {
        if (storedArgs == null) {
            wait = false;
        } else {
            cb(...storedArgs);
            storedArgs = null;
            setTimeout(checkStoredArgs, delay);
        }
    }

    return (...args) => {
        if (wait) {
            storedArgs = args;
            return;
        }

        cb(...args);
        wait = true;
        setTimeout(checkStoredArgs, delay);
    }
}

class BlazoriseRichTextEditPlugin {
    constructor(dotNetAdapter) {
        this.dotNetAdapter = dotNetAdapter;
    }

    getName() {
        return "BlazoriseRichTextEditPlugin"
    }

    initialize(editor) {
        this.editor = editor
        this.changeDisposer = this.editor.addDomEventHandler(
            "input",
            this.onChangeEvent
        )
        this.textInputDisposer = this.editor.addDomEventHandler(
            "textinput",
            this.onChangeEvent
        )
        this.pasteDisposer = this.editor.addDomEventHandler(
            "paste",
            this.onChangeEvent
        )

        // Throttle changed event otherwise blazor gets bombed
        this.eventHandler = throttle(() => this.dotNetAdapter
            .invokeMethodAsync('OnContentChanged', this.editor.getContent())
            .then(null, err => { throw new Error(err); }), 250);
   }

    onPluginEvent(event) {
        if (event && event.eventType === roosterjs.PluginEventType.ContentChanged) {
            this.onChangeEvent()
        }
    }

    dispose() {
        if (this.changeDisposer) {
            this.changeDisposer()
            this.changeDisposer = null
        }
        if (this.textInputDisposer) {
            this.textInputDisposer()
            this.textInputDisposer = null
        }
        if (this.pasteDisposer) {
            this.pasteDisposer()
            this.pasteDisposer = null
        }

        this.editor = null
    }

    onChangeEvent = () => this.eventHandler();
}
