window.blazoriseCharts = {
    setChartData: function (id, type, data, options, dataIsString, optionsIsString) {
        if (dataIsString)
            data = JSON.parse(data);

        if (optionsIsString)
            options = JSON.parse(options);

        //console.log(type);
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