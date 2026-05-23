using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Charts.Svg;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsSvgPage
{
    private readonly Random random = new( DateTime.Now.Millisecond );

    private SvgColumnChart<object> imperativeChart;

    private string lastEvent = "Interact with a chart point to see event details.";

    private readonly List<MonthlySales> Sales =
    [
        new() { Month = "Jan", Tokyo = 49.9, NewYork = 83.6, London = 48.9 },
        new() { Month = "Feb", Tokyo = 71.5, NewYork = 78.8, London = 38.8 },
        new() { Month = "Mar", Tokyo = 106.4, NewYork = 98.5, London = 39.3 },
        new() { Month = "Apr", Tokyo = 129.2, NewYork = 93.4, London = 42.4 },
    ];

    private readonly List<PointSample> Points =
    [
        new() { X = 8, Y = 42, Radius = 5 },
        new() { X = 16, Y = 64, Radius = 9 },
        new() { X = 22, Y = 58, Radius = 7 },
        new() { X = 29, Y = 91, Radius = 12 },
        new() { X = 34, Y = 76, Radius = 10 },
        new() { X = 42, Y = 112, Radius = 14 },
    ];

    private readonly List<RevenueGrowthSample> RevenueGrowth =
    [
        new() { Month = "Jan", Revenue = 68, Growth = 8.4 },
        new() { Month = "Feb", Revenue = 74, Growth = 11.2 },
        new() { Month = "Mar", Revenue = 91, Growth = 16.7 },
        new() { Month = "Apr", Revenue = 86, Growth = 13.9 },
        new() { Month = "May", Revenue = 103, Growth = 20.4 },
    ];

    private readonly List<LatencySample> Latency =
    [
        new() { Timestamp = new DateTime( 2026, 5, 23, 9, 0, 0 ), Api = 42, Queue = 28 },
        new() { Timestamp = new DateTime( 2026, 5, 23, 9, 5, 0 ), Api = 48, Queue = 31 },
        new() { Timestamp = new DateTime( 2026, 5, 23, 9, 10, 0 ), Api = 55, Queue = 36 },
        new() { Timestamp = new DateTime( 2026, 5, 23, 9, 15, 0 ), Api = 51, Queue = 33 },
        new() { Timestamp = new DateTime( 2026, 5, 23, 9, 20, 0 ), Api = 64, Queue = 42 },
    ];

    private SvgChartOptions columnOptions = new()
    {
        Height = 380,
        Title = "Monthly revenue",
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
    };

    private SvgChartOptions lineOptions = new()
    {
        Height = 380,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
    };

    private SvgChartOptions barOptions = new()
    {
        Height = 380,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
    };

    private SvgChartOptions multiAxisOptions = new()
    {
        Height = 380,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
    };

    private SvgChartOptions areaOptions = new()
    {
        Height = 380,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
    };

    private SvgChartOptions stackedOptions = new()
    {
        Height = 380,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
        Tooltip = new() { InteractionMode = SvgChartInteractionMode.Index, Width = 220 },
        YAxis = new() { BeginAtZero = true, Stacked = true, TickCount = 6 },
    };

    private SvgChartOptions timeAxisOptions = new()
    {
        Height = 380,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
        Tooltip = new() { InteractionMode = SvgChartInteractionMode.Dataset, Width = 220 },
        YAxis = new() { BeginAtZero = true, TickCount = 6 },
    };

    private SvgChartOptions pieOptions = new()
    {
        Height = 320,
    };

    private SvgChartOptions doughnutOptions = new()
    {
        Height = 320,
    };

    private SvgChartOptions polarAreaOptions = new()
    {
        Height = 320,
    };

    private SvgChartOptions radarOptions = new()
    {
        Height = 360,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
        YAxis = new() { BeginAtZero = true, Max = 100 },
    };

    private SvgChartOptions scatterOptions = new()
    {
        Height = 360,
        YAxis = new() { BeginAtZero = true, TickCount = 6 },
    };

    private SvgChartOptions bubbleOptions = new()
    {
        Height = 360,
        YAxis = new() { BeginAtZero = true, TickCount = 6 },
    };

    private SvgChartOptions imperativeOptions = new()
    {
        Height = 380,
        Title = "Dynamic revenue",
        Subtitle = "Updated through @ref methods",
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
    };

    private SvgChartData<double?> imperativeData = new()
    {
        Labels = ["Jan", "Feb", "Mar"],
        Series =
        [
            new()
            {
                Name = "Tokyo",
                Color = Color.Primary,
                Values = [49.9, 71.5, 106.4],
            },
            new()
            {
                Name = "New York",
                Color = Color.Info,
                Values = [83.6, 78.8, 98.5],
            },
            new()
            {
                Name = "London",
                Color = Color.Success,
                Values = [48.9, 38.8, 39.3],
            },
        ]
    };

    private SvgChartData<double?> radialData = new()
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

    private SvgChartData<double?> radarData = new()
    {
        Labels = ["Quality", "Speed", "Cost", "Support", "Adoption"],
        Series =
        [
            new()
            {
                Name = "Current",
                Color = Color.Primary,
                Values = [82, 76, 58, 88, 72],
            },
            new()
            {
                Name = "Target",
                Color = Color.Success,
                Values = [92, 86, 70, 94, 84],
            },
        ]
    };

    private async Task AddMonth()
    {
        string month = $"M{imperativeData.Labels.Count + 1}";

        await imperativeChart.AddLabel( month );
        await imperativeChart.AddValue( "Tokyo", NextValue( 70, 140 ) );
        await imperativeChart.AddValue( "New York", NextValue( 60, 120 ) );
        await imperativeChart.AddValue( "London", NextValue( 30, 80 ) );
        await imperativeChart.Update();
    }

    private async Task ToggleLondon()
    {
        await imperativeChart.ToggleSeries( "London" );
    }

    private async Task ClearImperative()
    {
        await imperativeChart.Clear();
    }

    private Task OnPointClicked( SvgChartPointEventArgs eventArgs )
    {
        lastEvent = $"Clicked {eventArgs.SeriesName} / {eventArgs.Category}: {eventArgs.Value}";

        return Task.CompletedTask;
    }

    private Task OnPointHovered( SvgChartPointEventArgs eventArgs )
    {
        lastEvent = $"Hovered {eventArgs.SeriesName} / {eventArgs.Category}: {eventArgs.Value}";

        return Task.CompletedTask;
    }

    private double NextValue( int min, int max )
    {
        return Math.Round( min + random.NextDouble() * ( max - min ), 1 );
    }

    private static string FormatCurrencyTick( SvgChartAxisTickContext context )
    {
        return context.Value is IFormattable value ? $"${value.ToString( "0", null )}" : context.Value?.ToString();
    }

    private static string FormatMillisecondsTick( SvgChartAxisTickContext context )
    {
        return context.Value is IFormattable value ? $"{value.ToString( "0", null )} ms" : context.Value?.ToString();
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

        public double Radius { get; set; }
    }

    private sealed class RevenueGrowthSample
    {
        public string Month { get; set; }

        public double Revenue { get; set; }

        public double Growth { get; set; }
    }

    private sealed class LatencySample
    {
        public DateTime Timestamp { get; set; }

        public double Api { get; set; }

        public double Queue { get; set; }
    }
}