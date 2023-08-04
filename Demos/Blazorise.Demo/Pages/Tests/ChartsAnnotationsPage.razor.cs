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
    private LineChart<int> lineChartWithBoxes;

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
                Label = new()
                {
                    BackgroundColor = "#ff0000",
                    Content = "Test Label",
                    Display = true
                },
                YMin = 60,
                YMax = 60,
                BorderColor = new( 255, 99, 132 ),
                BorderWidth = 5,
            }
        }
    };

    #endregion

    #region Box

    LineChartOptions lineChartWithBoxesOptions = new()
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
                BeginAtZero = true,
                Min = 0,
                Max = 120,
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
                BackgroundColor = "rgba(255, 245, 157, 0.2)",
                BorderWidth = 0,
                XMax = 2.5,
                XMin = -0.5,
                Label = new()
                {
                    DrawTime = "afterDraw",
                    Display = true,
                    Content = "First quarter",
                    Position = new { x = "center", y = "start" }
                },
            }
        },
        { "box2", new BoxChartAnnotationOptions
            {
                Type = "box",
                BackgroundColor = "rgba(188, 170, 164, 0.2)",
                BorderWidth = 0,
                XMax = 5.5,
                XMin = 2.5,
                Label = new()
                {
                    DrawTime = "afterDraw",
                    Display = true,
                    Content = "Second quarter",
                    Position = new { x = "center", y = "start" }
                },
            }
        },
        { "box3", new BoxChartAnnotationOptions
            {
                Type = "box",
                BackgroundColor = "rgba(165, 214, 167, 0.2)",
                BorderWidth = 0,
                XMax = 8.5,
                XMin = 5.5,
                Label = new()
                {
                    DrawTime = "afterDraw",
                    Display = true,
                    Content = "Third quarter",
                    Position = new { x = "center", y = "start" }
                },
            }
        },
        { "box4", new BoxChartAnnotationOptions
            {
                Type = "box",
                BackgroundColor = "rgba(159, 168, 218, 0.2)",
                BorderWidth = 0,
                XMin = 8.5,
                Label = new()
                {
                    DrawTime = "afterDraw",
                    Display = true,
                    Content = "Fourth quarter",
                    Position = new { x = "center", y = "start" }
                },
            }
        }
    };

    #endregion

    private static string[] Labels = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
    private static string[] BackgroundColors = new string[] { "#4bc0c0", "#36a2eb", "#ff3d88" };
    private static string[] BorderColors = new string[] { "#4bc0c0", "#36a2eb", "#ff3d88" };
    private Random random = new( DateTime.Now.Millisecond );

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await lineChart.Clear();
            await lineChart.AddLabelsDatasetsAndUpdate( Labels, GetLineChartDataset( 0 ), GetLineChartDataset( 1 ), GetLineChartDataset( 2 ) );

            await lineChartWithBoxes.Clear();
            await lineChartWithBoxes.AddLabelsDatasetsAndUpdate( Labels, GetLineChartDataset( 1 ) );

            //await lineChart.Clear();

            //await lineChart.AddLabelsDatasetsAndUpdate( Labels,
            //    GetLineChartDataset( 0 ),
            //    GetLineChartDataset( 1 ),
            //    GetLineChartDataset( 2 ) );
        }
    }

    private LineChartDataset<int> GetLineChartDataset( int colorIndex )
    {
        return new()
        {
            Label = "# of randoms",
            Data = RandomizeData( 2, 110 ),
            BackgroundColor = BackgroundColors[colorIndex],
            BorderColor = BorderColors[colorIndex],
        };
    }

    List<int> RandomizeData( int min, int max )
    {
        return Enumerable.Range( 0, Labels.Count() ).Select( x => random.Next( min, max ) ).ToList();
    }
}
