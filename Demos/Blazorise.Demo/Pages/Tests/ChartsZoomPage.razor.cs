using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Charts;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsZoomPage
{
    private LineChart<double> lineChart;

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
        Plugins = new ChartPlugins()
        {
            Zoom = new()
            {
                Zoom = new()
                {
                    Mode = "xy",
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
                    X = new()
                    {
                        Min = 0,
                        Max = 50000,
                    },
                    Y = new()
                    {
                        Min = 0,
                        Max = 50000
                    }
                },
                Animation = new ChartAnimation()
                {
                    Duration = 1000,
                    Easing = "easeOutCubic"
                }
            }
        }
    };


    #endregion

    private string[] Labels = { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };
    private List<string> backgroundColors = new() { ChartColor.FromRgba( 255, 99, 132, 0.2f ), ChartColor.FromRgba( 54, 162, 235, 0.2f ), ChartColor.FromRgba( 255, 206, 86, 0.2f ), ChartColor.FromRgba( 75, 192, 192, 0.2f ), ChartColor.FromRgba( 153, 102, 255, 0.2f ), ChartColor.FromRgba( 255, 159, 64, 0.2f ) };
    private List<string> borderColors = new() { ChartColor.FromRgba( 255, 99, 132, 1f ), ChartColor.FromRgba( 54, 162, 235, 1f ), ChartColor.FromRgba( 255, 206, 86, 1f ), ChartColor.FromRgba( 75, 192, 192, 1f ), ChartColor.FromRgba( 153, 102, 255, 1f ), ChartColor.FromRgba( 255, 159, 64, 1f ) };

    private Random random = new( DateTime.Now.Millisecond );

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await HandleRedraw( lineChart, GetLineChartDataset );
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
            BackgroundColor = backgroundColors[0], // line chart can only have one color
            BorderColor = borderColors[0],
            Fill = true,
            PointRadius = 3,
            BorderWidth = 1,
            PointBorderColor = Enumerable.Repeat( borderColors.First(), 6 ).ToList(),
            CubicInterpolationMode = "monotone",
        };
    }
    List<double> RandomizeData() => RandomizeData( 3, 50 );

    List<double> RandomizeData( int min, int max )
    {
        return Enumerable.Range( 0, 6 ).Select( x => random.Next( min, max ) * random.NextDouble() ).ToList();
    }


}