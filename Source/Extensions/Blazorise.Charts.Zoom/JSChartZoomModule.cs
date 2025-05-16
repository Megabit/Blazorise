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

    public virtual async ValueTask AddZoom<TItem>( DotNetObjectReference<ChartZoomAdapter<TItem>> dotNetObjectReference, string canvasId, ChartZoomPluginOptions options )
    {
        await InvokeSafeVoidAsync( "addZoom", dotNetObjectReference, canvasId, options );
    }

    public virtual async ValueTask ResetZoomLevel( string canvasId )
    {
        await InvokeSafeVoidAsync( "resetZoomLevel", canvasId );
    }

    public virtual async ValueTask<double> GetZoomLevel( string canvasId )
    {
        return await InvokeSafeAsync<double>( "getZoomLevel", canvasId );
    }

    public virtual async ValueTask SetZoomLevel( string canvasId, double zoomLevel )
    {
        await InvokeSafeVoidAsync( "setZoomLevel", canvasId, zoomLevel );
    }

    public virtual async ValueTask SetZoomLevel( string canvasId, double zoomLevelX, double zoomLevelY )
    {
        await InvokeSafeVoidAsync( "setZoomLevel", canvasId, new { x = zoomLevelX, y = zoomLevelY } );
    }

    public virtual async ValueTask<bool> IsZoomingOrPanning( string canvasId )
    {
        return await InvokeSafeAsync<bool>( "isZoomingOrPanning", canvasId );
    }

    public virtual async ValueTask<bool> IsZoomedOrPanned( string canvasId )
    {
        return await InvokeSafeAsync<bool>( "isZoomedOrPanned", canvasId );
    }

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Charts.Zoom/chart.zoom.js?v={VersionProvider.Version}";
}
