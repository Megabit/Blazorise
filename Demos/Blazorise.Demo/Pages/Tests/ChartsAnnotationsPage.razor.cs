using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Charts;
using Blazorise.Charts.Annotation;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsAnnotationsPage
{
    private LineChart<int> lineChart;
    private BarChart<int> barChart;

    #region Line

    LineChartOptions lineChartOptions = new()
    {
        AspectRatio = 5d / 3d,
        Layout = new()
        {
            Padding = new()
            {
                Top = 32,
                Right = 16,
                Bottom = 16,
                Left = 8
            }
        },
        Elements = new()
        {
            Line = new()
            {
                Fill = false,
                Tension = 0.4,
            }
        },
        Scales = new()
        {
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
            }
        }
    };

    Dictionary<string, ChartAnnotationOptions> lineAnnotationOptions = new()
    {
        { "line1", new LineChartAnnotationOptions
            {
                Type = "line",
                YMin = 10,
                YMax = 10,
                BorderColor = new( 255, 99, 132 ),
                BorderWidth = 5,
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
            }
        }
    };

    static Expression<Func<ScriptableOptionsContext, string>> TestScriptableColor = ( context ) => context.Active ? "#ff0000" : "#4bc0c0";

    Dictionary<string, ChartAnnotationOptions> boxAnnotationOptions = new()
    {
        { "box1", new BoxChartAnnotationOptions
            {
                Type = "box",
                XMin = 1,
                XMax = 2,
                YMin = 50,
                YMax = 70,
                BackgroundColor = new( 255, 99, 132, 0.5f ),
            }
        }
    };

    #endregion

    private static string[] Labels = new string[] { "1", "2", "3", "4", "5", "6" };
    private static string[] BackgroundColors = new string[] { "#4bc0c0", "#36a2eb", "#ff3d88" };
    private static string[] BorderColors = new string[] { "#4bc0c0", "#36a2eb", "#ff3d88" };
    private Random random = new( DateTime.Now.Millisecond );

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await Task.WhenAll(
                HandleRedraw( lineChart, GetLineChartDataset ),
                HandleRedraw( barChart, GetBarChartDataset ) );

            await lineChart.Clear();

            await lineChart.AddLabelsDatasetsAndUpdate( Labels,
                GetLineChartDataset( 0 ),
                GetLineChartDataset( 1 ),
                GetLineChartDataset( 2 ) );
        }
    }

    private async Task HandleRedraw<TDataSet, TItem, TOptions, TModel>( Blazorise.Charts.BaseChart<TDataSet, TItem, TOptions, TModel> chart, Func<int, TDataSet> getDataSet )
        where TDataSet : ChartDataset<TItem>
        where TOptions : ChartOptions
        where TModel : ChartModel
    {
        await chart.Clear();

        await chart.AddLabelsDatasetsAndUpdate( Labels,
            getDataSet( 0 ),
            getDataSet( 1 ),
            getDataSet( 2 ) );
    }

    private LineChartDataset<int> GetLineChartDataset( int colorIndex )
    {
        return new()
        {
            Label = "# of randoms",
            Data = RandomizeData( 2, 9 ),
            BackgroundColor = BackgroundColors[colorIndex],
            BorderColor = BorderColors[colorIndex],
        };
    }

    private BarChartDataset<int> GetBarChartDataset( int colorIndex )
    {
        return new()
        {
            Label = "# of randoms",
            Data = RandomizeData( 2, 9 ),
            BackgroundColor = BackgroundColors[colorIndex],
            BorderColor = BorderColors[colorIndex],
            BorderWidth = 1
        };
    }

    List<int> RandomizeData( int min, int max )
    {
        return Enumerable.Range( 0, Labels.Count() ).Select( x => random.Next( min, max ) ).ToList();
    }
}
