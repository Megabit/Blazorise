using System.Collections.Generic;
using Blazorise.Charts.Svg;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsSvgTrendlinesPage
{
    private readonly List<MonthlySales> Sales =
    [
        new() { Month = "Jan", Revenue = 82, North = 43, South = 71, ActiveUsers = 64 },
        new() { Month = "Feb", Revenue = 88, North = 48, South = 66, ActiveUsers = 70 },
        new() { Month = "Mar", Revenue = 93, North = 52, South = 68, ActiveUsers = 73 },
        new() { Month = "Apr", Revenue = 91, North = 49, South = 59, ActiveUsers = 77 },
        new() { Month = "May", Revenue = 104, North = 56, South = 54, ActiveUsers = 84 },
        new() { Month = "Jun", Revenue = 112, North = 64, South = 51, ActiveUsers = 92 },
        new() { Month = "Jul", Revenue = 118, North = 67, South = 45, ActiveUsers = 101 },
    ];

    private readonly List<StoreSample> Stores =
    [
        new() { Traffic = 8, Sales = 42 },
        new() { Traffic = 13, Sales = 48 },
        new() { Traffic = 18, Sales = 51 },
        new() { Traffic = 22, Sales = 64 },
        new() { Traffic = 27, Sales = 62 },
        new() { Traffic = 33, Sales = 74 },
        new() { Traffic = 38, Sales = 83 },
        new() { Traffic = 45, Sales = 88 },
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
        Trendlines =
        [
            new()
            {
                SeriesName = "North",
                Color = Color.Danger,
                StrokeWidth = 2,
                DashPattern = "8 4",
            },
            new()
            {
                SeriesName = "South",
                Color = Color.Warning,
                StrokeWidth = 2,
                DashPattern = "4 4",
            },
        ],
    };

    private SvgChartOptions scatterOptions = new()
    {
        Height = 360,
        XAxis = new()
        {
            GridLines = new() { Visible = true },
        },
        YAxis = new()
        {
            BeginAtZero = true,
            TickCount = 6,
        },
    };

    private SvgChartOptions areaOptions = new()
    {
        Height = 360,
    };

    private sealed class MonthlySales
    {
        public string Month { get; set; }

        public double Revenue { get; set; }

        public double North { get; set; }

        public double South { get; set; }

        public double ActiveUsers { get; set; }
    }

    private sealed class StoreSample
    {
        public double Traffic { get; set; }

        public double Sales { get; set; }
    }
}