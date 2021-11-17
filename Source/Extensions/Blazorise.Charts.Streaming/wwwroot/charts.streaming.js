import { getChart } from "/_content/Blazorise.Charts/charts.js";

export function initialize(dotNetAdapter, canvas, canvasId, vertical, streamOptions) {
    const chart = getChart(canvasId);

    if (chart) {
        const scalesOptions = getStreamingOptions(dotNetAdapter, vertical, chart.options, streamOptions);

        // merge all options
        chart.options = { ...chart.options, ...scalesOptions };

        chart.update();
    }

    return true;
}

export function destroy(canvas, canvasId) {
    const chart = getChart(canvasId);

    if (chart && chart.options && chart.options.scales) {
        const scales = chart.options.scales;

        // unsubscribe events
        if (scales.xAxes) {
            scales.xAxes.forEach(function (axe) {
                if (axe.realtime) {
                    axe.realtime.onRefresh = null;
                }
            });
        }

        if (scales.yAxes) {
            scales.yAxes.forEach(function (axe) {
                if (axe.realtime) {
                    axe.realtime.onRefresh = null;
                }
            });
        }
    }
}

export function addData(canvasId, datasetIndex, newData) {
    const chart = getChart(canvasId);

    if (chart) {
        chart.data.datasets[datasetIndex].data.push(newData);
    }

    return true;
}

function getStreamingOptions(dotNetAdapter, vertical, chartOptions, streamOptions) {
    const axeOptions = {
        type: "realtime",
        realtime: {
            duration: streamOptions.duration,
            refresh: streamOptions.refresh,
            delay: streamOptions.delay,
            frameRate: streamOptions.frameRate,
            onRefresh: function (chart) {
                dotNetAdapter.invokeMethodAsync("Refresh")
                    .catch((reason) => {
                        console.error(reason);
                    });
            }
        }
    };

    if (vertical) {
        let verticalScalesOptions = {
            scales: {
                yAxes: [axeOptions]
            }
        };

        // this is needed so that any additional axes option can be merged
        if (chartOptions && chartOptions.scales && chartOptions.scales.xAxes) {
            verticalScalesOptions.scales.xAxes = chartOptions.scales.xAxes;
        }

        return verticalScalesOptions;
    }

    let horizontalScalesOptions = {
        scales: {
            xAxes: [axeOptions],
            yAxes: chartOptions.scales.yAxes,
        }
    };

    // this is needed so that any additional axes option can be merged
    if (chartOptions && chartOptions.scales && chartOptions.scales.yAxes) {
        horizontalScalesOptions.scales.yAxes = chartOptions.scales.yAxes;
    }

    return horizontalScalesOptions;
}