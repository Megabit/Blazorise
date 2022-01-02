import { getChart, compileOptionCallbacks } from "../Blazorise.Charts/charts.js";

export function addTrendlines(canvasId, trendlines) {

    const chart = getChart(canvasId);

    if (chart && trendlines) {

        trendlines.forEach(element => addTrendline(chart, element));
        chart.update();

    }
    return true;

}

function addTrendline(chart, trendline) {

    if (chart.data.datasets[trendline.datasetIndex]) {

        chart.data.datasets[trendline.datasetIndex].trendlineLinear = {
            style: "rgba(" + trendline.color.r + ", " + trendline.color.g + ", " + trendline.color.b + ", " + trendline.color.a + ")",
            lineStyle: trendline.lineStyle,
            width: trendline.width
        };

    }

}