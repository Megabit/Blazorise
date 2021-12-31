import { getChart, compileOptionCallbacks } from "../Blazorise.Charts/charts.js";

Chart.defaults.set('plugins.trendline', {
    style: 'rgba(255,105,180, .8)',
    lineStyle: 'solid',
    width: 2,
});

export function initialize(dotNetAdapter, canvas, canvasId, vertical, streamOptions) {
    const chart = getChart(canvasId);

    if (chart) {
        //// Chart v3 options now contains all kind of objects and functions in options. Thats why we need to deep
        //// clone options without getting the reference to any running object or function. Otherwise we
        //// will run into recursion errors.
        //let options = deepClone(chart.originalOptions);

        //options = compileOptionCallbacks(options);

        //let scalesOptions = getTrendlineOptions(dotNetAdapter, vertical, options, streamOptions);

        //// merge all options
        //const merged = { ...options, ...scalesOptions }
        //chart.options = merged;

        pluginTrendlineLinear();

        chart.update();
    }

    return true;
}


/*!
 * chartjs-plugin-trendline.js
 * Version: 1.0.0
 *
 * Copyright 2021 Marcus Alsterfjord
 * Released under the MIT license
 * https://github.com/Makanz/chartjs-plugin-trendline/blob/master/README.md
 *
 * Mod by: vesal: accept also xy-data so works with scatter
 */
var pluginTrendlineLinear = {
    id: "chartjs-plugin-trendline",
    afterDraw: function (chartInstance) {
        var yScale;
        var xScale;
        for (var axis in chartInstance.scales) {
            if (axis[0] == 'x')
                xScale = chartInstance.scales[axis];
            else
                yScale = chartInstance.scales[axis];
            if (xScale && yScale) break;
        }
        var ctx = chartInstance.ctx;

        chartInstance.data.datasets.forEach(function (dataset, index) {
            if (dataset.trendlineLinear && chartInstance.isDatasetVisible(index) && dataset.data.length != 0) {
                var datasetMeta = chartInstance.getDatasetMeta(index);
                addFitter(datasetMeta, ctx, dataset, xScale, chartInstance.scales[datasetMeta.yAxisID]);
            }
        });

        ctx.setLineDash([]);
    }
};

function addFitter(datasetMeta, ctx, dataset, xScale, yScale) {
    var style = dataset.trendlineLinear.style || dataset.borderColor;
    var lineWidth = dataset.trendlineLinear.width || dataset.borderWidth;
    var lineStyle = dataset.trendlineLinear.lineStyle || "solid";

    style = (style !== undefined) ? style : "rgba(169,169,169, .6)";
    lineWidth = (lineWidth !== undefined) ? lineWidth : 3;

    var fitter = new LineFitter();
    var lastIndex = dataset.data.length - 1;
    var startPos = datasetMeta.data[0].x;
    var endPos = datasetMeta.data[lastIndex].x;

    var xy = false;
    if (dataset.data && typeof dataset.data[0] === 'object') xy = true;

    dataset.data.forEach(function (data, index) {

        if (data == null)
            return;

        if (xScale.options.type === "time") {
            var x = data.x != null ? data.x : data.t;
            fitter.add(new Date(x).getTime(), data.y);
        }
        else if (xy) {
            fitter.add(data.x, data.y);
        }
        else {
            fitter.add(index, data);
        }

    });

    var x1 = xScale.getPixelForValue(fitter.minx);
    var x2 = xScale.getPixelForValue(fitter.maxx);
    var y1 = yScale.getPixelForValue(fitter.f(fitter.minx));
    var y2 = yScale.getPixelForValue(fitter.f(fitter.maxx));
    if (!xy) { x1 = startPos; x2 = endPos; }

    var drawBottom = datasetMeta.controller.chart.chartArea.bottom;
    var chartWidth = datasetMeta.controller.chart.width;

    if (y1 > drawBottom) { // Left side is below zero
        var diff = y1 - drawBottom;
        var lineHeight = y1 - y2;
        var overlapPercentage = diff / lineHeight;
        var addition = chartWidth * overlapPercentage;

        y1 = drawBottom;
        x1 = (x1 + addition);
    } else if (y2 > drawBottom) { // right side is below zero
        var diff = y2 - drawBottom;
        var lineHeight = y2 - y1;
        var overlapPercentage = diff / lineHeight;
        var subtraction = chartWidth - (chartWidth * overlapPercentage);

        y2 = drawBottom;
        x2 = chartWidth - (x2 - subtraction);
    }

    ctx.lineWidth = lineWidth;
    if (lineStyle === "dotted") { ctx.setLineDash([2, 3]); }
    ctx.beginPath();
    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);
    ctx.strokeStyle = style;
    ctx.stroke();
}

function LineFitter() {
    this.count = 0;
    this.sumX = 0;
    this.sumX2 = 0;
    this.sumXY = 0;
    this.sumY = 0;
    this.minx = 1e100;
    this.maxx = -1e100;
}

LineFitter.prototype = {
    'add': function (x, y) {
        x = parseFloat(x);
        y = parseFloat(y);

        this.count++;
        this.sumX += x;
        this.sumX2 += x * x;
        this.sumXY += x * y;
        this.sumY += y;
        if (x < this.minx) this.minx = x;
        if (x > this.maxx) this.maxx = x;
    },
    'f': function (x) {
        x = parseFloat(x);

        var det = this.count * this.sumX2 - this.sumX * this.sumX;
        var offset = (this.sumX2 * this.sumY - this.sumX * this.sumXY) / det;
        var scale = (this.count * this.sumXY - this.sumX * this.sumY) / det;
        return offset + x * scale;
    }
};

// If we're in the browser and have access to the global Chart obj, register plugin automatically
if (typeof window !== undefined && window.Chart) {
    if (window.Chart.hasOwnProperty('register')) {
        window.Chart.register(pluginTrendlineLinear);
    } else {
        window.Chart.plugins.register(pluginTrendlineLinear);
    }
}

// Otherwise, try to export the plugin
try {
    module.exports = exports = pluginTrendlineLinear;
} catch (e) { }