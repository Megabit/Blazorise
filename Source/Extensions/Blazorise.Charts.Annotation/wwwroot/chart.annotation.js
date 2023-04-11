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

function compileDatasetsOptionsCallbacks(options) {
    if (!options) {
        return;
    }

    Object.keys(options).forEach(function (key) {
        if (options[key] && options[key].startsWith("function")) {
            options[key] = parseFunction(options[key]);
        }
    });

    return options;
}