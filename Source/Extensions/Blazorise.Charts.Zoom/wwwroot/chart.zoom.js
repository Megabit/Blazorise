import { getChart } from "../Blazorise.Charts/charts.js?v=1.7.2.0";

export function addZoom(canvasId, options) {
    const chart = getChart(canvasId);

    if (chart) {
        if (options) {
            if (!chart.options.plugins) {
                chart.options.plugins = [];
            }

            if (!chart.options.transitions) {
                chart.options.transitions = [];
            }

            chart.options.plugins.zoom = options;

            if (options.transition) {
                chart.options.transitions.zoom = options.transition;
            }
        }

        chart.update();
    }
}