using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Charts;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsPage
{
    private LineChart<double> lineChart;
    private Chart<double> barChart;
    private Chart<double> pieChart;
    private Chart<double> doughnutChart;
    private Chart<double> polarAreaChart;
    private Chart<double> radarChart;
    private Chart<ScatterChartPoint> scatterChart;
    private Chart<BubbleChartPoint> bubbleChart;
    private LineChart<double> multiAxisLineChart;

    ChartOptions chartOptions = new()
    {
        AspectRatio = 1.5,
        Plugins = new ChartPlugins()
        {
            Tooltip = new ChartTooltip()
            {
                Enabled = true,
                UsePointStyle = true,
                Callbacks = new ChartTooltipCallbacks
                {
                    Title = ( items ) => "Custom title: " + items[0].Parsed,
                    Label = ( item ) => "Custom label: " + item.Parsed,
                }
            }
        }
    };

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
        }
    };

    LineChartOptions line2ChartOptions = new()
    {
        AspectRatio = 1.5
    };

    object multiAxisLineChartOptions = new
    {
        responsive = true,
        interaction = new
        {
            mode = "index",
            intersect = false,
        },
        stacked = false,
        plugins = new
        {
            title = new
            {
                display = true,
                text = "Chart.js Line Chart - Multi Axis"
            }
        },
        scales = new
        {
            y = new
            {
                type = "linear",
                display = true,
                position = "left"
            },
            y1 = new
            {
                type = "linear",
                display = true,
                position = "right",
                grid = new
                {
                    drawOnChartArea = false
                }
            }
        }
    };

    private LineChart<double> lineChartWithData;

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

    private bool isAlreadyInitialised;

    private Random random = new( DateTime.Now.Millisecond );

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( !isAlreadyInitialised )
        {
            isAlreadyInitialised = true;

            await Task.WhenAll(
                HandleRedraw( lineChart, GetLineChartDataset ),
                HandleRedraw( barChart, GetBarChartDataset ),
                HandleRedraw( pieChart, GetPieChartDataset ),
                HandleRedraw( doughnutChart, GetDoughnutChartDataset ),
                HandleRedraw( polarAreaChart, GetPolarAreaChartDataset ),
                HandleRedraw( radarChart, GetRadarChartDataset ),
                HandleRedraw( lineChartWithData, GetLineChartDataset ),
                HandleRedraw( scatterChart, GetScatterChartDataset ),
                HandleRedraw( bubbleChart, GetBubbleChartDataset ),
                HandleRedraw( multiAxisLineChart, GetLineChartDataset ) );
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

    private async Task SetDataAndUpdate<TDataSet, TItem, TOptions, TModel>( Blazorise.Charts.BaseChart<TDataSet, TItem, TOptions, TModel> chart, Func<List<TItem>> items )
        where TDataSet : ChartDataset<TItem>
        where TOptions : ChartOptions
        where TModel : ChartModel
    {
        await chart.SetData( 0, items() );
        await chart.Update();
    }

    private ChartDataset<double> GetChartDataset()
    {
        return new()
        {
            Label = "# of randoms",
            Data = RandomizeData(),
            BackgroundColor = BackgroundColors,
            BorderColor = BorderColors
        };
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
            PointBorderColor = Enumerable.Repeat( BorderColors.First(), 6 ).ToList(),
            CubicInterpolationMode = "monotone",
        };
    }

    private ScatterChartDataset<ScatterChartPoint> GetScatterChartDataset()
    {
        return new()
        {
            Label = "# of randoms",
            Data = RandomizeScatterData( 3000, 50000 ),
            BackgroundColor = BackgroundColors[0], // line chart can only have one color
            BorderColor = BorderColors[0],
            PointRadius = 3,
            BorderWidth = 1,
            PointBorderColor = Enumerable.Repeat( BorderColors.First(), 6 ).ToList()
        };
    }

    private BubbleChartDataset<BubbleChartPoint> GetBubbleChartDataset()
    {
        return new()
        {
            Label = "# of randoms",
            Data = RandomizeBubbleData( 3000, 50000 ),
            BackgroundColor = BackgroundColors[0], // line chart can only have one color
            BorderColor = BorderColors[0],
        };
    }

    private BarChartDataset<double> GetBarChartDataset()
    {
        return new()
        {
            Label = "# of randoms",
            Data = RandomizeData(),
            BackgroundColor = BackgroundColors,
            BorderColor = BorderColors,
            BorderWidth = 1
        };
    }

    private int pieLabel;

    private PieChartDataset<double> GetPieChartDataset()
    {
        return new()
        {
            Label = $"#{++pieLabel} of randoms",
            Data = RandomizeData(),
            BackgroundColor = BackgroundColors,
            BorderColor = BorderColors,
            BorderWidth = 1
        };
    }

    private DoughnutChartDataset<double> GetDoughnutChartDataset()
    {
        return new()
        {
            Label = "# of randoms",
            Data = RandomizeData(),
            BackgroundColor = BackgroundColors,
            BorderColor = BorderColors,
            BorderWidth = 1
        };
    }

    private PolarAreaChartDataset<double> GetPolarAreaChartDataset()
    {
        return new()
        {
            Label = "# of randoms",
            Data = RandomizeData(),
            BackgroundColor = BackgroundColors,
            BorderColor = BorderColors,
            BorderWidth = 1
        };
    }

    private RadarChartDataset<double> GetRadarChartDataset()
    {
        return new()
        {
            Label = "custom radar",
            Data = RandomizeData(),
            BackgroundColor = BackgroundColors[0], // radar chart can only have one color
            BorderColor = BorderColors,
            Tension = 0.0f,
            BorderWidth = 1,
            Fill = true,
        };
    }

    private async Task ShiftLineChart()
    {
        await lineChart.ShiftData( 0 );
        await lineChart.ShiftLabel();
        await lineChart.Update();
    }

    private async Task PopLineChart()
    {
        await lineChart.PopData( 0 );
        await lineChart.PopLabel();
        await lineChart.Update();
    }

    List<double> RandomizeData() => RandomizeData( 3, 50 );

    List<double> RandomizeData( int min, int max )
    {
        return Enumerable.Range( 0, 6 ).Select( x => random.Next( min, max ) * random.NextDouble() ).ToList();
    }

    List<ScatterChartPoint> RandomizeScatterData() => RandomizeScatterData( 3, 50 );

    List<ScatterChartPoint> RandomizeScatterData( int min, int max )
    {
        return Enumerable.Range( 0, 6 ).Select( x => new ScatterChartPoint(
            random.Next( min, max ) * random.NextDouble(),
            random.Next( min, max ) * random.NextDouble() ) ).ToList();
    }

    List<BubbleChartPoint> RandomizeBubbleData() => RandomizeBubbleData( 3, 50 );

    List<BubbleChartPoint> RandomizeBubbleData( int min, int max )
    {
        return Enumerable.Range( 0, 6 ).Select( x => new BubbleChartPoint(
            random.Next( min, max ) * random.NextDouble(),
            random.Next( min, max ) * random.NextDouble(),
            random.Next( 5, 60 ) * random.NextDouble() ) ).ToList();
    }
}