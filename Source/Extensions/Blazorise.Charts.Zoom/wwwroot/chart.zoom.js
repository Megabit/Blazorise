import { getChart } from "../Blazorise.Charts/charts.js?v=1.8.1.0";

export function initialize(dotNetAdapter, canvasId, pluginOptions) {
    const chart = getChart(canvasId);

    if (chart) {
        if (pluginOptions) {
            if (!chart.options.plugins) {
                chart.options.plugins = [];
            }

            if (!chart.options.transitions) {
                chart.options.transitions = [];
            }

            if (pluginOptions.zoom) {
                pluginOptions.zoom.onZoom = ({ chart, trigger }) => {
                    if (dotNetAdapter) {
                        const amount = chart.getZoomLevel();

                        if (!isFinite(amount) || isNaN(amount)) {
                            return;
                        }

                        invokeDotNetMethodAsync(dotNetAdapter, "NotifyZoomLevel", amount, trigger);
                    }
                };
            }

            chart.options.plugins.zoom = pluginOptions;

            if (pluginOptions.transition) {
                chart.options.transitions.zoom = pluginOptions.transition;
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