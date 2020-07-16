window.blazoriseCharts = {
    _instances: [],

    initialize: function (dotnetAdapter, hasClickEvent, hasHoverEvent, canvasId, type, data, options, dataJsonString, optionsJsonString, optionsObject) {
        if (dataJsonString) {
            data = JSON.parse(dataJsonString);
        }

        if (optionsJsonString) {
            options = JSON.parse(optionsJsonString);
        }
        else if (optionsObject) {
            options = optionsObject;
        }

        // search for canvas element
        var canvas = document.getElementById(canvasId);

        if (canvas) {
            var chart = new Chart(canvas, {
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

            window.blazoriseCharts.wireEvents(dotnetAdapter, hasClickEvent, hasHoverEvent, canvas, chart);
        }
    },

    destroy: function (canvasId) {
        var instances = window.blazoriseCharts._instances || {};

        var chart = instances[canvasId].chart;

        if (chart) {
            chart.destroy();
        }

        delete instances[canvasId];
    },

    setOptions: function (canvasId, options, optionsJsonString, optionsObject) {
        if (optionsJsonString) {
            options = JSON.parse(optionsJsonString);
        }
        else if (optionsObject) {
            options = optionsObject;
        }

        var chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.options = options;
        }
    },

    update: function (canvasId) {
        var chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.update();
        }
    },

    clear: function (canvasId) {
        var chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.data.labels = [];
            chart.data.datasets = [];
            chart.update();
        }
    },

    addLabel: function (canvasId, newLabels) {
        var chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newLabels.forEach(function (label, index) {
                chart.data.labels.push(label);
            });
        }
    },

    addDataset: function (canvasId, newDatasets) {
        var chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newDatasets.forEach(function (dataset, index) {
                chart.data.datasets.push(dataset);
            });
        }
    },

    addData: function (canvasId, datasetIndex, newData) {
        var chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newData.forEach(function (data, index) {
                chart.data.datasets[datasetIndex].data.push(data);
            });
        }
    },

    addDatasetsAndUpdate: function (canvasId, newDatasets) {
        var chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newDatasets.forEach(function (dataset, index) {
                chart.data.datasets.push(dataset);
            });

            chart.update();
        }
    },

    addLabelsDatasetsAndUpdate: function (canvasId, newLabels, newDatasets) {
        var chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newLabels.forEach(function (label, index) {
                chart.data.labels.push(label);
            });

            newDatasets.forEach(function (dataset, index) {
                chart.data.datasets.push(dataset);
            });

            chart.update();
        }
    },

    wireEvents: function (dotnetAdapter, hasClickEvent, hasHoverEvent, canvas, chart) {
        if (hasClickEvent) {
            canvas.onclick = function (evt) {
                var element = chart.getElementsAtEvent(evt);

                for (var i = 0; i < element.length; i++) {
                    var datasetIndex = element[i]["_datasetIndex"];
                    var index = element[i]["_index"];
                    var model = element[i]["_model"];

                    dotnetAdapter.invokeMethodAsync("Event", "click", datasetIndex, index, JSON.stringify(model));
                }
            };
        }

        if (hasHoverEvent) {
            chart.config.options.onHover = function (evt) {
                var element = chart.getElementsAtEvent(evt);

                for (var i = 0; i < element.length; i++) {
                    var datasetIndex = element[i]["_datasetIndex"];
                    var index = element[i]["_index"];
                    var model = element[i]["_model"];

                    dotnetAdapter.invokeMethodAsync("Event", "hover", datasetIndex, index, JSON.stringify(model));
                }
            };
        }
    },

    getChart: function (canvasId) {
        var chart = null;

        Chart.helpers.each(Chart.instances, function (instance) {
            if (instance.chart.canvas.id === canvasId) {
                chart = instance.chart;
            }
        });

        return chart;
    }
};
