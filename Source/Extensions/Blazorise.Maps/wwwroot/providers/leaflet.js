const instances = [];

let leafletLoader;

export function createLeafletProvider() {
    return {
        initialize,
        destroy,
        setView,
        panTo,
        fitBounds,
        zoomIn,
        zoomOut,
        invalidateSize,
        setLayer,
        removeLayer,
        getView,
        getBounds,
    };
}

async function initialize(dotNetAdapter, element, elementId, options) {
    const L = await ensureLeaflet(options?.version);

    if (!L)
        return;

    const mapOptions = toLeafletMapOptions(options?.options);
    const view = options?.view || {};
    const center = toLatLng(view.center);
    const map = L.map(element, mapOptions);

    map.setView(center, view.zoom ?? 13);

    const instance = {
        adapter: dotNetAdapter,
        map: map,
        layers: {},
        programmaticViewChange: false,
    };

    instances[elementId] = instance;

    registerMapEvents(instance);
    registerControls(instance, options?.options);

    for (const layer of options?.layers || []) {
        setLayer(element, elementId, layer);
    }
}

function destroy(element, elementId) {
    const instance = instances[elementId];

    if (!instance)
        return;

    for (const layerId of Object.keys(instance.layers)) {
        removeLayer(element, elementId, layerId);
    }

    instance.map.off();
    instance.map.remove();

    delete instances[elementId];
}

function setView(element, elementId, view, options) {
    const instance = instances[elementId];

    if (!instance)
        return;

    instance.programmaticViewChange = true;
    instance.map.setView(toLatLng(view.center), view.zoom, toAnimationOptions(options));
    notifyViewChanged(instance, 2);
    instance.programmaticViewChange = false;
}

function panTo(element, elementId, center, options) {
    const instance = instances[elementId];

    if (!instance)
        return;

    instance.programmaticViewChange = true;
    instance.map.panTo(toLatLng(center), toAnimationOptions(options));
    notifyViewChanged(instance, 2);
    instance.programmaticViewChange = false;
}

function fitBounds(element, elementId, bounds, options) {
    const instance = instances[elementId];

    if (!instance)
        return;

    instance.programmaticViewChange = true;
    instance.map.fitBounds(toLatLngBounds(bounds), toFitBoundsOptions(options));
    notifyViewChanged(instance, 2);
    instance.programmaticViewChange = false;
}

function zoomIn(element, elementId) {
    const instance = instances[elementId];

    if (instance)
        instance.map.zoomIn();
}

function zoomOut(element, elementId) {
    const instance = instances[elementId];

    if (instance)
        instance.map.zoomOut();
}

function invalidateSize(element, elementId) {
    const instance = instances[elementId];

    if (instance)
        instance.map.invalidateSize();
}

function setLayer(element, elementId, layer) {
    const instance = instances[elementId];

    if (!instance || !layer || !layer.id)
        return;

    removeLayer(element, elementId, layer.id);

    if (!layer.visible)
        return;

    const leafletLayer = createLayer(instance, layer);

    if (!leafletLayer)
        return;

    instance.layers[layer.id] = leafletLayer;
    leafletLayer.addTo(instance.map);

    if (typeof leafletLayer.setOpacity === "function")
        leafletLayer.setOpacity(layer.opacity ?? 1);

    if (typeof leafletLayer.setZIndex === "function" && layer.zIndex !== null && layer.zIndex !== undefined)
        leafletLayer.setZIndex(layer.zIndex);
}

function removeLayer(element, elementId, layerId) {
    const instance = instances[elementId];

    if (!instance)
        return;

    const layer = instance.layers[layerId];

    if (layer) {
        instance.map.removeLayer(layer);
        delete instance.layers[layerId];
    }
}

function getView(element, elementId) {
    const instance = instances[elementId];

    if (!instance)
        return null;

    return toMapView(instance.map);
}

function getBounds(element, elementId) {
    const instance = instances[elementId];

    if (!instance)
        return null;

    return toBounds(instance.map.getBounds());
}

