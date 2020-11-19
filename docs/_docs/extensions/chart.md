---
title: "Chart extension"
permalink: /docs/extensions/chart/
excerpt: "Learn how to use chart extension components."
toc: true
toc_label: "Guide"
---

**Warning:** Right now there are some issues when serializing dataset and option objects to JSON. Blazor internal serializer is serializing nullable fields and when ChartJS is trying to read them it will break. There is not much I can do for now, except to always initialize all of the fields for the particular chart dataset.
To overcome this limitation you can use `DataJsonString` and `OptionsJsonString` or `OptionsObject`. Keep in mind that these parameters are just a temporary feature that will be removed once the .Net Core team implements a better serializer.
{: .notice--warning}

## Basics

The chart extension is defined of several different chart components. Each of the chart type have it's own dataset and option settings.

Supported charts types are:

- `Chart` default chart components, should be used only for testing(see warning)
- `LineChart`
- `BarChart`
- `HorizontalBarChart`
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
<script src="https://cdn.jsdelivr.net/npm/chart.js@2.8.0"></script>

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
        await lineChart.Clear();

        await lineChart.AddLabelsDatasetsAndUpdate( Labels, GetLineChartDataset() );
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

It is possible to use `Clicked` and `Hovered` events to interact with chart. The usage is pretty straightforward. The only thing to keep in mind is the `Model` field that needs to be casted to the right chart-model type. The available model types are:

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

## Third-party Plugins

### Streaming

Live streaming is made possible with the help of [chartjs-plugin-streaming](https://nagix.github.io/chartjs-plugin-streaming/).

#### Installation

To use live streaming charts you need to first install it from NuGet:

```
Install-Package Blazorise.Charts.Streaming
```

The next step is to add necessary files to the _index.html_ or __Host.cshtml_ file.

```html
<script src="https://cdn.jsdelivr.net/npm/moment@2.24.0/min/moment.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/chart.js@2.8.0"></script>
<script src="https://cdn.jsdelivr.net/npm/chartjs-plugin-streaming@1.8.0"></script>

<script src="_content/Blazorise.Charts/blazorise.charts.js"></script>
<script src="_content/Blazorise.Charts.Streaming/blazorise.charts.streaming.js"></script>
```

#### Usage

```html
<LineChart @ref="horizontalLineChart" TItem="LiveDataPoint" OptionsObject="@horizontalLineChartOptions">
    <ChartStreaming TItem="LiveDataPoint"
                    Options="new ChartStreamingOptions { Delay = 2000 }"
                    Refreshed="@OnHorizontalLineRefreshed" />
</LineChart>
```

```cs
@code{
    LineChart<LiveDataPoint> horizontalLineChart;

    Random random = new Random( DateTime.Now.Millisecond );

    public struct LiveDataPoint
    {
        public object X { get; set; }

        public object Y { get; set; }
    }

    object horizontalLineChartOptions = new
    {
        Title = new
        {
            Display = true,
            Text = "Line chart (horizontal scroll) sample"
        },
        Scales = new
        {
            YAxes = new object[]
            {
                new {
                    ScaleLabel = new {
                    Display = true, LabelString = "value" }
                }
            }
        },
        Tooltips = new
        {
            Mode = "nearest",
            Intersect = false
        },
        Hover = new
        {
            Mode = "nearest",
            Intersect = false
        }
    };

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await Task.WhenAll(
                HandleRedraw( horizontalLineChart, GetLineChartDataset1 ) );
        }
    }

    async Task HandleRedraw<TDataSet, TItem, TOptions, TModel>( BaseChart<TDataSet, TItem, TOptions, TModel> chart, params Func<TDataSet>[] getDataSets )
        where TDataSet : ChartDataset<TItem>
        where TOptions : ChartOptions
        where TModel : ChartModel
    {
        await chart.Clear();

        await chart.AddLabelsDatasetsAndUpdate( Labels, getDataSets.Select( x => x.Invoke() ).ToArray() );
    }

    LineChartDataset<LiveDataPoint> GetLineChartDataset1()
    {
        return new LineChartDataset<LiveDataPoint>
        {
            Data = new List<LiveDataPoint>(),
            Label = "Dataset 1 (linear interpolation)",
            BackgroundColor = backgroundColors[0],
            BorderColor = borderColors[0],
            Fill = false,
            LineTension = 0,
            BorderDash = new List<int> { 8, 4 },
        };
    }

    Task OnHorizontalLineRefreshed( ChartStreamingData<LiveDataPoint> data )
    {
        data.Value = new LiveDataPoint
        {
            X = DateTime.Now,
            Y = RandomScalingFactor(),
        };

        return Task.CompletedTask;
    }

    double RandomScalingFactor()
    {
        return ( random.NextDouble() > 0.5 ? 1.0 : -1.0 ) * Math.Round( random.NextDouble() * 100 );
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

## ChartOptions Attributes

| Name                | Type                                                                       | Default     | Description                                                                           |
|---------------------|----------------------------------------------------------------------------|-------------|---------------------------------------------------------------------------------------|
| Scales              | Scales                                                                     |             |                                                                                       |
| Legend              | Legend                                                                     |             |                                                                                       |
| Tooltips            | Tooltips                                                                   |             |                                                                                       |
| Animation           | Animation                                                                  |             |                                                                                       |
| Responsive          | bool?                                                                      | `true`      | Resizes the chart canvas when its container does.                                     |
| MaintainAspectRatio | bool?                                                                      | `true`      | Maintain the original canvas aspect ratio (width / height) when resizing.             |
| ResponsiveAnimationDuration | bool?                                                              | `true`      | Duration in milliseconds it takes to animate to new size after a resize event.        |
| AspectRatio         | int                                                                        | `2`         | Canvas aspect ratio (i.e. width / height, a value of 1 representing a square canvas). |

## Scales Attributes

| Name               | Type                                                                       | Default      | Description                                                                           |
|--------------------|----------------------------------------------------------------------------|--------------|---------------------------------------------------------------------------------------|
| XAxes              | List of Axis                                                               |              |                                                                                       |
| YAxes              | List of Axis                                                               |              |                                                                                       |

## Legend Attributes

| Name               | Type                                                                       | Default      | Description                                                                           |
|--------------------|----------------------------------------------------------------------------|--------------|---------------------------------------------------------------------------------------|
| Display            | bool                                                                       | `true`       | Is the legend shown.                                                                  |
| Reverse            | bool                                                                       | `false`      | Legend will show datasets in reverse order.                                           |
| FullWidth          | bool                                                                       | `true`       | Marks that this box should take the full width of the canvas.                         |

## Tooltips Attributes

| Name               | Type                                                                       | Default      | Description                                                                           |
|--------------------|----------------------------------------------------------------------------|--------------|---------------------------------------------------------------------------------------|
| Display            | bool                                                                       | `true`       | Are on-canvas tooltips enabled.                                                       |

## Axis Attributes

| Name               | Type                                                                       | Default      | Description                                                                           |
|--------------------|----------------------------------------------------------------------------|--------------|---------------------------------------------------------------------------------------|
| Type               | string                                                                     | null         |                                                                                       |
| Display            | bool                                                                       | `true`       |                                                                                       |
| Ticks              | AxeTicks                                                                   |              |                                                                                       |
| GridLines          | GridLines                                                                  |              |                                                                                       |
| ScaleLabel         | ScaleLabel                                                                 |              |                                                                                       |
| Ticks              | AxeTicks                                                                   |              |                                                                                       |
| Stacked            | bool                                                                       | `false`      | Only used for `BarChart` and settng this to true will stack the datasets              |