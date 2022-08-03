import { getRequiredElement } from "../Blazorise/utilities.js?v=1.0.6.0";
import { initialize, destroy, reset } from "../Blazorise/fileEdit.js?v=1.0.6.0";

export function open(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    element.click();
}

export { initialize, destroy, reset };