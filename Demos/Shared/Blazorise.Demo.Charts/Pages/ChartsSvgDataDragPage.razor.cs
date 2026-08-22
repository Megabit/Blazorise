#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Charts.Svg;
#endregion

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsSvgDataDragPage
{
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

    private readonly List<AllocationSample> Allocations =
    [
        new() { Team = "Platform", Allocation = 70 },
        new() { Team = "Commerce", Allocation = 55 },
        new() { Team = "Mobile", Allocation = 80 },
        new() { Team = "Support", Allocation = 45 },
    ];

    private readonly List<WorkloadSample> Workloads =
    [
        new() { Team = "Platform", Committed = 65, Planned = 35 },
        new() { Team = "Commerce", Committed = 50, Planned = 45 },
        new() { Team = "Mobile", Committed = 75, Planned = 30 },
        new() { Team = "Support", Committed = 40, Planned = 50 },
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

    private readonly SvgChartData<double?> pieData = new()
    {
        Labels = ["Search", "Social", "Email", "Direct"],
        Series =
        [
            new()
            {
                Name = "Traffic",
                Values = [42, 28, 18, 12],
            },
        ]
    };

    private readonly SvgChartData<double?> doughnutData = new()
    {
        Labels = ["Engineering", "Marketing", "Sales", "Operations"],
        Series =
        [
            new()
            {
                Name = "Budget",
                Values = [38, 22, 25, 15],
            },
        ]
    };

    private readonly SvgChartData<double?> polarAreaData = new()
    {
        Labels = ["Organic", "Paid", "Referral", "Partner", "Direct"],
        Series =
        [
            new()
            {
                Name = "Strength",
                Values = [72, 55, 64, 48, 80],
            },
        ]
    };

    private readonly SvgChartData<double?> radarData = new()
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
                Draggable = false,
            },
        ]
    };

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

    private readonly SvgChartOptions barOptions = new()
    {
        Height = 360,
        Legend = new() { Visible = false },
        YAxis = new() { Min = 0, Max = 100, TickCount = 6 },
    };

    private readonly SvgChartOptions stackedBarOptions = new()
    {
        Height = 360,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
        YAxis = new() { Min = 0, Max = 160, Stacked = true, TickCount = 5 },
    };

    private readonly SvgChartOptions columnOptions = new()
    {
        Height = 360,
        Legend = new() { Visible = false },
        YAxis = new() { Min = 0, Max = 100, TickCount = 6 },
    };

    private readonly SvgChartOptions stackedAreaOptions = new()
    {
        Height = 360,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
        YAxis = new() { Min = 0, Max = 160, Stacked = true, TickCount = 5 },
    };

    private readonly SvgChartOptions radialOptions = new()
    {
        Height = 360,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
        YAxis = new() { Min = 0, Max = 100, TickCount = 6 },
    };

    private readonly SvgChartOptions radarOptions = new()
    {
        Height = 360,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
        YAxis = new() { Min = 0, Max = 100, TickCount = 6 },
    };

    private Task OnForecastDragEnded( SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled && eventArgs.YValue.HasValue )
            Forecasts[eventArgs.PointIndex].Forecast = eventArgs.YValue.Value;

        return Task.CompletedTask;
    }

    private Task OnCapacityDragEnded( SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled && eventArgs.YValue.HasValue )
            Capacity[eventArgs.PointIndex].Utilization = eventArgs.YValue.Value;

        return Task.CompletedTask;
    }

    private Task OnAllocationDragEnded( SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled && eventArgs.XValue.HasValue )
            Allocations[eventArgs.PointIndex].Allocation = eventArgs.XValue.Value;

        return Task.CompletedTask;
    }

    private Task OnWorkloadDragEnded( SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled && eventArgs.XValue.HasValue )
        {
            if ( eventArgs.SeriesIndex == 0 )
                Workloads[eventArgs.PointIndex].Committed = eventArgs.XValue.Value;
            else if ( eventArgs.SeriesIndex == 1 )
                Workloads[eventArgs.PointIndex].Planned = eventArgs.XValue.Value;
        }

        return Task.CompletedTask;
    }

    private Task OnAllocationColumnDragEnded( SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled && eventArgs.YValue.HasValue )
            Allocations[eventArgs.PointIndex].Allocation = eventArgs.YValue.Value;

        return Task.CompletedTask;
    }

    private Task OnWorkloadAreaDragEnded( SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled && eventArgs.YValue.HasValue )
        {
            if ( eventArgs.SeriesIndex == 0 )
                Workloads[eventArgs.PointIndex].Committed = eventArgs.YValue.Value;
            else if ( eventArgs.SeriesIndex == 1 )
                Workloads[eventArgs.PointIndex].Planned = eventArgs.YValue.Value;
        }

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

        return Task.CompletedTask;
    }

    private Task OnOpportunityDragEnded( SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled && eventArgs.XValue.HasValue )
            Opportunities[eventArgs.PointIndex].Effort = eventArgs.XValue.Value;

        return Task.CompletedTask;
    }

    private static Task OnRadialDragEnded( SvgChartData<double?> data, SvgChartDataPointDragEventArgs eventArgs )
    {
        if ( !eventArgs.Canceled
             && eventArgs.YValue.HasValue
             && eventArgs.SeriesIndex >= 0
             && eventArgs.SeriesIndex < data.Series.Count
             && eventArgs.PointIndex >= 0
             && eventArgs.PointIndex < data.Series[eventArgs.SeriesIndex].Values.Count )
        {
            data.Series[eventArgs.SeriesIndex].Values[eventArgs.PointIndex] = eventArgs.YValue.Value;
        }

        return Task.CompletedTask;
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

    private sealed class AllocationSample
    {
        public string Team { get; set; }

        public double Allocation { get; set; }
    }

    private sealed class WorkloadSample
    {
        public string Team { get; set; }

        public double Committed { get; set; }

        public double Planned { get; set; }
    }

    private sealed class BubbleSample
    {
        public double Effort { get; set; }

        public double Impact { get; set; }

        public double Size { get; set; }
    }
}