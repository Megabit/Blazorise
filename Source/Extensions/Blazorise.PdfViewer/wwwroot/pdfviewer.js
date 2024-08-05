import "./vendors/pdf.min.mjs?v=1.6.0.0";

import { getRequiredElement } from "../Blazorise/utilities.js?v=1.6.0.0";

document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", "<link rel=\"stylesheet\" href=\"_content/Blazorise.PdfViewer/vendors/pdf_viewer.min.css\" />");

const _instances = [];

export async function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const instance = {
        options: options,
    };


    _instances[elementId] = instance;
}

export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {


        delete instances[elementId];
    }
}