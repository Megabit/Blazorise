window.blazoriseChartsStreaming = {
    initialize: (dotNetAdapter, canvasId, vertical, streamOptions) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            const options = Object.assign({},
                chart.options,
                window.blazoriseChartsStreaming.getStreamingOptions(dotNetAdapter, vertical, streamOptions));

            chart.options = options;
            chart.update();
        }

        return true;
    },

    updateData: (canvasId, datasetIndex, newDataSet) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.data.datasets[datasetIndex].data.push(newDataSet.data[0]);
        }

        return true;
    },

    getStreamingOptions: (dotNetAdapter, vertical, streamOptions) => {
        const options = {
            type: "realtime",
            realtime: {
                duration: streamOptions.duration,
                refresh: streamOptions.refresh,
                delay: streamOptions.delay,
                frameRate: streamOptions.frameRate,
                onRefresh: function (chart) {
                    dotNetAdapter.invokeMethodAsync("Refresh")
                        .catch((reason) => {
                            console.error(reason);
                        });
                }
            }
        };

        if (vertical) {
            return {
                scales: {
                    yAxes: [options]
                }
            };
        }

        return {
            scales: {
                xAxes: [options]
            }
        };
    }
};
