import { getChart } from "../Blazorise.Charts/charts.js?v=1.7.6.0";

export function addZoom(dotNetAdapter, canvasId, options) {
    const chart = getChart(canvasId);

    if (chart) {
        if (options) {
            if (!chart.options.plugins) {
                chart.options.plugins = [];
            }

            if (!chart.options.transitions) {
                chart.options.transitions = [];
            }

            if (options.zoom) {
                options.zoom.onZoom = ({ chart }) => {
                    if (dotNetAdapter) {
                        invokeDotNetMethodAsync(dotNetAdapter, "NotifyZoomLevel", chart.getZoomLevel());
                    }
                };
            }
            chart.options.plugins.zoom = options;

            if (options.transition) {
                chart.options.transitions.zoom = options.transition;
            }
        }

        chart.update();
    }
}

export function getZoomLevel(canvasId) {
    const chart = getChart(canvasId);

    if (chart) {
        return chart.getZoomLevel();
    }

    return 1;
}

export function setZoomLevel(canvasId, zoomLevel) {
    const chart = getChart(canvasId);

    if (chart) {
        chart.zoom(zoomLevel);
    }
}

export function resetZoomLevel(canvasId) {
    const chart = getChart(canvasId);

    if (chart) {
        chart.resetZoom();
    }
}

export function isZoomingOrPanning(canvasId) {
    const chart = getChart(canvasId);

    if (chart) {
        return chart.isZoomingOrPanning();
    }

    return false;
}

export function isZoomedOrPanned(canvasId) {
    const chart = getChart(canvasId);

    if (chart) {
        return chart.isZoomedOrPanned();
    }

    return false;
}

function invokeDotNetMethodAsync(dotNetAdapter, methodName, ...args) {
    dotNetAdapter.invokeMethodAsync(methodName, ...args)
        .catch((reason) => {
            console.error(reason);
        });
}