import { getChart } from "../Blazorise.Charts/charts.js?v=1.7.4.0";

export function addAnnotation(canvasId, options) {
    const chart = getChart(canvasId);

    if (chart) {
        if (options) {
            if (!chart.options.plugins) {
                chart.options.plugins = [];
            }

            chart.options.plugins.annotation = adjustOptions(options);
        }

        chart.update();
    }
}

function adjustOptions(options) {
    return { annotations: options };
}