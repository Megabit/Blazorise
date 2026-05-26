#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines declarative zoom and pan behavior for a native SVG chart.
/// </summary>
public class SvgChartZoom : SvgChartPluginBase
{
    #region Properties

    /// <summary>
    /// Defines whether zoom and pan behavior is enabled.
    /// </summary>
    [Parameter] public bool Enabled { get; set; }

    /// <summary>
    /// Defines which chart axis can be zoomed or panned.
    /// </summary>
    [Parameter] public SvgChartZoomMode Mode { get; set; } = SvgChartZoomMode.X;

    /// <summary>
    /// Defines whether mouse wheel zoom is enabled.
    /// </summary>
    [Parameter] public bool Wheel { get; set; } = true;

    /// <summary>
    /// Defines whether drag panning is enabled.
    /// </summary>
    [Parameter] public bool Pan { get; set; } = true;

    /// <summary>
    /// Defines the minimum zoom factor relative to the full data range.
    /// </summary>
    [Parameter] public double MinZoom { get; set; } = 1;

    /// <summary>
    /// Defines the maximum zoom factor relative to the full data range.
    /// </summary>
    [Parameter] public double MaxZoom { get; set; } = 20;

    /// <summary>
    /// Defines the current visible viewport.
    /// </summary>
    [Parameter] public SvgChartViewport Viewport { get; set; }

    /// <summary>
    /// Occurs when the visible viewport changes.
    /// </summary>
    [Parameter] public EventCallback<SvgChartViewport> ViewportChanged { get; set; }

    /// <summary>
    /// Occurs after the chart is zoomed.
    /// </summary>
    [Parameter] public EventCallback<SvgChartZoomedEventArgs> Zoomed { get; set; }

    /// <summary>
    /// Occurs after the chart is panned.
    /// </summary>
    [Parameter] public EventCallback<SvgChartPannedEventArgs> Panned { get; set; }

    /// <inheritdoc/>
    public override bool RendersContent => false;

    #endregion
}