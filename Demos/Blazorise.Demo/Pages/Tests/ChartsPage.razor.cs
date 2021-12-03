using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Charts;

namespace Blazorise.Demo.Pages.Tests
{
    public partial class ChartsPage
    {
        private LineChart<double> lineChart;
        private Chart<double> barChart;
        private Chart<double> pieChart;
        private Chart<double> doughnutChart;
        private Chart<double> polarAreaChart;
        private Chart<double> radarChart;

        LineChartOptions lineChartOptions = new()
        {
            Scales = new()
            {
                Y = new()
                {
                    Ticks = new ChartAxisTicks
                    {
                        Callback = ( value, index, values ) => value / 1000 + "K"
                    }
                }
            }
        };

        private LineChart<double> lineChartWithData;

        private string[] Labels = { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };
        private List<string> backgroundColors = new() { ChartColor.FromRgba( 255, 99, 132, 0.2f ), ChartColor.FromRgba( 54, 162, 235, 0.2f ), ChartColor.FromRgba( 255, 206, 86, 0.2f ), ChartColor.FromRgba( 75, 192, 192, 0.2f ), ChartColor.FromRgba( 153, 102, 255, 0.2f ), ChartColor.FromRgba( 255, 159, 64, 0.2f ) };
        private List<string> borderColors = new() { ChartColor.FromRgba( 255, 99, 132, 1f ), ChartColor.FromRgba( 54, 162, 235, 1f ), ChartColor.FromRgba( 255, 206, 86, 1f ), ChartColor.FromRgba( 75, 192, 192, 1f ), ChartColor.FromRgba( 153, 102, 255, 1f ), ChartColor.FromRgba( 255, 159, 64, 1f ) };

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
                    HandleRedraw( lineChartWithData, GetLineChartDataset ) );
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
                BackgroundColor = backgroundColors,
                BorderColor = borderColors
            };
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
                PointBorderColor = Enumerable.Repeat( borderColors.First(), 6 ).ToList()
            };
        }

        private BarChartDataset<double> GetBarChartDataset()
        {
            return new()
            {
                Label = "# of randoms",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
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
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
                BorderWidth = 1
            };
        }

        private DoughnutChartDataset<double> GetDoughnutChartDataset()
        {
            return new()
            {
                Label = "# of randoms",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
                BorderWidth = 1
            };
        }

        private PolarAreaChartDataset<double> GetPolarAreaChartDataset()
        {
            return new()
            {
                Label = "# of randoms",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
                BorderWidth = 1
            };
        }

        private RadarChartDataset<double> GetRadarChartDataset()
        {
            return new()
            {
                Label = "custom radar",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors[0], // radar chart can only have one color
                BorderColor = borderColors,
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
    }
}