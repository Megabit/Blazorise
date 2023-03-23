import { getRequiredElement, loadScript } from "../Blazorise/utilities.js?v=1.2.2.0";

const _instances = [];


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

