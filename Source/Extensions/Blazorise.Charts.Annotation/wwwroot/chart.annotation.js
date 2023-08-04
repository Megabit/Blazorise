import { getChart } from "../Blazorise.Charts/charts.js?v=1.2.2.0";
import { parseFunction } from "../Blazorise.Charts/utilities.js?v=1.2.2.0";

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