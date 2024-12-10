import { parseFunction, deepClone } from "./utilities.js?v=1.7.1.0";

// workaround for: https://github.com/Megabit/Blazorise/issues/2287
const _ChartTitleCallbacks = function (item) {
    return item[0].dataset.label;
};

const _ChartLabelCallback = function (item) {
    const label = item.label;
    const value = item.dataset.data[item.dataIndex];
    return label + ': ' + value;
};

// In Chart v3 callbacks are now defined by default. So to override them by the Blazorise the user
// would have to first set them to null immediately after charts.js is loaded for this workaround
// to have any effect.

if (!Chart.overrides.pie.plugins.tooltip.callbacks.title) {
    Chart.overrides.pie.plugins.tooltip.callbacks.title = _ChartTitleCallbacks;
}

if (!Chart.overrides.pie.plugins.tooltip.callbacks.label) {
    Chart.overrides.pie.plugins.tooltip.callbacks.label = _ChartLabelCallback;
}

if (!Chart.overrides.doughnut.plugins.tooltip.callbacks.title) {
    Chart.overrides.doughnut.plugins.tooltip.callbacks.title = _ChartTitleCallbacks;
}

if (!Chart.overrides.doughnut.plugins.tooltip.callbacks.label) {
    Chart.overrides.doughnut.plugins.tooltip.callbacks.label = _ChartLabelCallback;
}

const _instances = [];

export function initialize(dotnetAdapter, eventOptions, canvas, canvasId, type, data, options, dataJsonString, optionsJsonString, optionsObject, pluginNames) {
    if (dataJsonString) {
        data = JSON.parse(dataJsonString);
    }

    if (optionsJsonString) {
        options = JSON.parse(optionsJsonString);
    }
    else if (optionsObject) {
        options = optionsObject;
    }

    function processTooltipCallbacks(callbacks) {
        if (callbacks && typeof callbacks === 'object') {
            const newCallbacks = {};

            Object.keys(callbacks).forEach(key => {
                if (typeof callbacks[key] === 'string') {
                    newCallbacks[key] = eval(`(${callbacks[key]})`);
                } else {
                    newCallbacks[key] = callbacks[key];
                }
            });

            Object.assign(callbacks, newCallbacks);
        }
    }

    function processTicksCallback(scales, axis) {
        if (scales && Array.isArray(scales[axis])) {
            scales[axis].forEach(a => {
                if (a.ticks && a.ticks.callback) {
                    if (typeof a.ticks.callback === 'string') {
                        const callback = a.ticks.callback;
                        a.ticks.callback = function (value, index, ticks) {
                            return eval(callback);
                        }
                    }
                }
            });
        }
    }

    if (options && options.scales) {
        processTicksCallback(options.scales, 'x');
        processTicksCallback(options.scales, 'y');
    }

    if (options && options.plugins && options.plugins.tooltip && options.plugins.tooltip.callbacks) {
        processTooltipCallbacks(options.plugins.tooltip.callbacks);
    }

    // search for canvas element
    canvas = canvas || document.getElementById(canvasId);

    if (canvas) {
        const chart = createChart(dotnetAdapter, eventOptions, canvas, canvasId, type, data, options, pluginNames);

        // save references to all elements
        _instances[canvasId] = {
            dotnetAdapter: dotnetAdapter,
            eventOptions: eventOptions,
            canvas: canvas,
            chart: chart
        };
    }
}

export function changeChartType(canvas, canvasId, type) {
    let chart = getChart(canvasId);

    if (chart) {
        const data = chart.data;
        const options = deepClone(chart.options);
        const instance = _instances[canvasId];

        if (data && data.datasets) {
            data.datasets.forEach((ds) => {
                ds.type = type;
            });
        }

        chart.destroy();

        chart = createChart(instance.dotnetAdapter, instance.eventOptions, canvas, canvas, type, data, options, instance.pluginNames);

        _instances[canvasId].chart = chart;
    }
}

function createChart(dotnetAdapter, eventOptions, canvas, canvasId, type, data, options, pluginNames) {
    // save the copy of the received options
    const originalOptions = deepClone(options);

    options = compileOptionCallbacks(options);

    const plugins = [];

    if (pluginNames) {
        if (pluginNames.includes("DataLabels") && ChartDataLabels) {
            plugins.push(ChartDataLabels);
        }

        if (pluginNames.includes("Streaming")) {
            plugins.push({
                duration: 20000
            });
        }
    }

    const chart = new Chart(canvas, {
        type: type,
        data: data,
        options: options,
        plugins: plugins
    });

    chart.originalOptions = originalOptions;

    wireEvents(dotnetAdapter, eventOptions, canvas, type, chart);

    return chart;
}

