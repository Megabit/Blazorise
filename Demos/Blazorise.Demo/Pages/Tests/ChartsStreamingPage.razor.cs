using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Charts;
using Blazorise.Charts.Streaming;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsStreamingPage : ComponentBase
{
    private LineChart<LiveDataPoint> horizontalLineChart;
    private LineChart<LiveDataPoint> verticalLineChart;

    private BarChart<LiveDataPoint> horizontalBarChart;
    private BarChart<LiveDataPoint> verticalBarChart;

    private ChartStreaming<LiveDataPoint> horizontalLineChartStreaming;

    private Random random = new( DateTime.Now.Millisecond );

    private string[] Labels = { "A", "B", "C", "D", "E", "F" };

    private static List<string> BackgroundColors = new()
    {
        ChartColor.FromRgba( 76, 110, 245, 0.25f ),   // Indigo
        ChartColor.FromRgba( 18, 184, 134, 0.25f ),   // Teal
        ChartColor.FromRgba( 245, 159, 0, 0.25f ),    // Amber
        ChartColor.FromRgba( 240, 62, 62, 0.25f ),    // Red
        ChartColor.FromRgba( 132, 94, 247, 0.25f ),   // Purple
        ChartColor.FromRgba( 34, 139, 230, 0.25f )    // Blue
    };

    private static List<string> BorderColors = new()
    {
        ChartColor.FromRgba( 76, 110, 245, 1f ),      // Indigo
        ChartColor.FromRgba( 18, 184, 134, 1f ),      // Teal
        ChartColor.FromRgba( 245, 159, 0, 1f ),       // Amber
        ChartColor.FromRgba( 240, 62, 62, 1f ),       // Red
        ChartColor.FromRgba( 132, 94, 247, 1f ),      // Purple
        ChartColor.FromRgba( 34, 139, 230, 1f )       // Blue
    };

    public struct LiveDataPoint
    {
        public object X { get; set; }

        public object Y { get; set; }
    }

    private object horizontalLineChartOptions = new
    {
        Scales = new
        {
            Y = new
            {
                Title = new
                {
                    Display = true,
                    Text = "Value"
                }
            }
        },
        Interaction = new
        {
            intersect = false
        }
    };

    private object verticalLineChartOptions = new
    {
        IndexAxis = "y",
        Scales = new
        {
            X = new
            {
                Type = "linear",
                Display = true,
                Title = new
                {
                    Display = true,
                    LabelString = "value"
                }
            }
        },
        Interaction = new
        {
            intersect = false
        }
    };

    private object horizontalBarChartOptions = new
    {
        Scales = new
        {
            Y = new
            {
                Title = new
                {
                    Display = true,
                    Text = "Value"
                }
            }
        },
        Interaction = new
        {
            intersect = false
        }
    };

    private object verticalBarChartOptions = new
    {
        IndexAxis = "y",
        Scales = new
        {
            X = new
            {
                Type = "linear",
                Display = true,
                Title = new
                {
                    Display = true,
                    Text = "Value"
                }
            }
        },
        Interaction = new
        {
            intersect = false
        }
    };

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await Task.WhenAll(
                HandleRedraw( horizontalLineChart, GetLineChartDataset1, GetLineChartDataset2 ),
                HandleRedraw( verticalLineChart, GetLineChartDataset1, GetLineChartDataset2 ),
                HandleRedraw( horizontalBarChart, GetBarChartDataset1 ),
                HandleRedraw( verticalBarChart, GetBarChartDataset2 )
            );
        }
    }

    private async Task HandleRedraw<TDataSet, TItem, TOptions, TModel>( BaseChart<TDataSet, TItem, TOptions, TModel> chart, params Func<TDataSet>[] getDataSets )
        where TDataSet : ChartDataset<TItem>
        where TOptions : ChartOptions
        where TModel : ChartModel
    {
        await chart.Clear();

        await chart.AddLabelsDatasetsAndUpdate( Labels, getDataSets.Select( x => x.Invoke() ).ToArray() );
    }

    private async Task AddNewHorizontalLineDataSet()
    {
        var colorIndex = horizontalLineChart.Data.Datasets.Count % BackgroundColors.Count;

        await horizontalLineChart.AddDatasetsAndUpdate( new LineChartDataset<LiveDataPoint>
        {
            Data = new(),
            Label = $"Dataset {horizontalLineChart.Data.Datasets.Count + 1}",
            BackgroundColor = BackgroundColors[colorIndex],
            BorderColor = BorderColors[colorIndex],
            Fill = false,
            Tension = 0,
        } );
    }

    private async Task AddNewHorizontalLineData()
    {
        foreach ( var dataset in horizontalLineChart.Data.Datasets )
        {
            await horizontalLineChart.AddData( horizontalLineChart.Data.Datasets.IndexOf( dataset ), new LiveDataPoint
            {
                X = DateTime.Now,
                Y = RandomScalingFactor(),
            } );
        }

        await horizontalLineChart.Update();
    }

    private async Task PauseHorizontalLineChart()
    {
        await horizontalLineChartStreaming.Pause();
    }

    private async Task PlayHorizontalLineChart()
    {
        await horizontalLineChartStreaming.Play();
    }

    private async Task AddNewVerticalLineDataSet()
    {
        var colorIndex = verticalLineChart.Data.Datasets.Count % BackgroundColors.Count;

        await verticalLineChart.AddDatasetsAndUpdate( new LineChartDataset<LiveDataPoint>
        {
            Data = new(),
            Label = $"Dataset {verticalLineChart.Data.Datasets.Count + 1}",
            BackgroundColor = BackgroundColors[colorIndex],
            BorderColor = BorderColors[colorIndex],
            Fill = false,
            Tension = 0,
        } );
    }

    private async Task AddNewVerticalLineData()
    {
        foreach ( var dataset in verticalLineChart.Data.Datasets )
        {
            await verticalLineChart.AddData( verticalLineChart.Data.Datasets.IndexOf( dataset ), new LiveDataPoint
            {
                X = RandomScalingFactor(),
                Y = DateTime.Now,
            } );
        }

        await verticalLineChart.Update();
    }

    private async Task AddNewHorizontalBarDataSet()
    {
        var colorIndex = horizontalBarChart.Data.Datasets.Count % BackgroundColors.Count;

        await horizontalBarChart.AddDatasetsAndUpdate( new BarChartDataset<LiveDataPoint>
        {
            Data = new(),
            Label = $"Dataset {horizontalBarChart.Data.Datasets.Count + 1}",
            BackgroundColor = BackgroundColors[colorIndex],
            BorderColor = BorderColors[colorIndex],
        } );
    }

    private async Task AddNewHorizontalBarData()
    {
        foreach ( var dataset in horizontalBarChart.Data.Datasets )
        {
            await horizontalBarChart.AddData( horizontalBarChart.Data.Datasets.IndexOf( dataset ), new LiveDataPoint
            {
                X = DateTime.Now,
                Y = RandomScalingFactor(),
            } );
        }

        await horizontalBarChart.Update();
    }

    private async Task AddNewVerticalBarDataSet()
    {
        var colorIndex = verticalBarChart.Data.Datasets.Count % BackgroundColors.Count;

        await verticalBarChart.AddDatasetsAndUpdate( new BarChartDataset<LiveDataPoint>
        {
            Data = new(),
            Label = $"Dataset {verticalBarChart.Data.Datasets.Count + 1}",
            BackgroundColor = BackgroundColors[colorIndex],
            BorderColor = BorderColors[colorIndex],
        } );
    }

    private async Task AddNewVerticalBarData()
    {
        foreach ( var dataset in verticalBarChart.Data.Datasets )
        {
            await verticalBarChart.AddData( verticalBarChart.Data.Datasets.IndexOf( dataset ), new LiveDataPoint
            {
                X = RandomScalingFactor(),
                Y = DateTime.Now,
            } );
        }

        await verticalBarChart.Update();
    }

    private LineChartDataset<LiveDataPoint> GetLineChartDataset1()
    {
        return new()
        {
            Data = new(),
            Label = "Dataset 1 (linear interpolation)",
            BackgroundColor = BackgroundColors[0],
            BorderColor = BorderColors[0],
            Fill = false,
            Tension = 0,
            BorderDash = new() { 8, 4 },
        };
    }

    private LineChartDataset<LiveDataPoint> GetLineChartDataset2()
    {
        return new()
        {
            Data = new(),
            Label = "Dataset 2 (cubic interpolation)",
            BackgroundColor = BackgroundColors[1],
            BorderColor = BorderColors[1],
            Fill = false,
            CubicInterpolationMode = "monotone",
        };
    }

    private BarChartDataset<LiveDataPoint> GetBarChartDataset1()
    {
        return new()
        {
            Data = new(),
            Label = "Dataset 1",
            BackgroundColor = BackgroundColors[0],
            BorderColor = BorderColors[0],
        };
    }

    private BarChartDataset<LiveDataPoint> GetBarChartDataset2()
    {
        return new()
        {
            Type = "bar",
            Data = new(),
            Label = "Dataset 1",
            BackgroundColor = BackgroundColors[0],
            BorderColor = BorderColors[0],
        };
    }

    private Task OnHorizontalLineRefreshed( ChartStreamingData<LiveDataPoint> data )
    {
        data.Value = new()
        {
            X = DateTime.Now,
            Y = RandomScalingFactor(),
        };

        return Task.CompletedTask;
    }

    private Task OnVerticalLineRefreshed( ChartStreamingData<LiveDataPoint> data )
    {
        data.Value = new()
        {
            X = RandomScalingFactor(),
            Y = DateTime.Now,
        };

        return Task.CompletedTask;
    }

    private Task OnHorizontalBarRefreshed( ChartStreamingData<LiveDataPoint> data )
    {
        data.Value = new()
        {
            X = DateTime.Now,
            Y = RandomScalingFactor(),
        };

        return Task.CompletedTask;
    }

    private Task OnVerticalBarRefreshed( ChartStreamingData<LiveDataPoint> data )
    {
        data.Value = new()
        {
            X = RandomScalingFactor(),
            Y = DateTime.Now,
        };

        return Task.CompletedTask;
    }

    private double RandomScalingFactor()
    {
        return ( random.NextDouble() > 0.5 ? 1.0 : -1.0 ) * Math.Round( random.NextDouble() * 100 );
    }
}