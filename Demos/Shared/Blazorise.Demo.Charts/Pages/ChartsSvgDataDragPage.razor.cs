#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Charts.Svg;
#endregion

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsSvgDataDragPage
{
    private string lastEvent = "Drag a chart point to edit its value.";

    private readonly List<ForecastSample> Forecasts =
    [
        new() { Month = "Jan", Baseline = 42, Forecast = 48 },
        new() { Month = "Feb", Baseline = 46, Forecast = 54 },
        new() { Month = "Mar", Baseline = 49, Forecast = 61 },
        new() { Month = "Apr", Baseline = 53, Forecast = 66 },
        new() { Month = "May", Baseline = 57, Forecast = 72 },
        new() { Month = "Jun", Baseline = 60, Forecast = 76 },
    ];

    private readonly List<CapacitySample> Capacity =
    [
        new() { Quarter = "Q1 2025", Utilization = 54 },
        new() { Quarter = "Q2 2025", Utilization = 61 },
        new() { Quarter = "Q3 2025", Utilization = 68 },
        new() { Quarter = "Q4 2025", Utilization = 74 },
        new() { Quarter = "Q1 2026", Utilization = 79 },
        new() { Quarter = "Q2 2026", Utilization = 84 },
    ];

    private readonly List<PointSample> Stores =
    [
        new() { X = 15, Y = 35 },
        new() { X = 30, Y = 55 },
        new() { X = 45, Y = 40 },
        new() { X = 60, Y = 75 },
        new() { X = 80, Y = 65 },
    ];

    private readonly List<BubbleSample> Opportunities =
    [
        new() { Effort = 15, Impact = 45, Size = 7 },
        new() { Effort = 30, Impact = 72, Size = 11 },
        new() { Effort = 50, Impact = 58, Size = 9 },
        new() { Effort = 70, Impact = 84, Size = 14 },
        new() { Effort = 85, Impact = 36, Size = 8 },
    ];

    private readonly SvgChartOptions lineOptions = new()
    {
        Height = 360,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
        YAxis = new() { Min = 0, Max = 100, TickCount = 6 },
    };

    private readonly SvgChartOptions areaOptions = new()
    {
        Height = 360,
        Legend = new() { Visible = false },
        YAxis = new() { Min = 0, Max = 100, TickCount = 6 },
    };

    private readonly SvgChartOptions scatterOptions = new()
    {
        Height = 360,
        Legend = new() { Visible = false },
        XAxis = new() { Min = 0, Max = 100, TickCount = 6, GridLines = new() { Visible = true, Opacity = 0.2 } },
        YAxis = new() { Min = 0, Max = 100, TickCount = 6 },
    };

    private readonly SvgChartOptions bubbleOptions = new()
    {
        Height = 360,
        Legend = new() { Visible = false },
        XAxis = new() { Min = 0, Max = 100, TickCount = 6, GridLines = new() { Visible = true, Opacity = 0.2 } },
        YAxis = new() { Min = 0, Max = 100, TickCount = 6 },
    };

    private Task OnDragging( SvgChartDataPointDragEventArgs eventArgs )
    {
        lastEvent = $"Dragging {eventArgs.SeriesName} point {eventArgs.PointIndex + 1}: X {FormatValue( eventArgs.XValue )}, Y {FormatValue( eventArgs.YValue )}";

        return Task.CompletedTask;
    }

    private Task OnForecastDragEnded( SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled && eventArgs.YValue.HasValue )
            Forecasts[eventArgs.PointIndex].Forecast = eventArgs.YValue.Value;

        UpdateEndedEvent( eventArgs );

        return Task.CompletedTask;
    }

    private Task OnCapacityDragEnded( SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled && eventArgs.YValue.HasValue )
            Capacity[eventArgs.PointIndex].Utilization = eventArgs.YValue.Value;

        UpdateEndedEvent( eventArgs );

        return Task.CompletedTask;
    }

    private Task OnStoreDragEnded( SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled )
        {
            if ( eventArgs.XValue.HasValue )
                Stores[eventArgs.PointIndex].X = eventArgs.XValue.Value;

            if ( eventArgs.YValue.HasValue )
                Stores[eventArgs.PointIndex].Y = eventArgs.YValue.Value;
        }

        UpdateEndedEvent( eventArgs );

        return Task.CompletedTask;
    }

    private Task OnOpportunityDragEnded( SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled && eventArgs.XValue.HasValue )
            Opportunities[eventArgs.PointIndex].Effort = eventArgs.XValue.Value;

        UpdateEndedEvent( eventArgs );

        return Task.CompletedTask;
    }

    private void UpdateEndedEvent( SvgChartDataPointDragEventArgs eventArgs )
    {
        lastEvent = eventArgs.Canceled
            ? $"Canceled {eventArgs.SeriesName} point {eventArgs.PointIndex + 1}; original values restored."
            : $"Saved {eventArgs.SeriesName} point {eventArgs.PointIndex + 1}: X {FormatValue( eventArgs.XValue )}, Y {FormatValue( eventArgs.YValue )}";
    }

    private static string FormatValue( double? value )
    {
        return value?.ToString( "0.##" ) ?? "n/a";
    }

    private sealed class ForecastSample
    {
        public string Month { get; set; }

        public double Baseline { get; set; }

        public double Forecast { get; set; }
    }

    private sealed class CapacitySample
    {
        public string Quarter { get; set; }

        public double Utilization { get; set; }
    }

    private sealed class PointSample
    {
        public double X { get; set; }

        public double Y { get; set; }
    }

    private sealed class BubbleSample
    {
        public double Effort { get; set; }

        public double Impact { get; set; }

        public double Size { get; set; }
    }
}