export function compileOptionCallbacks(options) {
    if (options && options.scales) {
        if (options.scales.x && options.scales.x.ticks && options.scales.x.ticks.callback) {
            options.scales.x.ticks.callback = parseFunction(options.scales.x.ticks.callback);
        }

        if (options.scales.y && options.scales.y.ticks && options.scales.y.ticks.callback) {
            options.scales.y.ticks.callback = parseFunction(options.scales.y.ticks.callback);
        }
    }

    return options;
}

export function destroy(canvas, canvasId) {
    var instances = _instances || {};

    const instance = instances[canvasId];

    if (instance) {
        const chart = instance.chart;

        if (chart) {
            chart.destroy();
        }

        delete instances[canvasId];
    }
}

export function setOptions(canvasId, options, optionsJsonString, optionsObject) {
    if (optionsJsonString) {
        options = JSON.parse(optionsJsonString);
    }
    else if (optionsObject) {
        options = optionsObject;
    }

    const chart = getChart(canvasId);

    if (chart) {
        // save the copy of the received options
        const originalOptions = deepClone(options);

        options = compileOptionCallbacks(options);

        chart.options = options;
        chart.originalOptions = originalOptions;

        // Due to a bug in chartjs we need to set aspectRatio directly on chart instance
        // instead of through the options.
        if (options.aspectRatio) {
            chart._aspectRatio = options.aspectRatio;

            chart.resize();
        }
    }
}

export function resize(canvasId) {
    const chart = getChart(canvasId);

    if (chart) {
        chart.resize();
    }
}

export function update(canvasId) {
    const chart = getChart(canvasId);

    if (chart) {
        chart.update();
    }
}

export function clear(canvasId) {
    const chart = getChart(canvasId);

    if (chart) {
        chart.data.labels = [];
        chart.data.datasets = [];
        chart.update();
    }
}

export function addLabel(canvasId, newLabels) {
    const chart = getChart(canvasId);

    if (chart) {
        newLabels.forEach((label, index) => {
            chart.data.labels.push(label);
        });
    }
}

export function addDataset(canvasId, newDatasets) {
    const chart = getChart(canvasId);
    if (chart) {
        newDatasets.forEach((dataset, index) => {
            chart.data.datasets.push(dataset);
        });
    }
}

export function removeDataset(canvasId, datasetIndex) {
    const chart = getChart(canvasId);

    if (chart && datasetIndex >= 0) {
        chart.data.datasets.splice(datasetIndex, 1);
    }
}

export function addData(canvasId, datasetIndex, newData) {
    const chart = getChart(canvasId);

    if (chart) {
        newData.forEach((data, index) => {
            chart.data.datasets[datasetIndex].data.push(data);
        });
    }
}

export function setData(canvasId, datasetIndex, newData) {
    const chart = getChart(canvasId);

    if (chart) {
        chart.data.datasets[datasetIndex].data = newData;
    }
}

export function addDatasetsAndUpdate(canvasId, newDatasets) {
    const chart = getChart(canvasId);

    if (chart) {
        newDatasets.forEach((dataset, index) => {
            chart.data.datasets.push(dataset);
        });

        chart.update();
    }
}

export function addLabelsDatasetsAndUpdate(canvasId, newLabels, newDatasets) {
    const chart = getChart(canvasId);

    if (chart) {
        newLabels.forEach((label, index) => {
            chart.data.labels.push(label);
        });

        newDatasets.forEach((dataset, index) => {
            chart.data.datasets.push(dataset);
        });

        chart.update();
    }
}

export function shiftLabel(canvasId) {
    const chart = getChart(canvasId);

    if (chart) {
        chart.data.labels.shift();
    }
}

export function shiftData(canvasId, datasetIndex) {
    const chart = getChart(canvasId);

    if (chart) {
        chart.data.datasets[datasetIndex].data.shift();
    }
}

export function popLabel(canvasId) {
    const chart = getChart(canvasId);

    if (chart) {
        chart.data.labels.pop();
    }
}

export function popData(canvasId, datasetIndex) {
    const chart = getChart(canvasId);

    if (chart) {
        chart.data.datasets[datasetIndex].data.pop();
    }
}

