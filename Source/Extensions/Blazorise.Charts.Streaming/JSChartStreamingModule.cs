#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Streaming
{
    public class JSChartStreamingModule : BaseJSModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        public JSChartStreamingModule( IJSRuntime jsRuntime )
            : base( jsRuntime )
        {
        }

        #endregion

        #region Methods

        public virtual async ValueTask<bool> Initialize( DotNetObjectReference<ChartStreamingAdapter> dotNetObjectReference, string canvasId, bool vertical, ChartStreamingOptions options )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<bool>( "initialize",
                dotNetObjectReference,
                canvasId,
                vertical,
                options );
        }

        public virtual async ValueTask<bool> AddData( string canvasId, int datasetIndex, object data )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<bool>( "addData", canvasId, datasetIndex, data );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => "./_content/Blazorise.Charts.Streaming/blazorise.charts.streaming.js";

        #endregion
    }
}
