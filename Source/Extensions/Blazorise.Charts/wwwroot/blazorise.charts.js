window.blazoriseCharts = {
    _instances: [],

    initialize: (dotnetAdapter, hasClickEvent, hasHoverEvent, canvasId, type, data, options, dataJsonString, optionsJsonString) => {
        if (dataJsonString)
            data = JSON.parse(dataJsonString);

        if (optionsJsonString)
            options = JSON.parse(optionsJsonString);

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

    update: (canvasId, data, options, dataJsonString, optionsJsonString) => {
        if (dataJsonString)
            data = JSON.parse(dataJsonString);

        if (optionsJsonString)
            options = JSON.parse(optionsJsonString);

        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.config.data = data;
            chart.config.options = options;
            chart.update();
        }

        return true;
    },

    wireEvents: (dotnetAdapter, hasClickEvent, hasHoverEvent, canvas, chart) => {
        if (hasClickEvent) {
            canvas.onclick = function (evt) {
                var elemetn = chart.getElementsAtEvent(evt);

                for (var i = 0; i < elemetn.length; i++) {
                    const datasetIndex = elemetn[i]["_datasetIndex"];
                    const index = elemetn[i]["_index"];
                    const model = elemetn[i]["_model"];

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