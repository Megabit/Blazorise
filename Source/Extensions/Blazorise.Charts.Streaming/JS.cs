#region Using directives
using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts.Streaming
{
    static class JS
    {
        public static DotNetObjectReference<T> CreateDotNetObjectRef<T>( T value ) where T : class
        {
            return DotNetObjectReference.Create( value );
        }

        public static void DisposeDotNetObjectRef<T>( DotNetObjectReference<T> value ) where T : class
        {
            if ( value != null )
            {
                value.Dispose();
            }
        }

        public static ValueTask<bool> Initialize( IJSRuntime runtime, DotNetObjectReference<ChartStreamingAdapter> dotNetObjectReference, string canvasId, bool vertical, ChartStreamingOptions options )
        {
            return runtime.InvokeAsync<bool>( "window.blazoriseChartsStreaming.initialize",
                dotNetObjectReference,
                canvasId,
                vertical,
                options );
        }

        public static ValueTask<bool> AddData( IJSRuntime runtime, string canvasId, int datasetIndex, object data )
        {
            return runtime.InvokeAsync<bool>( "window.blazoriseChartsStreaming.addData", canvasId, datasetIndex, data );
        }
    }
}