function createLayer(instance, layer) {
    switch (layer.kind) {
        case 0:
            return createTileLayer(layer);
        case 1:
            return createMarkerLayer(instance, layer);
        case 2:
            return createCircleLayer(layer);
        case 3:
            return createPolylineLayer(layer);
        case 4:
            return createPolygonLayer(layer);
        default:
            return null;
    }
}

function createTileLayer(layer) {
    if (!layer.urlTemplate)
        return null;

    const options = {
        tileSize: layer.tileSize || 256,
        opacity: layer.opacity ?? 1,
    };

    if (layer.attribution)
        options.attribution = layer.attribution;

    if (layer.minZoom !== null && layer.minZoom !== undefined)
        options.minZoom = layer.minZoom;

    if (layer.maxZoom !== null && layer.maxZoom !== undefined)
        options.maxZoom = layer.maxZoom;

    if (layer.subdomains !== null && layer.subdomains !== undefined && layer.subdomains.length > 0)
        options.subdomains = layer.subdomains;

    return globalThis.L.tileLayer(layer.urlTemplate, options);
}

function createMarkerLayer(instance, layer) {
    const group = globalThis.L.layerGroup();

    for (const markerOptions of layer.markers || []) {
        const options = {
            title: markerOptions.title,
            draggable: markerOptions.draggable,
            interactive: layer.interactive,
            opacity: layer.opacity ?? 1,
        };

        const icon = toMarkerIcon(markerOptions.icon);

        if (icon)
            options.icon = icon;

        const marker = globalThis.L.marker(toLatLng(markerOptions.position), options);

        if (markerOptions.tooltipText)
            marker.bindTooltip(markerOptions.tooltipText);

        if (markerOptions.popupText)
            marker.bindPopup(markerOptions.popupText);

        marker.on("click", (event) => {
            instance.adapter.invokeMethodAsync("MarkerClick", layer.id, markerOptions.id, toMouseEventArgs(event));
        });

        marker.on("dragend", () => {
            const position = marker.getLatLng();
            instance.adapter.invokeMethodAsync("MarkerDragged", layer.id, markerOptions.id, toCoordinate(position));
        });

        group.addLayer(marker);
    }

    return group;
}

function createCircleLayer(layer) {
    return globalThis.L.circle(toLatLng(layer.center), {
        ...toPathOptions(layer),
        radius: layer.radius,
    });
}

function createPolylineLayer(layer) {
    return globalThis.L.polyline((layer.coordinates || []).map(toLatLng), toPathOptions(layer));
}

function createPolygonLayer(layer) {
    return globalThis.L.polygon((layer.rings || []).map((ring) => ring.map(toLatLng)), toPathOptions(layer));
}

function registerMapEvents(instance) {
    instance.map.on("click", (event) => {
        instance.adapter.invokeMethodAsync("Click", toMouseEventArgs(event));
    });

    instance.map.on("dblclick", (event) => {
        instance.adapter.invokeMethodAsync("DoubleClick", toMouseEventArgs(event));
    });

    instance.map.on("contextmenu", (event) => {
        instance.adapter.invokeMethodAsync("ContextMenu", toMouseEventArgs(event));
    });

    instance.map.on("moveend zoomend", () => {
        notifyViewChanged(instance, instance.programmaticViewChange ? 2 : 1);
    });
}

function registerControls(instance, options) {
    const controls = options?.controls || {};

    if (controls.scale)
        globalThis.L.control.scale().addTo(instance.map);
}

function notifyViewChanged(instance, reason) {
    instance.adapter.invokeMethodAsync("ViewChanged", {
        view: toMapView(instance.map),
        bounds: toBounds(instance.map.getBounds()),
        reason: reason,
    });
}

function toLeafletMapOptions(options) {
    options = options || {};
    const controls = options.controls || {};

    const mapOptions = {
        dragging: options.interactive && options.dragging,
        scrollWheelZoom: options.interactive && options.scrollWheelZoom,
        doubleClickZoom: options.interactive && options.doubleClickZoom,
        keyboard: options.interactive && options.keyboard,
        touchZoom: options.interactive && options.touchZoom,
        zoomControl: controls.zoom,
        attributionControl: controls.attribution,
    };

    if (options.minZoom !== null && options.minZoom !== undefined)
        mapOptions.minZoom = options.minZoom;

    if (options.maxZoom !== null && options.maxZoom !== undefined)
        mapOptions.maxZoom = options.maxZoom;

    return mapOptions;
}

