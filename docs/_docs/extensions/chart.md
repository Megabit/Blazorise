---
title: "Chart extension"
permalink: /docs/extensions/chart/
excerpt: "Learn how to use chart extension components."
toc: true
toc_label: "Guide"
---

**Warning:** Right now there are some issues when serializing dataset object to JSON. Blazor internal serializer is serializing nullable fields and when ChartJS is trying to read them it will break. There is not much I can do for now, except to always initialize all of the fields for the particular chart dataset.
{: .notice--warning}

**Update:** As of version **0.5.2** and **0.6.0-preview3** there are now two parameters for the chart components that will serve as a workaround for Blazor serializer which does not supports DataContract and DataMember attributes. The new parameters are `DataJsonString` and `OptionsJsonString` and are used to provide data and options for charts as JSON strings.
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

### NuGet

Install chart extension from NuGet.

```
Install-Package Blazorise.Charts
```

### Index

Add `ChartsJS` and `blazorise.charts.js` to your index.html file.

```html
<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.min.js"></script>

<script src="_content/Blazorise.Charts/blazorise.charts.js"></script>
```

### Imports

In your main _Imports.razor_ add:

```cs
@using Blazorise.Charts
```

## Usage

You should always define `TItem` data type.

```html
<Button Clicked="@(async () => await HandleRedraw())">Redraw</Button>

<LineChart @ref="lineChart" TItem="double" />
```
```cs
@code{
    LineChart<double> lineChart;

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await HandleRedraw();
        }
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

### Events

It is possible to use `Clicked` and `Hovered` events to interact with chart. The usage is pretty straighforward. The only thing to keep in mind is the `Model` field that needs to be casted to the right chart-model type. The available model types are:

- `LineChartModel`
- `BarChartModel`
- `DoughnutChartModel`
- `PieChartModel`
- `PolarChartModel`
- `RadarChartModel`

```html
<Chart @ref="barChart" Type="ChartType.Bar" TItem="double" Clicked="@OnClicked" />
@code{
    void OnClicked(ChartMouseEventArgs e)
    {
        var model = e.Model as BarChartModel;

        Console.WriteLine($"{model.X}-{model.Y}");
    }
}
```

## Attributes

| Name               | Type                                                                       | Default      | Description                                                                           |
|--------------------|----------------------------------------------------------------------------|--------------|---------------------------------------------------------------------------------------|
| Type               | [ChartType]({{ "/docs/helpers/enums/#charttype" | relative_url }})         | `Line`       | Defines the chart type.                                                               |
| Data               | ChartData                                                                  |              | Defines the chart data.                                                               |
| Options            | ChartOptions                                                               |              | Defines the chart options.                                                            |
| DataJsonString     | string                                                                     | null         | Defines the chart data that is serialized as JSON string. **[WILL BE REMOVED]**       |
| OptionsJsonString  | string                                                                     | null         | Defines the chart options that is serialized as JSON string. **[WILL BE REMOVED]**    |
| Clicked            | event                                                                      |              | Raised when clicked on data point.                                                    |
| Hovered            | event                                                                      |              | Raised when hovered over data point.                                                  |


**Note:** DataJsonString and OptionsJsonString are used only temporary until the Blazor team fixes the built-in JSON serializer.
{: .notice--info}