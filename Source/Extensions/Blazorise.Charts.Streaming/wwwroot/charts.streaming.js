import { getChart, compileOptionCallbacks } from "../Blazorise.Charts/charts.js";
import { deepClone } from "../Blazorise.Charts/utilities.js";

Chart.defaults.set('plugins.streaming', {
    duration: 20000
});

export function initialize(dotNetAdapter, canvas, canvasId, vertical, streamOptions) {
    const chart = getChart(canvasId);

    if (chart) {
        // Chart v3 options now contains all kind of objects and functions in options. Thats why we need to deep
        // clone options without getting the reference to any running object or function. Otherwise we
        // will run into recursion errors.
        let options = deepClone(chart.originalOptions);

        options = compileOptionCallbacks(options);

        let scalesOptions = getStreamingOptions(dotNetAdapter, vertical, options, streamOptions);

        // merge all options
        const merged = Object.assign({}, options, scalesOptions);
        chart.options = merged;

        chart.update();
    }

    return true;
}

export function destroy(canvas, canvasId) {
    const chart = getChart(canvasId);

    if (chart && chart.options && chart.options.scales) {
        const scales = chart.options.scales;

        // unsubscribe events
        if (scales.x && scales.x.realtime) {
            scales.x.realtime.onRefresh = null;
        }

        if (scales.y && scales.y.realtime) {
            scales.y.realtime.onRefresh = null;
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
                y: axeOptions
            }
        };

        // this is needed so that any additional axes option can be merged
        if (chartOptions && chartOptions.scales && chartOptions.scales.x) {
            verticalScalesOptions.scales.x = chartOptions.scales.x;
        }

        return verticalScalesOptions;
    }

    let horizontalScalesOptions = {
        scales: {
            x: axeOptions,
            y: chartOptions.scales.y,
        }
    };

    // this is needed so that any additional axes option can be merged
    if (chartOptions && chartOptions.scales && chartOptions.scales.y) {
        horizontalScalesOptions.scales.y = chartOptions.scales.y;
    }

    return horizontalScalesOptions;
}