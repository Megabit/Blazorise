window.blazoriseCharts = {
    setChartData: (canvasId, type, data, options, dataJsonString, optionsJsonString) => {
        if (dataJsonString)
            data = JSON.parse(dataJsonString);

        if (optionsJsonString)
            options = JSON.parse(optionsJsonString);

        //console.log(type);
        //console.log(JSON.stringify(data));
        //console.log(JSON.stringify(options));

        const chart = blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.config.data = data;
            chart.config.options = options;
            chart.update();
        } else {
            const canvas = document.getElementById(canvasId);

            if (canvas) {
                new Chart(canvas, {
                    type: type,
                    data: data,
                    options: options
                });
            }
        }

        return true;
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