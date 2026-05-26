using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Charts.Svg;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsSvgZoomPage
{
    private string lastEvent = "Wheel over a chart to zoom, or drag the plot area to pan.";

    private SvgChartViewport scatterViewport;

    private SvgChartViewport lineViewport;

    private SvgChartViewport columnViewport;

    private readonly SvgChartGridLinesOptions gridLines = new()
    {
        Visible = true,
        Opacity = 0.2,
    };

    private readonly List<PointSample> Points =
    [
        new() { X = 8, Y = 42, Radius = 5 },
        new() { X = 12, Y = 55, Radius = 6 },
        new() { X = 16, Y = 64, Radius = 9 },
        new() { X = 22, Y = 58, Radius = 7 },
        new() { X = 29, Y = 91, Radius = 12 },
        new() { X = 34, Y = 76, Radius = 10 },
        new() { X = 42, Y = 112, Radius = 14 },
        new() { X = 48, Y = 108, Radius = 11 },
        new() { X = 53, Y = 132, Radius = 13 },
        new() { X = 61, Y = 126, Radius = 12 },
    ];

    private readonly List<MonthlyRevenue> Revenue =
    [
        new() { Month = "Jan", Revenue = 68, Target = 72, Pipeline = 44 },
        new() { Month = "Feb", Revenue = 74, Target = 76, Pipeline = 52 },
        new() { Month = "Mar", Revenue = 91, Target = 80, Pipeline = 63 },
        new() { Month = "Apr", Revenue = 86, Target = 84, Pipeline = 58 },
        new() { Month = "May", Revenue = 103, Target = 88, Pipeline = 72 },
        new() { Month = "Jun", Revenue = 112, Target = 94, Pipeline = 84 },
        new() { Month = "Jul", Revenue = 98, Target = 98, Pipeline = 69 },
        new() { Month = "Aug", Revenue = 124, Target = 104, Pipeline = 91 },
        new() { Month = "Sep", Revenue = 119, Target = 108, Pipeline = 88 },
        new() { Month = "Oct", Revenue = 132, Target = 114, Pipeline = 104 },
        new() { Month = "Nov", Revenue = 141, Target = 120, Pipeline = 112 },
        new() { Month = "Dec", Revenue = 154, Target = 126, Pipeline = 128 },
    ];

    private SvgChartOptions scatterZoomOptions = new()
    {
        Height = 360,
        XAxis = new() { BeginAtZero = false, TickCount = 7, GridLines = new() { Visible = true, Opacity = 0.2 } },
        YAxis = new() { BeginAtZero = true, TickCount = 7 },
        Zoom = new() { Enabled = true, Mode = SvgChartZoomMode.XY, MaxZoom = 120 },
    };

    private SvgChartOptions lineZoomOptions = new()
    {
        Height = 360,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
        Zoom = new() { Enabled = true, Mode = SvgChartZoomMode.X, MaxZoom = 80 },
    };

    private SvgChartOptions columnZoomOptions = new()
    {
        Height = 360,
        Zoom = new() { Enabled = true, Mode = SvgChartZoomMode.X, MaxZoom = 80 },
    };

    private Task OnPointHovered( SvgChartPointEventArgs eventArgs )
    {
        lastEvent = $"Hovered {eventArgs.SeriesName} / {eventArgs.Category}: {eventArgs.Value}";

        return Task.CompletedTask;
    }

    private Task OnScatterViewportChanged( SvgChartViewport viewport )
    {
        scatterViewport = viewport;

        return Task.CompletedTask;
    }

    private Task OnLineViewportChanged( SvgChartViewport viewport )
    {
        lineViewport = viewport;

        return Task.CompletedTask;
    }

    private Task OnColumnViewportChanged( SvgChartViewport viewport )
    {
        columnViewport = viewport;

        return Task.CompletedTask;
    }

    private Task OnZoomed( SvgChartZoomedEventArgs eventArgs )
    {
        lastEvent = $"Zoomed viewport X {eventArgs.Viewport.XMin:0.##}-{eventArgs.Viewport.XMax:0.##}, Y {eventArgs.Viewport.YMin:0.##}-{eventArgs.Viewport.YMax:0.##}";

        return Task.CompletedTask;
    }

    private Task OnPanned( SvgChartPannedEventArgs eventArgs )
    {
        lastEvent = $"Panned viewport X {eventArgs.Viewport.XMin:0.##}-{eventArgs.Viewport.XMax:0.##}, Y {eventArgs.Viewport.YMin:0.##}-{eventArgs.Viewport.YMax:0.##}";

        return Task.CompletedTask;
    }

    private sealed class PointSample
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Radius { get; set; }
    }

    private sealed class MonthlyRevenue
    {
        public string Month { get; set; }

        public double Revenue { get; set; }

        public double Target { get; set; }

        public double Pipeline { get; set; }
    }
}