function toModel(element, type) {
    if (type == "line") {
        return {
            x: element.x,
            y: element.y,
            label: element.options.label,
            backgroundColor: element.options.backgroundColor,
            borderColor: element.options.borderColor,
            borderWidth: element.options.borderWidth,
            controlPointNextX: element.cp1x,
            controlPointNextY: element.cp1y,
            controlPointPreviousX: element.cp2x,
            controlPointPreviousY: element.cp2y,
            hitRadius: element.options.hitRadius,
            pointStyle: element.options.pointStyle,
            radius: element.options.radius,
            skip: element.skip,
            stop: element.stop,
            steppedLine: element.options.steppedLine,
            tension: element.options.tension
        };
    }
    else if (type == "bar") {
        return {
            x: element.x,
            y: element.y,
            backgroundColor: element.options.backgroundColor,
            borderColor: element.options.borderColor,
            borderRadius: element.options.borderRadius,
            borderSkipped: element.options.borderSkipped,
            borderWidth: element.options.borderWidth,
            inflateAmount: element.options.inflateAmount,
            pointStyle: element.options.pointStyle,
            datasetLabel: element.options.datasetLabel,
            base: element.base,
            horizontal: element.horizontal,
            width: element.width,
            height: element.height
        };
    }
    else if (type == "pie" || type == "polarArea" || type == "doughnut") {
        return {
            x: element.x,
            y: element.y,

            backgroundColor: element.options.backgroundColor,
            borderAlign: element.options.borderAlign,
            borderColor: element.options.borderColor,
            borderRadius: element.options.borderRadius,
            borderWidth: element.options.borderWidth,
            offset: element.options.offset,
            spacing: element.options.spacing,
            circumference: element.circumference,
            startAngle: element.startAngle,
            endAngle: element.endAngle,
            innerRadius: element.innerRadius,
            outerRadius: element.outerRadius,
            pixelMargin: element.pixelMargin,
            fullCircles: element.fullCircles
        };
    }
    else if (type == "scatter" || type == "bubble") {
        return {
            x: element.x,
            y: element.y,

            backgroundColor: element.options.backgroundColor,
            borderColor: element.options.borderColor,
            borderWidth: element.options.borderWidth,
            hitRadius: element.options.hitRadius,
            hoverBorderWidth: element.options.hoverBorderWidth,
            hoverRadius: element.options.hoverRadius,
            pointStyle: element.options.pointStyle,
            radius: element.options.radius,
            rotation: element.options.rotation,
            skip: element.skip,
            stop: element.stop
        };
    }
    else if (type == "radar") {
        return {
            x: element.x,
            y: element.y,

            angle: element.angle,
            backgroundColor: element.options.backgroundColor,
            borderColor: element.options.borderColor,
            borderWidth: element.options.borderWidth,
            hitRadius: element.options.hitRadius,
            hoverBorderWidth: element.options.hoverBorderWidth,
            hoverRadius: element.options.hoverRadius,
            pointStyle: element.options.pointStyle,
            radius: element.options.radius,
            rotation: element.options.rotation,
            skip: element.skip,
            stop: element.stop
        };
    }

    return {
        x: element.x,
        y: element.y,
        backgroundColor: element.options.backgroundColor,
        borderColor: element.options.borderColor,
        borderWidth: element.options.borderWidth
    };
}

export function wireEvents(dotnetAdapter, eventOptions, canvas, type, chart) {
    if (eventOptions.hasClickEvent) {
        canvas.onclick = function (evt) {
            const activePoint = chart.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, false);

            if (activePoint && activePoint.length > 0) {
                const datasetIndex = activePoint[0].datasetIndex;
                const index = activePoint[0].index;
                const model = toModel(activePoint[0].element, type);
                model.datasetLabel = chart.data.labels[index];

                dotnetAdapter.invokeMethodAsync("Event", "click", datasetIndex, index, JSON.stringify(model));
            }
        };
    }

    if (eventOptions.hasHoverEvent) {
        chart.config.options.onHover = function (evt) {
            if (evt.type === "mousemove") {
                const activePoint = chart.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, false);

                if (activePoint && activePoint.length > 0) {
                    const datasetIndex = activePoint[0].datasetIndex;
                    const index = activePoint[0].index;
                    const model = toModel(activePoint[0].element, type);
                    model.datasetLabel = chart.data.labels[index];

                    dotnetAdapter.invokeMethodAsync("Event", "hover", datasetIndex, index, JSON.stringify(model));
                }
            }
            else if (evt.type === "mouseout") {
                dotnetAdapter.invokeMethodAsync("Event", "mouseout", -1, -1, "{}");
            }
        };
    }
}

export function getChart(canvasId) {
    let chart = null;

    Chart.helpers.each(Chart.instances, function (instance) {
        if (instance.canvas.id === canvasId) {
            chart = instance;
        }
    });

    return chart;
}