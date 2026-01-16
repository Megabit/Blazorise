import { getChart } from "../Blazorise.Charts/charts.js?v=1.8.9.0";
import { parseFunction } from "../Blazorise.Charts/utilities.js?v=1.8.9.0";

export function setDataLabels(canvasId, datasets, options) {
    const chart = getChart(canvasId);

    if (chart) {
        if (options) {
            if (!chart.options.plugins) {
                chart.options.plugins = [];
            }

            chart.options.plugins.datalabels = adjustOptions(options);
        }

        if (datasets) {
            datasets.forEach((dataset) => {
                compileDatasetsOptionsCallbacks(dataset.options);

                setDatasetDataLabels(chart, dataset.datasetIndex, adjustOptions(dataset.options));
            });
        }

        chart.update();
    }
}

function setDatasetDataLabels(chart, datasetIndex, options) {
    if (chart.data && chart.data.datasets && chart.data.datasets[datasetIndex]) {
        chart.data.datasets[datasetIndex].datalabels = options;
    }
}

function adjustOptions(options) {
    if (options.formatter) {
        if (options.formatter === 0) {
            options.formatter = Math.round;
        } else if (options.formatter === 1) {
            options.formatter = Math.floor;
        } else if (options.formatter === 2) {
            options.formatter = Math.ceil;
        }
    }

    return options;
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