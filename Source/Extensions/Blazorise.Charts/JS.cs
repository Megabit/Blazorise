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

        public static ValueTask<bool> Initialize<TItem, TOptions>( IJSRuntime runtime, DotNetObjectReference<ChartAdapter> dotNetObjectReference, bool hasClickEvent, bool hasHoverEvent, string canvasId, ChartType type, ChartData<TItem> data, TOptions options, string dataJsonString, string optionsJsonString, object optionsObject )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.initialize",
                dotNetObjectReference,
                hasClickEvent,
                hasHoverEvent,
                canvasId,
                ToChartTypeString( type ),
                ToChartDataSet( data ),
                options,
                dataJsonString,
                optionsJsonString,
                optionsObject );
        }

        public static ValueTask<bool> Destroy( IJSRuntime runtime, string canvasId )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.destroy", canvasId );
        }

        public static ValueTask<bool> SetOptions<TOptions>( IJSRuntime runtime, string canvasId, TOptions options, string optionsJsonString, object optionsObject )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.setOptions", canvasId, options, optionsJsonString, optionsObject );
        }

        public static ValueTask<bool> Update( IJSRuntime runtime, string canvasId )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.update", canvasId );
        }

        public static ValueTask<bool> Clear( IJSRuntime runtime, string canvasId )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.clear", canvasId );
        }

        public static ValueTask<bool> AddLabel( IJSRuntime runtime, string canvasId, params string[] labels )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.addLabel", canvasId, labels );
        }

        public static ValueTask<bool> AddDataSet( IJSRuntime runtime, string canvasId, params object[] newDataSet )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.addDataset", canvasId, newDataSet );
        }

        public static ValueTask<bool> AddData<TItem>( IJSRuntime runtime, string canvasId, int dataSetIndex, params TItem[] newData )
        {
            return runtime.InvokeAsync<bool>( "blazoriseCharts.addData", canvasId, dataSetIndex, newData );
        }

        public static string ToChartTypeString( ChartType type )
        {
            switch ( type )
            {
                case ChartType.Bar:
                    return "bar";
                case ChartType.HorizontalBar:
                    return "horizontalBar";
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
                data?.Labels,
                Datasets = data?.Datasets?.ToList<object>()
            };
        }
    }
}
