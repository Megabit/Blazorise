#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Streaming
{
    public class JSChartStreamingModule : BaseJSModule,
        IJSDestroyableModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSChartStreamingModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods

        public virtual async ValueTask<bool> Initialize( DotNetObjectReference<ChartStreamingAdapter> dotNetObjectReference, ElementReference canvasRef, string canvasId, bool vertical, ChartStreamingOptions options )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<bool>( "initialize",
                dotNetObjectReference,
                canvasRef,
                canvasId,
                vertical,
                options );
        }

        public virtual async ValueTask Destroy( ElementReference canvasRef, string canvasId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "destroy", canvasRef, canvasId );
        }

        public virtual async ValueTask<bool> AddData( string canvasId, int datasetIndex, object data )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<bool>( "addData", canvasId, datasetIndex, data );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise.Charts.Streaming/charts.streaming.js?v={VersionProvider.Version}";

        #endregion
    }
}
