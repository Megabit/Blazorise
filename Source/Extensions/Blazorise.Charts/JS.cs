#region Using directives
using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts
{
    static class JS
    {
        // TODO: clean this
        public static Task<bool> SetChartData<TItem, TOptions>( IJSRuntime runtime, string id, ChartType type, ChartData<TItem> data, TOptions options )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.setChartData", id, ToChartTypeString( type ), ToChartDataSet( data ), options);
        }

        public static string ToChartTypeString( ChartType type )
        {
            switch ( type )
            {
                case ChartType.Bar:
                    return "bar";
                case ChartType.Pie:
                    return "pie";
                case ChartType.Doughnut:
                    return "doughnut";
                case ChartType.Radar:
                    return "radar";
                case ChartType.PolarArea:
                    return "polarArea";
                case ChartType.Line:
                default:
                    return "line";
            }
        }

        private static object ToChartDataSet<T>( ChartData<T> data )
        {
            return new
            {
                data.Labels,
                Datasets = data.Datasets.Select( d => d as object ).ToList()
            };
        }
    }
}
