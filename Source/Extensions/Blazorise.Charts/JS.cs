#region Using directives
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts
{
    static class JS
    {
        private static readonly object CreateDotNetObjectRefSyncObj = new object();

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

        public static ValueTask Initialize<TItem, TOptions>( IJSRuntime runtime, DotNetObjectReference<ChartAdapter> dotNetObjectReference, bool hasClickEvent, bool hasHoverEvent, string canvasId, ChartType type, ChartData<TItem> data, TOptions options, string dataJsonString, string optionsJsonString, object optionsObject )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.initialize",
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

        public static ValueTask Destroy( IJSRuntime runtime, string canvasId )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.destroy", canvasId );
        }

        public static ValueTask SetOptions<TOptions>( IJSRuntime runtime, string canvasId, TOptions options, string optionsJsonString, object optionsObject )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.setOptions", canvasId, options, optionsJsonString, optionsObject );
        }

        public static ValueTask Update( IJSRuntime runtime, string canvasId )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.update", canvasId );
        }

        public static ValueTask Clear( IJSRuntime runtime, string canvasId )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.clear", canvasId );
        }

        public static ValueTask AddLabel( IJSRuntime runtime, string canvasId, IReadOnlyCollection<object> newLabels )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.addLabel", canvasId, newLabels );
        }

        public static ValueTask AddDataSet( IJSRuntime runtime, string canvasId, IReadOnlyCollection<object> newDataSet )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.addDataset", canvasId, newDataSet );
        }

        public static ValueTask AddDatasetsAndUpdate( IJSRuntime runtime, string canvasId, IReadOnlyCollection<object> newDataSet )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.addDatasetsAndUpdate", canvasId, newDataSet );
        }

        public static ValueTask AddLabelsDatasetsAndUpdate( IJSRuntime runtime, string canvasId, IReadOnlyCollection<object> newLabels, IReadOnlyCollection<object> newDataSet )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.addLabelsDatasetsAndUpdate", canvasId, newLabels, newDataSet );
        }

        public static ValueTask SetData<TItem>( IJSRuntime runtime, string canvasId, int dataSetIndex, IReadOnlyCollection<TItem> newData )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.setData", canvasId, dataSetIndex, newData );
        }

        public static ValueTask AddData<TItem>( IJSRuntime runtime, string canvasId, int dataSetIndex, IReadOnlyCollection<TItem> newData )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.addData", canvasId, dataSetIndex, newData );
        }

        public static ValueTask ShiftLabel( IJSRuntime runtime, string canvasId )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.shiftLabel", canvasId );
        }

        public static ValueTask ShiftData( IJSRuntime runtime, string canvasId, int dataSetIndex )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.shiftData", canvasId, dataSetIndex );
        }
        public static ValueTask PopLabel( IJSRuntime runtime, string canvasId )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.popLabel", canvasId );
        }

        public static ValueTask PopData( IJSRuntime runtime, string canvasId, int dataSetIndex )
        {
            return runtime.InvokeVoidAsync( "blazoriseCharts.popData", canvasId, dataSetIndex );
        }


        public static string ToChartTypeString( ChartType type )
        {
            return type switch
            {
                ChartType.Bar => "bar",
                ChartType.HorizontalBar => "horizontalBar",
                ChartType.Pie => "pie",
                ChartType.Doughnut => "doughnut",
                ChartType.Radar => "radar",
                ChartType.PolarArea => "polarArea",
                _ => "line",
            };
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
