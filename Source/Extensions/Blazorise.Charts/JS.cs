#region Using directives
using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts
{
    static class JS
    {
        private static object CreateDotNetObjectRefSyncObj = new object();

        public static DotNetObjectReference<ChartAdapter> CreateDotNetObjectRef( ChartAdapter adapter )
        {
            lock ( CreateDotNetObjectRefSyncObj )
            {
                return DotNetObjectReference.Create( adapter );
            }
        }

        public static void DisposeDotNetObjectRef( DotNetObjectReference<ChartAdapter> dotNetObjectReference )
        {
            if ( dotNetObjectReference != null )
            {
                lock ( CreateDotNetObjectRefSyncObj )
                {
                    dotNetObjectReference.Dispose();
                }
            }
        }

        public static ValueTask<bool> InitializeChart<TItem, TOptions>( DotNetObjectReference<ChartAdapter> dotNetObjectReference, IJSRuntime runtime, string id, ChartType type, ChartData<TItem> data, TOptions options, string dataJsonString, string optionsJsonString )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.initialize", dotNetObjectReference, id, ToChartTypeString( type ), ToChartDataSet( data ), options, dataJsonString, optionsJsonString );
        }

        public static ValueTask<bool> Destroy( IJSRuntime runtime, string id )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.destroy", id );
        }

        // TODO: clean this
        public static ValueTask<bool> UpdateChart<TItem, TOptions>( IJSRuntime runtime, string id, ChartData<TItem> data, TOptions options, string dataJsonString, string optionsJsonString )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.update", id, ToChartDataSet( data ), options, dataJsonString, optionsJsonString );
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
                Datasets = data.Datasets.ToList<object>()
            };
        }
    }
}
