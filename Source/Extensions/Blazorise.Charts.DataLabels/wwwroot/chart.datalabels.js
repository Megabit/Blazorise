import { getChart } from "../Blazorise.Charts/charts.js?v=1.1.2.0";

// Register the plugin to all charts:
Chart.register(ChartDataLabels);

export function setDataLabels(canvasId, datasets, options) {
    Retry(function () { return getChart(canvasId) }, 100, 10).then(chart => {
        console.log(chart);

        if (chart) {
            if (options) {
                if (!chart.options.plugins) {
                    chart.options.plugins = [];
                }

                chart.options.plugins.datalabels = adjustOptions(options);
            }

            if (datasets) {
                datasets.forEach((dataset) => {
                    setDatasetDataLabels(chart, dataset.datasetIndex, adjustOptions(dataset.options));
                });
            }

            chart.update();
        }
    });
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

async function Retry(action, retryInterval = 100, maxAttemptCount = 5) {
    const exceptions = [];
    for (let attempted = 0; attempted < maxAttemptCount; attempted++) {
        try {
            if (attempted > 0)
                await sleep(retryInterval);
            return action();
        }
        catch (e) {
            exceptions.push(e);
        }
    }

    return exceptions;
}

function sleep(ms) { return new Promise(resolve => setTimeout(resolve, ms)); }