window.blazoriseChartsStreaming = {
    initialize: (dotNetAdapter, canvasId, vertical, streamOptions) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            const scalesOptions = window.blazoriseChartsStreaming.getStreamingOptions(dotNetAdapter, vertical, chart.options, streamOptions);

            // merge all options
            chart.options = { ...chart.options, ...scalesOptions };

            chart.update();
        }

        return true;
    },

    addData: (canvasId, datasetIndex, newData) => {
        const chart = window.blazoriseCharts.getChart(canvasId);

        if (chart) {
            chart.data.datasets[datasetIndex].data.push(newData);
        }

        return true;
    },

    getStreamingOptions: (dotNetAdapter, vertical, chartOptions, streamOptions) => {
        const axeOptions = {
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
            let verticalScalesOptions = {
                scales: {
                    yAxes: [axeOptions]
                }
            };

            // this is needed so that any additional axes option can be merged
            if (chartOptions && chartOptions.scales && chartOptions.scales.xAxes) {
                verticalScalesOptions.scales.xAxes = chartOptions.scales.xAxes;
            }

            return verticalScalesOptions;
        }

        let horizontalScalesOptions = {
            scales: {
                xAxes: [axeOptions],
                yAxes: chartOptions.scales.yAxes,
            }
        };

        // this is needed so that any additional axes option can be merged
        if (chartOptions && chartOptions.scales && chartOptions.scales.yAxes) {
            horizontalScalesOptions.scales.yAxes = chartOptions.scales.yAxes;
        }

        return horizontalScalesOptions;
    }
};
