using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Charts;
using Blazorise.Charts.DataLabels;
using Blazorise.Charts.Zoom;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsZoomPage
{
    private LineChart<double> lineChart;
    private BarChart<double> barChart;

    private ChartZoom<double> lineChartZoom;
    private ChartZoom<double> barChartZoom;

    #region Line

    LineChartOptions lineChartOptions = new()
    {
        AspectRatio = 1.5,
        Scales = new()
        {
            Y = new()
            {
                Title = new()
                {
                    Display = true,
                    Text = "Kelvins"
                },
                Ticks = new ChartAxisTicks
                {
                    Callback = ( value, index, values ) => value / 1000 + "K",
                    StepSize = 10000
                }
            }
        },
    };

    private ChartZoomPluginOptions lineChartZoomOptions = new()
    {
        Zoom = new()
        {
            Mode = "y",
            Wheel = new()
            {
                Enabled = true,
            },
            Pinch = new()
            {
                Enabled = true
            },
            Drag = new()
            {
                Enabled = true
            }
        },
        Limits = new()
        {
            Y = new()
            {
                Min = 0,
                Max = 50000,
                MinRange = 20000
            }
        },
        Transition = new ChartZoomTransitionOptions()
        {
            Animation = new ChartAnimation()
            {
                Duration = 1000,
                Easing = "easeOutCubic"
            }
        }
    };

    #endregion

    #region Bar



    BarChartOptions barChartOptions = new()
    {
        AspectRatio = 5d / 3d,
        Layout = new()
        {
            Padding = new()
            {
                Top = 24,
                Right = 16,
                Bottom = 0,
                Left = 8
            }
        },
        Elements = new()
        {
            Line = new()
            {
                Fill = false,
            },
            Point = new()
            {
                HoverRadius = 7,
                Radius = 5
            }
        },
        Scales = new()
        {
            X = new()
            {
                Stacked = true,
            },
            Y = new()
            {
                Stacked = true,
            }
        },
        Plugins = new()
        {
            Legend = new()
            {
                Display = false
            },
        }
    };

    static Expression<Func<ScriptableOptionsContext, string>> TestScriptableColor = ( context ) => context.Active ? "#ff0000" : "#4bc0c0";

    List<ChartDataLabelsDataset> barDataLabelsDatasets = new()
    {
        new()
        {
            DatasetIndex = 0,
            Options = new()
            {
                BackgroundColor = TestScriptableColor,
                BorderColor = TestScriptableColor,
                // BackgroundColor = BackgroundColors[0],
                //BorderColor = BorderColors[0],
                Align = "end",
                Anchor = "start"
            }
        },
        new()
        {
            DatasetIndex = 1,
            Options = new ()
            {
                BackgroundColor = BackgroundColors[1],
                BorderColor = BorderColors[1],
            }
        },
        new()
        {
            DatasetIndex = 2,
            Options = new ()
            {
                BackgroundColor = BackgroundColors[2],
                BorderColor = BorderColors[2],
                Align = "center",
                Anchor = "center"
            }
        },
    };

    ChartDataLabelsOptions barDataLabelsOptions = new()
    {
        BorderRadius = 4,
        Color = "#ffffff",
        Font = new()
        {
            Weight = "bold"
        },
        Formatter = ChartMathFormatter.Round,
        Padding = new( 6 )
    };



    private ChartZoomPluginOptions barChartZoomOptions = new()
    {
        Zoom = new()
        {
            Mode = "y",
            Wheel = new()
            {
                Enabled = true,
            },
            Pinch = new()
            {
                Enabled = true
            },
            Drag = new()
            {
                Enabled = true
            }
        },
        Limits = new()
        {
            Y = new()
            {
                Min = 0,
                Max = 4000,
                MinRange = 1000
            }
        },
        Transition = new ChartZoomTransitionOptions()
        {
            Animation = new ChartAnimation()
            {
                Duration = 1000,
                Easing = "easeOutCubic"
            }
        }
    };
    #endregion

    private string[] Labels = { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };
    private static List<string> BackgroundColors = new() { ChartColor.FromRgba( 255, 99, 132, 0.2f ), ChartColor.FromRgba( 54, 162, 235, 0.2f ), ChartColor.FromRgba( 255, 206, 86, 0.2f ), ChartColor.FromRgba( 75, 192, 192, 0.2f ), ChartColor.FromRgba( 153, 102, 255, 0.2f ), ChartColor.FromRgba( 255, 159, 64, 0.2f ) };
    private static List<string> BorderColors = new() { ChartColor.FromRgba( 255, 99, 132, 1f ), ChartColor.FromRgba( 54, 162, 235, 1f ), ChartColor.FromRgba( 255, 206, 86, 1f ), ChartColor.FromRgba( 75, 192, 192, 1f ), ChartColor.FromRgba( 153, 102, 255, 1f ), ChartColor.FromRgba( 255, 159, 64, 1f ) };

    private Random random = new( DateTime.Now.Millisecond );


    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await Task.WhenAll(
                HandleRedraw( lineChart, GetLineChartDataset ),
                HandleRedraw( barChart, GetBarChartDataset ) );
        }
    }
    private async Task HandleRedraw<TDataSet, TItem, TOptions, TModel>( Blazorise.Charts.BaseChart<TDataSet, TItem, TOptions, TModel> chart, Func<TDataSet> getDataSet )
        where TDataSet : ChartDataset<TItem>
        where TOptions : ChartOptions
        where TModel : ChartModel
    {
        await chart.Clear();

        await chart.AddLabelsDatasetsAndUpdate( Labels, getDataSet() );
    }

    private LineChartDataset<double> GetLineChartDataset()
    {
        return new()
        {
            Label = "# of randoms",
            Data = RandomizeData( 3000, 50000 ),
            BackgroundColor = BackgroundColors[0], // line chart can only have one color
            BorderColor = BorderColors[0],
            Fill = true,
            PointRadius = 3,
            BorderWidth = 1,
            PointBorderColor = Enumerable.Repeat( BorderColors[0], 6 ).ToList(),
            CubicInterpolationMode = "monotone",
        };
    }

    private BarChartDataset<double> GetBarChartDataset()
    {
        return new()
        {
            Label = "# of randoms",
            Data = RandomizeData( 2000, 9000 ),
            BackgroundColor = BackgroundColors[2],
            BorderColor = BorderColors[2],
            BorderWidth = 1
        };
    }

    List<double> RandomizeData( int min, int max )
    {
        return Enumerable.Range( 0, 6 ).Select( x => random.Next( min, max ) * random.NextDouble() ).ToList();
    }

    async Task OnResetZoomLevelClicked()
    {
        await lineChartZoom.ResetZoomLevel();
    }

    async Task OnSetZoomLevelClicked()
    {
        await lineChartZoom.SetZoomLevel( 2 );
    }

    Task OnZoomed( double zoomLevel )
    {
        Console.WriteLine( $"Zoomed to {zoomLevel}" );

        return Task.CompletedTask;
    }
}