function toAnimationOptions(options) {
    if (!options)
        return {};

    return {
        animate: options.animate,
        duration: options.duration,
    };
}

function toFitBoundsOptions(options) {
    const result = toAnimationOptions(options);

    if (options?.padding)
        result.padding = [options.padding.x, options.padding.y];

    return result;
}

function toPathOptions(layer) {
    const style = layer.style || {};

    return {
        interactive: layer.interactive,
        opacity: style.strokeOpacity ?? 1,
        color: style.strokeColor || "#3388ff",
        weight: style.strokeWidth ?? 3,
        dashArray: style.strokeDashArray,
        fillColor: style.fillColor || style.strokeColor,
        fillOpacity: style.fillOpacity ?? 0.2,
    };
}

function toMarkerIcon(icon) {
    if (!icon || !icon.url)
        return undefined;

    return globalThis.L.icon({
        iconUrl: icon.url,
        iconSize: icon.size ? [icon.size.width, icon.size.height] : undefined,
        iconAnchor: icon.anchor ? [icon.anchor.x, icon.anchor.y] : undefined,
        className: icon.cssClass,
    });
}

function toMapView(map) {
    const center = map.getCenter();

    return {
        center: toCoordinate(center),
        zoom: map.getZoom(),
        bounds: toBounds(map.getBounds()),
    };
}

function toMouseEventArgs(event) {
    return {
        coordinate: toCoordinate(event.latlng),
        containerPoint: {
            x: event.containerPoint?.x ?? 0,
            y: event.containerPoint?.y ?? 0,
        },
    };
}

function toCoordinate(latLng) {
    return {
        latitude: latLng?.lat ?? 0,
        longitude: latLng?.lng ?? 0,
    };
}

function toBounds(bounds) {
    return {
        southWest: toCoordinate(bounds.getSouthWest()),
        northEast: toCoordinate(bounds.getNorthEast()),
    };
}

function toLatLng(coordinate) {
    return [coordinate?.latitude ?? 0, coordinate?.longitude ?? 0];
}

function toLatLngBounds(bounds) {
    return [
        toLatLng(bounds.southWest),
        toLatLng(bounds.northEast),
    ];
}

function ensureLeaflet(version) {
    const assetVersion = version || "2.1.0.0";

    if (globalThis.L?.map) {
        configureLeafletDefaultIcons(globalThis.L, assetVersion);
        return Promise.resolve(globalThis.L);
    }

    if (leafletLoader)
        return leafletLoader;

    leafletLoader = new Promise((resolve) => {
        const leafletCssUrl = `_content/Blazorise.Maps/vendors/leaflet.css?v=${assetVersion}`;
        const leafletScriptUrl = `_content/Blazorise.Maps/vendors/leaflet.js?v=${assetVersion}`;

        appendStylesheet(leafletCssUrl);

        const script = document.createElement("script");
        script.src = leafletScriptUrl;
        script.async = true;
        script.onload = () => {
            configureLeafletDefaultIcons(globalThis.L, assetVersion);
            resolve(globalThis.L);
        };
        script.onerror = () => resolve(null);
        document.head.appendChild(script);
    });

    return leafletLoader;
}

function configureLeafletDefaultIcons(L, assetVersion) {
    if (!L?.Icon?.Default)
        return;

    L.Icon.Default.imagePath = "";
    L.Icon.Default.mergeOptions({
        iconRetinaUrl: `_content/Blazorise.Maps/vendors/images/marker-icon-2x.png?v=${assetVersion}`,
        iconUrl: `_content/Blazorise.Maps/vendors/images/marker-icon.png?v=${assetVersion}`,
        shadowUrl: `_content/Blazorise.Maps/vendors/images/marker-shadow.png?v=${assetVersion}`,
    });
}

function appendStylesheet(url) {
    if ([...document.styleSheets].some((styleSheet) => styleSheet.href === url))
        return;

    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = url;
    document.head.appendChild(link);
}