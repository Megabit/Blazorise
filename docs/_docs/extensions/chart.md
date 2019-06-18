---
title: "Chart extension"
permalink: /docs/extensions/chart/
excerpt: "Learn how to use chart extension components."
toc: true
toc_label: "Guide"
---

**Warning:** Right now there are some issues when serializing dataset object to json. Blazor internal serializer is serializing nullable fields and when ChartJS is trying to read them it will break. There is not much I can do for now, except to always inititalise all of the fields for the particular chart dataset.
{: .notice--warning}

**Update:** As of version **0.5.2** and **0.6.0-preview3** there are now two parameters for the chart components that will serve as a workaround for Blazor serializer which does not supports DataContract and DataMember attributes. The new parameters are `DataJsonString` and `OptionsJsonString` and are used to provide data and options for charts as json strings.
Keep in mind that these two parameters are just a temporary feature that will be removed once the Blazor team implements a better serializer.
{: .notice--info}

## Basics

The chart extension is defined of several different chart components. Each of the chart type have it's own dataset and option settings.

Supported charts types are:

- `Chart` default chart components, should be used only for testing(see warning)
- `LineChart`
- `BarChart`
- `PieChart`
- `PolarAreaChart`
- `DoughnutChart`
- `RadarChart`

## Installation

### Nuget

Install chart extension from nuget.

```
Install-Package Blazorise.Charts
```

### Index

Add ChartsJS to your index.html file.

```html
<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.min.js"></script>
```

### Imports

In your main _Imports.razor_ add:

```cs
@using Blazorise.Charts
```

## Usage

You should always define `TItem` data type.

```html
<SimpleButton Clicked="@(async () => await HandleRedraw())">Redraw</SimpleButton>

<LineChart ref="lineChart" TItem="double" />
```
```cs
@code{
    LineChart<double> lineChart;

    protected override async Task OnAfterRenderAsync()
    {
        await HandleRedraw();
    }

    async Task HandleRedraw()
    {
        lineChart.Clear();

        lineChart.AddLabel( Labels );

        lineChart.AddDataSet( GetLineChartDataset() );

        await lineChart.Update();
    }

    LineChartDataset<double> GetLineChartDataset()
    {
        return new LineChartDataset<double>
        {
            Label = "# of randoms",
            Data = RandomizeData(),
            BackgroundColor = backgroundColors,
            BorderColor = borderColors,
            Fill = true,
            PointRadius = 2,
            BorderDash = new List<int> { }
        };
    }

    string[] Labels = { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };
    List<string> backgroundColors = new List<string> { ChartColor.FromRgba( 255, 99, 132, 0.2f ), ChartColor.FromRgba( 54, 162, 235, 0.2f ), ChartColor.FromRgba( 255, 206, 86, 0.2f ), ChartColor.FromRgba( 75, 192, 192, 0.2f ), ChartColor.FromRgba( 153, 102, 255, 0.2f ), ChartColor.FromRgba( 255, 159, 64, 0.2f ) };
    List<string> borderColors = new List<string> { ChartColor.FromRgba( 255, 99, 132, 1f ), ChartColor.FromRgba( 54, 162, 235, 1f ), ChartColor.FromRgba( 255, 206, 86, 1f ), ChartColor.FromRgba( 75, 192, 192, 1f ), ChartColor.FromRgba( 153, 102, 255, 1f ), ChartColor.FromRgba( 255, 159, 64, 1f ) };

    List<double> RandomizeData()
    {
        var r = new Random( DateTime.Now.Millisecond );

        return new List<double> { r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble() };
    }
}
```