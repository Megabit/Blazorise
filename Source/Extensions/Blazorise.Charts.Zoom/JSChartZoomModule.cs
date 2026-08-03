#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// Invokes pan and zoom operations provided by the Chart.js zoom plugin.
/// </summary>
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

    /// <summary>
    /// Activates zoom interaction and .NET event callbacks for a chart.
    /// </summary>
    public virtual async ValueTask Initialize<TItem>( DotNetObjectReference<ChartZoomAdapter<TItem>> dotNetObjectReference, string canvasId, ChartZoomPluginOptions options )
    {
        await InvokeSafeVoidAsync( "initialize", dotNetObjectReference, canvasId, options );
    }

    /// <summary>
    /// Restores a chart to its original scale and position.
    /// </summary>
    public virtual async ValueTask ResetZoomLevel( string canvasId )
    {
        await InvokeSafeVoidAsync( "resetZoomLevel", canvasId );
    }

    /// <summary>
    /// Reads the chart's current uniform scale factor.
    /// </summary>
    public virtual async ValueTask<double> GetZoomLevel( string canvasId )
    {
        return await InvokeSafeAsync<double>( "getZoomLevel", canvasId );
    }

    /// <summary>
    /// Applies one scale factor to both chart axes.
    /// </summary>
    public virtual async ValueTask SetZoomLevel( string canvasId, double zoomLevel )
    {
        await InvokeSafeVoidAsync( "setZoomLevel", canvasId, zoomLevel );
    }

    /// <summary>
    /// Applies independent horizontal and vertical scale factors.
    /// </summary>
    public virtual async ValueTask SetZoomLevel( string canvasId, double zoomLevelX, double zoomLevelY )
    {
        await InvokeSafeVoidAsync( "setZoomLevel", canvasId, new { x = zoomLevelX, y = zoomLevelY } );
    }

    /// <summary>
    /// Moves the visible chart range equally along both axes.
    /// </summary>
    public virtual async ValueTask Pan( string canvasId, double delta )
    {
        await InvokeSafeVoidAsync( "pan", canvasId, delta );
    }

    /// <summary>
    /// Moves the visible chart range by separate axis offsets.
    /// </summary>
    public virtual async ValueTask Pan( string canvasId, double deltaX, double deltaY )
    {
        await InvokeSafeVoidAsync( "pan", canvasId, new { x = deltaX, y = deltaY } );
    }

    /// <summary>
    /// Reports whether a pointer-driven zoom or pan gesture is active.
    /// </summary>
    public virtual async ValueTask<bool> IsZoomingOrPanning( string canvasId )
    {
        return await InvokeSafeAsync<bool>( "isZoomingOrPanning", canvasId );
    }

    /// <summary>
    /// Reports whether the chart differs from its initial viewport.
    /// </summary>
    public virtual async ValueTask<bool> IsZoomedOrPanned( string canvasId )
    {
        return await InvokeSafeAsync<bool>( "isZoomedOrPanned", canvasId );
    }

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Charts.Zoom/chart.zoom.js?v={VersionProvider.Version}";
}