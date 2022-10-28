import { getChart } from "../Blazorise.Charts/charts.js?v=1.1.2.0";

// Register the plugin to all charts:
Chart.register(ChartDataLabels);

export function setDataLabels(canvasId, datasets, options) {
    const chart = getChart(canvasId);

    if (chart) {
        if (options) {
            chart.options.plugins.datalabels = adjustOptions(options);
        }

        if (datasets) {
            datasets.forEach((dataset) => {
                setDatasetDataLabels(chart, dataset.datasetIndex, adjustOptions(dataset.options));
            });
        }

        chart.update();
    }

    return true;
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