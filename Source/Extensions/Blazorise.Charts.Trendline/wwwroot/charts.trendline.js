import { getChart } from "../Blazorise.Charts/charts.js?v=1.4.1.0";

export function addTrendlines(canvasId, trendlines) {
    const chart = getChart(canvasId);

    if (chart && trendlines) {

        trendlines.forEach(element => addTrendline(chart, element));
    }

    return true;
}

function addTrendline(chart, trendlineData) {
    if (chart.data.datasets[trendlineData.datasetIndex]) {

        chart.data.datasets[trendlineData.datasetIndex].trendlineLinear = {
            style: "rgba(" + trendlineData.color.r + ", " + trendlineData.color.g + ", " + trendlineData.color.b + ", " + trendlineData.color.a + ")",
            lineStyle: trendlineData.lineStyle,
            width: trendlineData.width
        };
    }
}