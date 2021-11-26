import { parseFunction, deepClone } from "./utilities.js";

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

export function initialize(dotnetAdapter, eventOptions, canvas, canvasId, type, data, options, dataJsonString, optionsJsonString, optionsObject) {
    if (dataJsonString) {
        data = JSON.parse(dataJsonString);
    }

    if (optionsJsonString) {
        options = JSON.parse(optionsJsonString);
    }
    else if (optionsObject) {
        options = optionsObject;
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

    // search for canvas element
    canvas = canvas || document.getElementById(canvasId);

    if (canvas) {
        const chart = createChart(dotnetAdapter, eventOptions, canvas, canvasId, type, data, options);

        // save references to all elements
        _instances[canvasId] = {
            dotNetRef: dotnetAdapter,
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

        if (data && data.datasets) {
            data.datasets.forEach((ds) => {
                ds.type = type;
            });
        }

        chart.destroy();

        chart = createChart(_instances[canvasId].dotnetAdapter, _instances[canvasId].eventOptions, canvas, canvas, type, data, options);

        _instances[canvasId].chart = chart;
    }
}

function createChart(dotnetAdapter, eventOptions, canvas, canvasId, type, data, options) {
    // save the copy of the received options
    const originalOptions = deepClone(options);

    options = compileOptionCallbacks(options);

    const chart = new Chart(canvas, {
        type: type,
        data: data,
        options: options
    });

    chart.originalOptions = originalOptions;

    wireEvents(dotnetAdapter, eventOptions, canvas, chart);

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

    const chart = instances[canvasId].chart;

    if (chart) {
        chart.destroy();
    }

    delete instances[canvasId];
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
            chart.aspectRatio = options.aspectRatio;
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

export function wireEvents(dotnetAdapter, eventOptions, canvas, chart) {
    if (eventOptions.hasClickEvent) {
        canvas.onclick = function (evt) {
            const activePoint = chart.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, false);

            if (activePoint && activePoint.length > 0) {
                const datasetIndex = activePoint[0]._datasetIndex;
                const index = activePoint[0]._index;
                const model = activePoint[0]._model;

                dotnetAdapter.invokeMethodAsync("Event", "click", datasetIndex, index, JSON.stringify(model));
            }
        };
    }

    if (eventOptions.hasHoverEvent) {
        chart.config.options.onHover = function (evt) {
            if (evt.type === "mousemove") {
                const activePoint = chart.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, false);

                if (activePoint && activePoint.length > 0) {
                    const datasetIndex = activePoint[0]._datasetIndex;
                    const index = activePoint[0]._index;
                    const model = activePoint[0]._model;

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
