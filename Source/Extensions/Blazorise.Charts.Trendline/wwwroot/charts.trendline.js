import { getChart, compileOptionCallbacks } from "../Blazorise.Charts/charts.js";
import { deepClone } from "../Blazorise.Charts/utilities.js";

Chart.defaults.set('plugins.trendline', {
    style: 'rgba(255,105,180, .8)',
    lineStyle: 'solid',
    width: 2,
});

export function initialize(dotNetAdapter, canvas, canvasId, vertical, streamOptions) {
    const chart = getChart(canvasId);

    if (chart) {
        // Chart v3 options now contains all kind of objects and functions in options. Thats why we need to deep
        // clone options without getting the reference to any running object or function. Otherwise we
        // will run into recursion errors.
        let options = deepClone(chart.originalOptions);

        options = compileOptionCallbacks(options);

        let trendlineOptions =
        {
            trendlineLinear: {
                style: "rgba(255,105,180, .8)",
                lineStyle: "solid",
                width: 4
            }
        };
        
        const merged = { ...options, ...trendlineOptions }
        chart.options = merged;

        chart.update();
    }

    return true;
}