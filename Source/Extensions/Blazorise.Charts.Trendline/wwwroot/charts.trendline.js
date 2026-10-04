import { getChart } from "../Blazorise.Charts/charts.js?v=__BLAZORISE_VERSION__";

import "./vendors/chartjs-plugin-trendline.js?v=__BLAZORISE_VERSION__";

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