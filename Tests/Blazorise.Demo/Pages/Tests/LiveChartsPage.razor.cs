using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Charts;
using Blazorise.Charts.Streaming;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Pages.Tests
{
    public partial class LiveChartsPage : ComponentBase
    {
        LineChart<LiveDataPoint> horizontalLineChart;
        LineChart<LiveDataPoint> verticalLineChart;

        BarChart<LiveDataPoint> horizontalBarChart;
        HorizontalBarChart<LiveDataPoint> verticalBarChart;

        Random random = new Random( DateTime.Now.Millisecond );

        string[] Labels = { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };
        List<string> backgroundColors = new List<string> { ChartColor.FromRgba( 255, 99, 132, 0.5f ), ChartColor.FromRgba( 54, 162, 235, 0.5f ), ChartColor.FromRgba( 255, 206, 86, 0.5f ), ChartColor.FromRgba( 75, 192, 192, 0.5f ), ChartColor.FromRgba( 153, 102, 255, 0.5f ), ChartColor.FromRgba( 255, 159, 64, 0.5f ) };
        List<string> borderColors = new List<string> { ChartColor.FromRgba( 255, 99, 132, 1f ), ChartColor.FromRgba( 54, 162, 235, 1f ), ChartColor.FromRgba( 255, 206, 86, 1f ), ChartColor.FromRgba( 75, 192, 192, 1f ), ChartColor.FromRgba( 153, 102, 255, 1f ), ChartColor.FromRgba( 255, 159, 64, 1f ) };

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

        object verticalLineChartOptions = new
        {
            Title = new
            {
                Display = true,
                Text = "Line chart (vertical scroll) sample"
            },
            Scales = new
            {
                XAxes = new object[]
                {
                    new {
                        Type = "linear",
                        Display = true,
                        ScaleLabel = new {
                            Display = true, LabelString = "value"
                        }
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

        object horizontalBarChartOptions = new
        {
            Title = new
            {
                Display = true,
                Text = "Bar chart (horizontal scroll) sample"
            },
            Scales = new
            {
                YAxes = new object[]
                {
                    new
                    {
                        ScaleLabel = new
                        {
                            Display = true, LabelString = "value"
                        }
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

        object verticalBarChartOptions = new
        {
            Title = new
            {
                Display = true,
                Text = "Bar chart (vertical scroll) sample"
            },
            Scales = new
            {
                XAxes = new object[]
                {
                    new {
                        ScaleLabel = new
                        {
                            Display = true, LabelString = "value"
                        }
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
                    HandleRedraw( horizontalLineChart, GetLineChartDataset1, GetLineChartDataset2 ),
                    HandleRedraw( verticalLineChart, GetLineChartDataset1, GetLineChartDataset2 ),
                    HandleRedraw( horizontalBarChart, GetBarChartDataset1 ),
                    HandleRedraw( verticalBarChart, GetBarChartDataset2 ) );
            }
        }

        async Task HandleRedraw<TDataSet, TItem, TOptions, TModel>( BaseChart<TDataSet, TItem, TOptions, TModel> chart, params Func<TDataSet>[] getDataSets )
            where TDataSet : ChartDataset<TItem>
            where TOptions : ChartOptions
            where TModel : ChartModel
        {
            chart.Clear();

            chart.AddLabel( Labels );

            foreach ( var getDataSet in getDataSets )
            {
                chart.AddDataSet( getDataSet() );
            }

            await chart.Update();
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

        LineChartDataset<LiveDataPoint> GetLineChartDataset2()
        {
            return new LineChartDataset<LiveDataPoint>
            {
                Data = new List<LiveDataPoint>(),
                Label = "Dataset 2 (cubic interpolation)",
                BackgroundColor = backgroundColors[1],
                BorderColor = borderColors[1],
                Fill = false,
                CubicInterpolationMode = "monotone",
            };
        }

        BarChartDataset<LiveDataPoint> GetBarChartDataset1()
        {
            return new BarChartDataset<LiveDataPoint>
            {
                Data = new List<LiveDataPoint>(),
                Label = "Dataset 1",
                BackgroundColor = backgroundColors[0],
                BorderColor = borderColors[0],
            };
        }

        BarChartDataset<LiveDataPoint> GetBarChartDataset2()
        {
            return new BarChartDataset<LiveDataPoint>
            {
                Data = new List<LiveDataPoint>(),
                Label = "Dataset 2",
                BackgroundColor = backgroundColors[0],
                BorderColor = borderColors[0],
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

        Task OnVerticalLineRefreshed( ChartStreamingData<LiveDataPoint> data )
        {
            data.Value = new LiveDataPoint
            {
                X = RandomScalingFactor(),
                Y = DateTime.Now,
            };

            return Task.CompletedTask;
        }

        Task OnHorizontalBarRefreshed( ChartStreamingData<LiveDataPoint> data )
        {
            data.Value = new LiveDataPoint
            {
                X = DateTime.Now,
                Y = RandomScalingFactor(),
            };

            return Task.CompletedTask;
        }

        Task OnVerticalBarRefreshed( ChartStreamingData<LiveDataPoint> data )
        {
            data.Value = new LiveDataPoint
            {
                X = RandomScalingFactor(),
                Y = DateTime.Now,
            };

            return Task.CompletedTask;
        }

        double RandomScalingFactor()
        {
            return ( random.NextDouble() > 0.5 ? 1.0 : -1.0 ) * Math.Round( random.NextDouble() * 100 );
        }
    }
}
