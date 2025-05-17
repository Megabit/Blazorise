#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// Provides the annotation capabilities to the supported chart types.
/// </summary>
/// <typeparam name="TItem">Data point type.</typeparam>
public partial class ChartZoom<TItem> : ChartPlugin<TItem, JSChartZoomModule>
{
    #region Methods

    /// <inheritdoc/>
    protected override JSChartZoomModule CreatePluginJsModule()
    {
        DotNetObjectRef ??= DotNetObjectReference.Create( new ChartZoomAdapter<TItem>( this ) );

        return new( JSRuntime, VersionProvider, BlazoriseOptions );
    }

    /// <inheritdoc/>
    protected override async Task InitializePlugin()
        => await JSModule.Initialize( DotNetObjectRef, ParentChart.ElementId, Options );

    /// <inheritdoc/>
    protected override bool UpdatePluginParameters( ParameterView parameters )
        => parameters.TryGetValue<ChartZoomPluginOptions>( nameof( Options ), out var paramOptions ) && !Options.IsEqual( paramOptions );

    /// <inheritdoc/>
    protected override ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            if ( DotNetObjectRef != null )
            {
                DotNetObjectRef.Dispose();
                DotNetObjectRef = null;
            }
        }

        return base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Asynchronously retrieves the current zoom level of the chart.
    /// </summary>
    /// <remarks>The zoom level represents the scaling factor applied to the chart. A value of <see
    /// langword="1.0"/>  indicates no zoom, while values greater than <see langword="1.0"/> indicate zooming in, and
    /// values  less than <see langword="1.0"/> indicate zooming out.</remarks>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation.  The result is a <see
    /// cref="double"/> value indicating the current zoom level of the chart.  The default value is <see
    /// langword="1.0"/> if the JavaScript module is not initialized.</returns>
    public async ValueTask<double> GetZoomLevel()
    {
        if ( JSModule is null )
            return 1.0;

        return await JSModule.GetZoomLevel( ParentChart.ElementId );
    }

    /// <summary>
    /// Sets the zoom level for the chart.
    /// </summary>
    /// <remarks>This method adjusts the zoom level of the chart identified by its element ID.  If the
    /// JavaScript module is not initialized, the method will return without performing any action.</remarks>
    /// <param name="zoomLevel">The desired zoom level. Must be a positive value.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public async ValueTask SetZoomLevel( double zoomLevel )
    {
        if ( JSModule is null )
            return;

        await JSModule.SetZoomLevel( ParentChart.ElementId, zoomLevel );
    }

    /// <summary>
    /// Adjusts the zoom level of the chart along the X and Y axes.
    /// </summary>
    /// <remarks>This method asynchronously updates the zoom level of the chart. If the JavaScript module is
    /// not initialized, the method will return without performing any action.</remarks>
    /// <param name="zoomLevelX">The zoom level for the X-axis. Must be a positive value.</param>
    /// <param name="zoomLevelY">The zoom level for the Y-axis. Must be a positive value.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public async ValueTask SetZoomLevel( double zoomLevelX, double zoomLevelY )
    {
        if ( JSModule is null )
            return;

        await JSModule.SetZoomLevel( ParentChart.ElementId, zoomLevelX, zoomLevelY );
    }

    /// <summary>
    /// Resets the zoom level of the chart to its default state.
    /// </summary>
    /// <remarks>This method asynchronously resets the zoom level of the chart associated with the current
    /// instance. If the JavaScript module is not initialized, the method will return without performing any
    /// action.</remarks>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public async ValueTask ResetZoomLevel()
    {
        if ( JSModule is null )
            return;

        await JSModule.ResetZoomLevel( ParentChart.ElementId );
    }

    /// <summary>
    /// Determines whether the chart is currently being zoomed or panned.
    /// </summary>
    /// <remarks>This method checks the current interaction state of the chart by invoking a JavaScript
    /// module. If the JavaScript module is not initialized, the method returns <see langword="false"/>.</remarks>
    /// <returns><see langword="true"/> if the chart is in a zooming or panning state; otherwise, <see langword="false"/>.</returns>
    public async ValueTask<bool> IsZoomingOrPanning()
    {
        if ( JSModule is null )
            return false;

        return await JSModule.IsZoomingOrPanning( ParentChart.ElementId );
    }

    /// <summary>
    /// Determines whether the chart has been zoomed or panned.
    /// </summary>
    /// <remarks>This method checks the current state of the chart to determine if any zooming or panning
    /// actions  have been performed. If the underlying JavaScript module is not initialized, the method will  return
    /// <see langword="false"/>.</remarks>
    /// <returns><see langword="true"/> if the chart has been zoomed or panned; otherwise, <see langword="false"/>.</returns>
    public async ValueTask<bool> IsZoomedOrPanned()
    {
        if ( JSModule is null )
            return false;

        return await JSModule.IsZoomedOrPanned( ParentChart.ElementId );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override string Name => "Zoom";

    /// <summary>
    /// Reference to the object that should be accessed through JSInterop.
    /// </summary>
    protected DotNetObjectReference<ChartZoomAdapter<TItem>> DotNetObjectRef { get; private set; }

    /// <inheritdoc/>
    protected override JSChartZoomModule JSModule { get; set; }

    /// <summary>
    /// Defines the options for an annotation.
    /// </summary>
    [Parameter] public ChartZoomPluginOptions Options { get; set; }

    /// <summary>
    /// Called while the chart is being zoomed.
    /// </summary>
    [Parameter] public EventCallback<double> Zoomed { get; set; }

    #endregion
}
