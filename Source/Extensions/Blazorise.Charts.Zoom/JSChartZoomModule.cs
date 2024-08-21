#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Zoom;

public class JSChartZoomModule : BaseJSModule
{
    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSChartZoomModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public virtual async ValueTask AddZoom( string canvasId, ChartZoomPluginOptions options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "addZoom", canvasId, options );
    }

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Charts.Zoom/chart.zoom.js?v={VersionProvider.Version}";
}
