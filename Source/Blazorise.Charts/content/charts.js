window.blazoriseCharts = {
    setChartData: function (id, type, data, options) {
        //console.log(JSON.stringify(data));
        //console.log(JSON.stringify(options));

        const element = document.getElementById(id);

        if (element) {
            new Chart(element, {
                type: type,
                data: data,
                options: options
            });
        }

        return true;
    }
};