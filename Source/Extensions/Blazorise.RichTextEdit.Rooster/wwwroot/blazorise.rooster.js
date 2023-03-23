import { getRequiredElement, loadScript } from "../Blazorise/utilities.js?v=1.2.2.0";

const _instances = [];

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"_content/Blazorise.RichTextEdit.Rooster/blazorise.rooster.css?v=1.2.2.0\" />");

export async function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    if (typeof roosterjs === 'undefined') {
        await loadScript("_content/Blazorise.RichTextEdit.Rooster/vendors/rooster.js?v=4.88.0");
    }

    const instance = {
        options: options,
        adapter: dotNetAdapter,
        rooster: null,
    };

    instance.rooster = roosterjs.createEditor(element);

    instance.rooster.setContent('Welcome to <b>RoosterJs</b>!');

    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (!instance)
        return;

    instance.rooster.dispose();
    delete instances[elementId];
}

