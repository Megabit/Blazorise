import { getChart, compileOptionCallbacks } from "../Blazorise.Charts/charts.js";
import { deepClone } from "../Blazorise.Charts/utilities.js";

export function initialize(dotNetAdapter, canvas, canvasId, vertical, trendlineOptions) {
    const chart = getChart(canvasId);

    if (chart) {

        trendlineOptions.forEach(element => addTrendline(chart, element));

        chart.update();
    }

    return true;
}

function addTrendline(chart, trendlineOptions) {
    chart.data.datasets[trendlineOptions.datasetIndex].trendlineLinear = {
        style: "rgba(" + trendlineOptions.color.r + ", " + trendlineOptions.color.g + ", " + trendlineOptions.color.b + ", " + trendlineOptions.color.a + ")",
        lineStyle: trendlineOptions.lineStyle,
        width: trendlineOptions.width
    };
}