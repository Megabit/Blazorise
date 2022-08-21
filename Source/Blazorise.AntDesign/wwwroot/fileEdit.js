import { getRequiredElement } from "../Blazorise/utilities.js?v=1.1.0.0-preview1";
import { initialize, destroy, reset } from "../Blazorise/fileEdit.js?v=1.1.0.0-preview1";

export function open(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    element.click();
}

export { initialize, destroy, reset };