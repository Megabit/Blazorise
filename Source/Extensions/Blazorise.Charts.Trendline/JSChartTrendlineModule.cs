#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Trendline
{
    public class JSChartTrendlineModule : BaseJSModule,
        IJSDestroyableModule
    {
        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSChartTrendlineModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public virtual async ValueTask<bool> Initialize( DotNetObjectReference<ChartTrendlineAdapter> dotNetObjectReference, ElementReference canvasRef, string canvasId, bool vertical, ChartTrendlineOptions options )
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

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise.Charts.Trendline/charts.trendline.js?v={VersionProvider.Version}";
    }
}
