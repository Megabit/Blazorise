window.blazoriseCharts = {
    _instances: [],

    initialize: (dotnetAdapter, canvasId, type, data, options, dataJsonString, optionsJsonString) => {
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

            window.blazoriseCharts.wireEvents(canvas, chart, dotnetAdapter);
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

    wireEvents: (canvas, chart, dotnetAdapter) => {
        canvas.onclick = function (evt) {
            var activePoints = chart.getElementsAtEvent(evt);

            for (var i = 0; i < activePoints.length; i++) {
                const datasetIndex = activePoints[i]["_datasetIndex"];
                const index = activePoints[i]["_index"];
                const model = activePoints[i]["_model"];

                dotnetAdapter.invokeMethodAsync('ModelClicked', datasetIndex, index, JSON.stringify(model));
            }
        };
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