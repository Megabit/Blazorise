using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Charts;
using Blazorise.Charts.DataLabels;

namespace Blazorise.Demo.Pages.Tests
{
    public partial class ChartsDataLabelsPage
    {
        private LineChart<int> lineChart;

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

        List<ChartDataLabelsDataset> dataLabelsDatasets = new()
        {
            new()
            {
                DatasetIndex = 0,
                Options = new()
                {
                    BackgroundColor = BackgroundColors[0],
                    Align = "start",
                    Anchor = "start"
                }
            },
            new()
            {
                DatasetIndex = 1,
                Options = new ()
                {
                    BackgroundColor = BackgroundColors[1],
                }
            },
            new()
            {
                DatasetIndex = 2,
                Options = new ()
                {
                    BackgroundColor = BackgroundColors[2],
                    Align = "end",
                    Anchor = "end"
                }
            },
        };

        ChartDataLabelsOptions dataLabelsOptions = new()
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

        private static string[] Labels = new string[] { "1", "2", "3", "4", "5", "6" };
        private static string[] BackgroundColors = new string[] { "#4bc0c0", "#36a2eb", "#ff3d88" };
        private static string[] borderColors = new string[] { "#4bc0c0", "#36a2eb", "#ff3d88" };
        private Random random = new( DateTime.Now.Millisecond );

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await lineChart.Clear();

                await lineChart.AddLabelsDatasetsAndUpdate( Labels,
                    GetLineChartDataset( 0 ),
                    GetLineChartDataset( 1 ),
                    GetLineChartDataset( 2 ) );
            }
        }

        private LineChartDataset<int> GetLineChartDataset( int colorIndex )
        {
            return new()
            {
                Label = "# of randoms",
                Data = RandomizeData( 2, 9 ),
                BackgroundColor = BackgroundColors[colorIndex], // line chart can only have one color
                BorderColor = borderColors[colorIndex],
                PointBorderColor = Enumerable.Repeat( borderColors.First(), 6 ).ToList(),
            };
        }

        List<int> RandomizeData( int min, int max )
        {
            return Enumerable.Range( 0, Labels.Count() ).Select( x => random.Next( min, max ) ).ToList();
        }
    }
}
