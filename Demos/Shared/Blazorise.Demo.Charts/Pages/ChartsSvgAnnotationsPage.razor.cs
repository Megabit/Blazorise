using System.Collections.Generic;
using Blazorise.Charts.Svg;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsSvgAnnotationsPage
{
    private readonly List<MonthlySample> Samples =
    [
        new() { Month = "Jan", Uptime = 73, Revenue = 88, ActiveUsers = 58 },
        new() { Month = "Feb", Uptime = 78, Revenue = 92, ActiveUsers = 64 },
        new() { Month = "Mar", Uptime = 84, Revenue = 96, ActiveUsers = 71 },
        new() { Month = "Apr", Uptime = 82, Revenue = 102, ActiveUsers = 76 },
        new() { Month = "May", Uptime = 88, Revenue = 108, ActiveUsers = 83 },
        new() { Month = "Jun", Uptime = 91, Revenue = 116, ActiveUsers = 89 },
        new() { Month = "Jul", Uptime = 86, Revenue = 111, ActiveUsers = 85 },
        new() { Month = "Aug", Uptime = 93, Revenue = 124, ActiveUsers = 96 },
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

    private readonly SvgChartAnnotationLabelOptions thresholdLabel = new()
    {
        Visible = true,
        Text = "Target",
        Position = SvgChartAnnotationLabelPosition.End,
        BackgroundColor = "#ffffff",
        Border = new() { Color = Color.Danger, Width = 1, Radius = 3 },
    };

    private readonly SvgChartAnnotationLabelOptions releaseLabel = new()
    {
        Visible = true,
        Text = "Release",
        Position = SvgChartAnnotationLabelPosition.Start,
        BackgroundColor = "#ffffff",
        Border = new() { Color = Color.Warning, Width = 1, Radius = 3 },
    };

    private readonly SvgChartAnnotationLabelOptions targetLabel = new()
    {
        Visible = true,
        Text = "Target area",
        Position = SvgChartAnnotationLabelPosition.Start,
        BackgroundColor = "#ffffff",
        Border = new() { Color = Color.Success, Width = 1, Radius = 3 },
    };

    private readonly SvgChartAnnotationLabelOptions noteLabel = new()
    {
        Visible = true,
        Text = "Stabilized",
        Position = SvgChartAnnotationLabelPosition.Center,
        BackgroundColor = "#ffffff",
        Border = new() { Color = Color.Primary, Width = 1, Radius = 3 },
    };

    private readonly SvgChartAnnotationLabelOptions pointLabel = new()
    {
        Visible = true,
        Text = "Selected store",
        Position = SvgChartAnnotationLabelPosition.End,
        BackgroundColor = "#ffffff",
        Border = new() { Color = Color.Warning, Width = 1, Radius = 3 },
    };

    private SvgChartOptions thresholdOptions = new()
    {
        Height = 380,
        Legend = new() { Visible = false },
    };

    private SvgChartOptions rangeOptions = new()
    {
        Height = 380,
        Legend = new() { Visible = false },
        Annotations =
        [
            new SvgChartBoxAnnotationOptions
            {
                XMin = -0.5,
                XMax = 1.5,
                BackgroundColor = Color.Warning,
                Opacity = 0.12,
                Label = new()
                {
                    Visible = true,
                    Text = "Q1",
                    Position = SvgChartAnnotationLabelPosition.Start,
                    BackgroundColor = "#ffffff",
                },
            },
            new SvgChartBoxAnnotationOptions
            {
                XMin = 1.5,
                XMax = 3.5,
                BackgroundColor = Color.Info,
                Opacity = 0.1,
                Label = new()
                {
                    Visible = true,
                    Text = "Q2",
                    Position = SvgChartAnnotationLabelPosition.Start,
                    BackgroundColor = "#ffffff",
                },
            },
            new SvgChartBoxAnnotationOptions
            {
                XMin = 3.5,
                XMax = 5.5,
                BackgroundColor = Color.Success,
                Opacity = 0.1,
                Label = new()
                {
                    Visible = true,
                    Text = "Q3",
                    Position = SvgChartAnnotationLabelPosition.Start,
                    BackgroundColor = "#ffffff",
                },
            },
            new SvgChartBoxAnnotationOptions
            {
                XMin = 5.5,
                XMax = 7.5,
                BackgroundColor = Color.Secondary,
                Opacity = 0.1,
                Label = new()
                {
                    Visible = true,
                    Text = "Q4",
                    Position = SvgChartAnnotationLabelPosition.Start,
                    BackgroundColor = "#ffffff",
                },
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

    private SvgChartOptions bandOptions = new()
    {
        Height = 360,
        Legend = new() { Visible = false },
        Annotations =
        [
            new SvgChartBoxAnnotationOptions
            {
                YMin = 70,
                YMax = 90,
                BackgroundColor = Color.Success,
                Opacity = 0.12,
                Label = new()
                {
                    Visible = true,
                    Text = "Operating band",
                    Position = SvgChartAnnotationLabelPosition.Start,
                    BackgroundColor = "#ffffff",
                },
            },
        ],
    };

    private sealed class MonthlySample
    {
        public string Month { get; set; }

        public double Uptime { get; set; }

        public double Revenue { get; set; }

        public double ActiveUsers { get; set; }
    }

    private sealed class StoreSample
    {
        public double Traffic { get; set; }

        public double Sales { get; set; }
    }
}