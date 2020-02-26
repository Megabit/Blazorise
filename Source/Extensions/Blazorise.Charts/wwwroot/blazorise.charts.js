window.blazoriseCharts = {
    _instances: [],

    initialize: (dotnetAdapter, hasClickEvent, hasHoverEvent, canvasId, type, data, options, dataJsonString, optionsJsonString, optionsObject) => {
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

            window.blazoriseCharts.wireEvents(dotnetAdapter, hasClickEvent, hasHoverEvent, canvas, chart);
        }

        return true;
    },

    destroy: (canvasId) => {
        var instances = window.blazoriseCharts._instances || {};

        const chart = instances[canvasId].chart;

        if (chart) {
            chart.destroy();
        }

        delete instances[canvasId];

        return true;
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
        }

        return true;
    },

    update: (canvasId) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.update();
        }

        return true;
    },

    clear: (canvasId) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.data.labels = [];
            chart.data.datasets = [];
            chart.update();
        }

        return true;
    },

    addLabel: (canvasId, newLabels) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newLabels.forEach((label, index) => {
                chart.data.labels.push(label);
            });
        }

        return true;
    },

    addDataset: (canvasId, newDatasets) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newDatasets.forEach((dataset, index) => {
                chart.data.datasets.push(dataset);
            });
        }

        return true;
    },

    addData: (canvasId, datasetIndex, newData) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            newData.forEach((data, index) => {
                chart.data.datasets[datasetIndex].data.push(data);
            });
        }

        return true;
    },

    wireEvents: (dotnetAdapter, hasClickEvent, hasHoverEvent, canvas, chart) => {
        if (hasClickEvent) {
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

        if (hasHoverEvent) {
            chart.config.options.onHover = function (evt) {
                var element = chart.getElementsAtEvent(evt);

                for (var i = 0; i < element.length; i++) {
                    const datasetIndex = element[i]["_datasetIndex"];
                    const index = element[i]["_index"];
                    const model = element[i]["_model"];

                    dotnetAdapter.invokeMethodAsync("Event", "hover", datasetIndex, index, JSON.stringify(model));
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
