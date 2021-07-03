using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Charts;

namespace Blazorise.Demo.Pages.Tests
{
    public partial class ChartsPage
    {
        LineChart<double> lineChart;
        Chart<double> barChart;
        Chart<double> pieChart;
        Chart<double> doughnutChart;
        Chart<double> polarAreaChart;
        Chart<double> radarChart;

        LineChart<double> lineChartWithData;

        string[] Labels = { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };
        List<string> backgroundColors = new List<string> { ChartColor.FromRgba( 255, 99, 132, 0.2f ), ChartColor.FromRgba( 54, 162, 235, 0.2f ), ChartColor.FromRgba( 255, 206, 86, 0.2f ), ChartColor.FromRgba( 75, 192, 192, 0.2f ), ChartColor.FromRgba( 153, 102, 255, 0.2f ), ChartColor.FromRgba( 255, 159, 64, 0.2f ) };
        List<string> borderColors = new List<string> { ChartColor.FromRgba( 255, 99, 132, 1f ), ChartColor.FromRgba( 54, 162, 235, 1f ), ChartColor.FromRgba( 255, 206, 86, 1f ), ChartColor.FromRgba( 75, 192, 192, 1f ), ChartColor.FromRgba( 153, 102, 255, 1f ), ChartColor.FromRgba( 255, 159, 64, 1f ) };

        bool isAlreadyInitialised;

        Random random = new Random( DateTime.Now.Millisecond );

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

        async Task HandleRedraw<TDataSet, TItem, TOptions, TModel>( Blazorise.Charts.BaseChart<TDataSet, TItem, TOptions, TModel> chart, Func<TDataSet> getDataSet )
            where TDataSet : ChartDataset<TItem>
            where TOptions : ChartOptions
            where TModel : ChartModel
        {
            await chart.Clear();

            await chart.AddLabelsDatasetsAndUpdate( Labels, getDataSet(), getDataSet(), getDataSet() );
        }

        async Task SetDataAndUpdate<TDataSet, TItem, TOptions, TModel>( Blazorise.Charts.BaseChart<TDataSet, TItem, TOptions, TModel> chart, Func<List<TItem>> items )
            where TDataSet : ChartDataset<TItem>
            where TOptions : ChartOptions
            where TModel : ChartModel
        {
            await chart.SetData( 0, items() );
            await chart.Update();
        }

        ChartDataset<double> GetChartDataset()
        {
            return new ChartDataset<double>
            {
                Label = "# of randoms",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors,
                BorderColor = borderColors
            };
        }

        LineChartDataset<double> GetLineChartDataset()
        {
            return new LineChartDataset<double>
            {
                Label = "# of randoms",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors[0], // line chart can only have one color
                BorderColor = borderColors[0],
                Fill = true,
                PointRadius = 3,
                BorderWidth = 1,
                PointBorderColor = Enumerable.Repeat( borderColors.First(), 6 ).ToList()
            };
        }

        BarChartDataset<double> GetBarChartDataset()
        {
            return new BarChartDataset<double>
            {
                Label = "# of randoms",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
                BorderWidth = 1
            };
        }

        int pieLabel;

        PieChartDataset<double> GetPieChartDataset()
        {
            return new PieChartDataset<double>

            {
                Label = $"#{++pieLabel} of randoms",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
                BorderWidth = 1
            };
        }

        DoughnutChartDataset<double> GetDoughnutChartDataset()
        {
            return new DoughnutChartDataset<double>
            {
                Label = "# of randoms",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
                BorderWidth = 1
            };
        }

        PolarAreaChartDataset<double> GetPolarAreaChartDataset()
        {
            return new PolarAreaChartDataset<double>
            {
                Label = "# of randoms",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
                BorderWidth = 1
            };
        }

        RadarChartDataset<double> GetRadarChartDataset()
        {
            return new RadarChartDataset<double>
            {
                Label = "custom radar",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors[0], // radar chart can only have one color
                BorderColor = borderColors,
                LineTension = 0.0f,
                BorderWidth = 1
            };
        }

        async Task ShiftLineChart()
        {
            await lineChart.ShiftData( 0 );
            await lineChart.ShiftLabel();
            await lineChart.Update();
        }

        async Task PopLineChart()
        {
            await lineChart.PopData( 0 );
            await lineChart.PopLabel();
            await lineChart.Update();
        }

        List<double> RandomizeData()
        {
            return new List<double> { random.Next( 3, 50 ) * random.NextDouble(), random.Next( 3, 50 ) * random.NextDouble(), random.Next( 3, 50 ) * random.NextDouble(), random.Next( 3, 50 ) * random.NextDouble(), random.Next( 3, 50 ) * random.NextDouble(), random.Next( 3, 50 ) * random.NextDouble() };
        }
    }
}
