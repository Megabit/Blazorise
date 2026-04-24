import { getRequiredElement, registerDisconnectCleanup, unregisterDisconnectCleanup } from "../Blazorise/utilities.js?v=2.1.1.0";
import { createLeafletProvider } from "./providers/leaflet.js?v=2.1.1.0";

const mapProviderLeaflet = 0;
const instances = [];

appendStylesheet("_content/Blazorise.Maps/blazorise.maps.css?v=2.1.1.0");

export async function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const provider = createProvider(options?.options?.provider);

    if (!provider)
        return;

    const instance = {
        provider: provider,
        disconnectCleanupId: null,
    };

    instances[elementId] = instance;

    await provider.initialize(dotNetAdapter, element, elementId, options);

    instance.disconnectCleanupId = registerDisconnectCleanup(element, () => destroy(null, elementId, false));
}

export function destroy(element, elementId, unregisterCleanup = true) {
    const instance = instances[elementId];

    if (!instance)
        return;

    if (unregisterCleanup)
        unregisterDisconnectCleanup(instance.disconnectCleanupId);

    instance.provider.destroy(element, elementId);
    instance.disconnectCleanupId = null;

    delete instances[elementId];
}

export function setView(element, elementId, view, options) {
    const instance = instances[elementId];

    if (!instance)
        return;

    instance.provider.setView(element, elementId, view, options);
}

export function panTo(element, elementId, center, options) {
    const instance = instances[elementId];

    if (!instance)
        return;

    instance.provider.panTo(element, elementId, center, options);
}

export function fitBounds(element, elementId, bounds, options) {
    const instance = instances[elementId];

    if (!instance)
        return;

    instance.provider.fitBounds(element, elementId, bounds, options);
}

export function zoomIn(element, elementId) {
    const instance = instances[elementId];

    if (instance)
        instance.provider.zoomIn(element, elementId);
}

export function zoomOut(element, elementId) {
    const instance = instances[elementId];

    if (instance)
        instance.provider.zoomOut(element, elementId);
}

export function invalidateSize(element, elementId) {
    const instance = instances[elementId];

    if (instance)
        instance.provider.invalidateSize(element, elementId);
}

export function setLayer(element, elementId, layer) {
    const instance = instances[elementId];

    if (instance)
        instance.provider.setLayer(element, elementId, layer);
}

export function removeLayer(element, elementId, layerId) {
    const instance = instances[elementId];

    if (!instance)
        return;

    instance.provider.removeLayer(element, elementId, layerId);
}

export function getView(element, elementId) {
    const instance = instances[elementId];

    if (!instance)
        return null;

    return instance.provider.getView(element, elementId);
}

export function getBounds(element, elementId) {
    const instance = instances[elementId];

    if (!instance)
        return null;

    return instance.provider.getBounds(element, elementId);
}

function createProvider(provider) {
    switch (provider ?? mapProviderLeaflet) {
        case mapProviderLeaflet:
            return createLeafletProvider();
        default:
            return null;
    }
}

function appendStylesheet(url) {
    if ([...document.styleSheets].some((styleSheet) => styleSheet.href === url))
        return;

    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = url;
    document.head.appendChild(link);
}