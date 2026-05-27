using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Charts.Svg;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsSvgDataLabelsPage
{
    private string lastEvent = "Interact with a chart point or data label to see event details.";

    private readonly List<MonthlySales> Sales =
    [
        new() { Month = "Jan", Tokyo = 49.9, NewYork = 83.6, London = 48.9 },
        new() { Month = "Feb", Tokyo = 71.5, NewYork = 78.8, London = 38.8 },
        new() { Month = "Mar", Tokyo = 106.4, NewYork = 98.5, London = 39.3 },
        new() { Month = "Apr", Tokyo = 129.2, NewYork = 93.4, London = 42.4 },
    ];

    private readonly List<PointSample> Points =
    [
        new() { X = 8, Y = 42 },
        new() { X = 16, Y = 64 },
        new() { X = 22, Y = 58 },
        new() { X = 29, Y = 91 },
        new() { X = 34, Y = 76 },
        new() { X = 42, Y = 112 },
    ];

    private SvgChartOptions columnOptions = new()
    {
        Height = 380,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
    };

    private SvgChartOptions lineOptions = new()
    {
        Height = 380,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
    };

    private SvgChartOptions doughnutOptions = new()
    {
        Height = 340,
    };

    private SvgChartOptions scatterOptions = new()
    {
        Height = 360,
        YAxis = new() { BeginAtZero = true, TickCount = 6 },
    };

    private SvgChartData<double?> shareData = new()
    {
        Labels = ["Desktop", "Mobile", "Tablet", "Other"],
        Series =
        [
            new()
            {
                Name = "Share",
                Values = [45, 32, 16, 7],
            },
        ]
    };

    private Task OnPointClicked( SvgChartPointEventArgs eventArgs )
    {
        lastEvent = $"Clicked point {eventArgs.SeriesName} / {eventArgs.Category}: {eventArgs.Value}";

        return Task.CompletedTask;
    }

    private Task OnPointHovered( SvgChartPointEventArgs eventArgs )
    {
        lastEvent = $"Hovered point {eventArgs.SeriesName} / {eventArgs.Category}: {eventArgs.Value}";

        return Task.CompletedTask;
    }

    private Task OnDataLabelClicked( SvgChartPointEventArgs eventArgs )
    {
        lastEvent = $"Clicked label {eventArgs.SeriesName} / {eventArgs.Category}: {eventArgs.Value}";

        return Task.CompletedTask;
    }

    private Task OnDataLabelHovered( SvgChartPointEventArgs eventArgs )
    {
        lastEvent = $"Hovered label {eventArgs.SeriesName} / {eventArgs.Category}: {eventArgs.Value}";

        return Task.CompletedTask;
    }

    private string FormatRevenueLabel( SvgChartDataLabelContext context )
    {
        return $"{Convert.ToDouble( context.Value, CultureInfo.InvariantCulture ).ToString( "0", CultureInfo.InvariantCulture )}k";
    }

    private string FormatTemperatureLabel( SvgChartDataLabelContext context )
    {
        return Convert.ToDouble( context.Value, CultureInfo.InvariantCulture ).ToString( "0", CultureInfo.InvariantCulture );
    }

    private string FormatShareLabel( SvgChartDataLabelContext context )
    {
        double total = 0d;

        foreach ( double? value in shareData.Series[0].Values )
            total += value ?? 0;

        double current = Convert.ToDouble( context.Value, CultureInfo.InvariantCulture );

        return total <= 0 ? string.Empty : $"{current / total:P0}";
    }

    private string FormatStoreLabel( SvgChartDataLabelContext context )
    {
        return $"S{context.PointIndex + 1}";
    }

    private sealed class MonthlySales
    {
        public string Month { get; set; }

        public double Tokyo { get; set; }

        public double NewYork { get; set; }

        public double London { get; set; }
    }

    private sealed class PointSample
    {
        public double X { get; set; }

        public double Y { get; set; }
    }
}