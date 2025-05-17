#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// Middleman class for the ChartZoom plugin.
/// </summary>
public class ChartZoomAdapter<TItem>
{
    #region Members

    private readonly ChartZoom<TItem> chartZoom;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartZoomAdapter{TItem}"/> class with the specified chart zoom
    /// functionality.
    /// </summary>
    /// <param name="chartZoom">The <see cref="ChartZoom{TItem}"/> instance that provides zooming functionality for the chart. Cannot be <see
    /// langword="null"/>.</param>
    public ChartZoomAdapter( ChartZoom<TItem> chartZoom )
    {
        this.chartZoom = chartZoom;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Notifies the system of a change in the zoom level.
    /// </summary>
    /// <param name="zoomLevel">The new zoom level, represented as a double. Must be a positive value.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    [JSInvokable]
    public async ValueTask NotifyZoomLevel( double zoomLevel )
    {
        await chartZoom.Zoomed.InvokeAsync( zoomLevel );
    }

    #endregion
}
