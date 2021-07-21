// workaround for: https://github.com/Megabit/Blazorise/issues/2287
const _ChartTitleCallbacks = function (item, data) {
    return data.datasets[item[0].datasetIndex].label;
};

const _ChartLabelCallback = function (item, data) {
    const label = data.labels[item.index];
    const value = data.datasets[item.datasetIndex].data[item.index];
    return label + ': ' + value;
};

Chart.defaults.pie.tooltips.callbacks.title = _ChartTitleCallbacks;
Chart.defaults.pie.tooltips.callbacks.label = _ChartLabelCallback;
Chart.defaults.doughnut.tooltips.callbacks.title = _ChartTitleCallbacks;
Chart.defaults.doughnut.tooltips.callbacks.label = _ChartLabelCallback;

window.blazoriseCharts = {
    _instances: [],

    initialize: (dotnetAdapter, eventOptions, canvasId, type, data, options, dataJsonString, optionsJsonString, optionsObject) => {
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
                    if (a.ticks && a.ticks.callbackJavaScript) {
                        a.ticks.callback = function (value, index, ticks) {
                            return eval(a.ticks.callbackJavaScript)
                        }
                    }
                });
            }
        }

        if (options && options.scales) {
            processTicksCallback(options.scales, 'xAxes');
            processTicksCallback(options.scales, 'yAxes');
        }

        // search for canvas element
        const canvas = document.getElementById(canvasId);

        if (canvas) {
            let chart = new Chart(canvas, {
                type: type,
                data: data,
                options: options
            });

            // save references to all elements
            window.blazoriseCharts._instances[canvasId] = {
                dotNetRef: dotnetAdapter,
                canvas: canvas,
                chart: chart
            };

            window.blazoriseCharts.wireEvents(dotnetAdapter, eventOptions, canvas, chart);
        }
    },

    destroy: (canvasId) => {
        var instances = window.blazoriseCharts._instances || {};

        const chart = instances[canvasId].chart;

        if (chart) {
            chart.destroy();
        }

        delete instances[canvasId];
    },

    setOptions: (canvasId, options, optionsJsonString, optionsObject) => {
        if (optionsJsonString) {
            options = JSON.parse(optionsJsonString);
        }
        else if (optionsObject) {
            options = optionsObject;
        }

        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.options = options;

            // Due to a bug in chartjs we need to set aspectRatio directly on chart instance
            // instead of through the options.
            if (options.aspectRatio) {
                chart.aspectRatio = options.aspectRatio;
            }
        }
    },
    resize: (canvasId) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.resize();
        }
    },
    update: (canvasId) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.update();
        }
    },

    clear: (canvasId) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.data.labels = [];
            chart.data.datasets = [];
            chart.update();
        }
    },

    addLabel: (canvasId, newLabels) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newLabels.forEach((label, index) => {
                chart.data.labels.push(label);
            });
        }
    },

    addDataset: (canvasId, newDatasets) => {
        const chart = window.blazoriseCharts.getChart(canvasId);
        if (chart) {
            newDatasets.forEach((dataset, index) => {
                chart.data.datasets.push(dataset);
            });
        }
    },

    removeDataset: (canvasId, datasetIndex) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart && datasetIndex >= 0) {
            chart.data.datasets.splice(datasetIndex, 1);
        }
    },

    addData: (canvasId, datasetIndex, newData) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newData.forEach((data, index) => {
                chart.data.datasets[datasetIndex].data.push(data);
            });
        }
    },

    setData: (canvasId, datasetIndex, newData) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.data.datasets[datasetIndex].data = newData;
        }
    },

    addDatasetsAndUpdate: (canvasId, newDatasets) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newDatasets.forEach((dataset, index) => {
                chart.data.datasets.push(dataset);
            });

            chart.update();
        }
    },

    addLabelsDatasetsAndUpdate: (canvasId, newLabels, newDatasets) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newLabels.forEach((label, index) => {
                chart.data.labels.push(label);
            });

            newDatasets.forEach((dataset, index) => {
                chart.data.datasets.push(dataset);
            });

            chart.update();
        }
    },

    shiftLabel: (canvasId) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.data.labels.shift();
        }
    },

    shiftData: (canvasId, datasetIndex) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.data.datasets[datasetIndex].data.shift();
        }
    },

    popLabel: (canvasId) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.data.labels.pop();
        }
    },

    popData: (canvasId, datasetIndex) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.data.datasets[datasetIndex].data.pop();
        }
    },

    wireEvents: (dotnetAdapter, eventOptions, canvas, chart) => {
        if (eventOptions.hasClickEvent) {
            canvas.onclick = function (evt) {
                var element = chart.getElementsAtEvent(evt);

                for (var i = 0; i < element.length; i++) {
                    const datasetIndex = element[i]["_datasetIndex"];
                    const index = element[i]["_index"];
                    const model = element[i]["_model"];

                    dotnetAdapter.invokeMethodAsync("Event", "click", datasetIndex, index, JSON.stringify(model));
                }
            };
        }

        if (eventOptions.hasHoverEvent) {
            chart.config.options.onHover = function (evt) {
                var element = chart.getElementsAtEvent(evt);

                if (evt.type === "mousemove") {
                    for (var i = 0; i < element.length; i++) {
                        const datasetIndex = element[i]["_datasetIndex"];
                        const index = element[i]["_index"];
                        const model = element[i]["_model"];

                        dotnetAdapter.invokeMethodAsync("Event", "hover", datasetIndex, index, JSON.stringify(model));
                    }
                }
                else if (evt.type === "mouseout") {
                    dotnetAdapter.invokeMethodAsync("Event", "mouseout", -1, -1, "{}");
                }
            };
        }
    },

    getChart: (canvasId) => {
        let chart = null;

        Chart.helpers.each(Chart.instances, function (instance) {
            if (instance.chart.canvas.id === canvasId) {
                chart = instance.chart;
            }
        });

        return chart;
    }
};
