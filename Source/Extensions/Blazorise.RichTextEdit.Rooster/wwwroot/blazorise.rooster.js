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

    instance.editor = roosterjs.createEditor(element);

    instance.editor.setContent('Welcome to <b>RoosterJs</b>!');

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

export function format(element, elementId, action, args) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (!instance)
        return;

    roosterjs[action](instance.editor, args);
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