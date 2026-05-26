#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines tooltip options for a native SVG chart.
/// </summary>
public class SvgChartTooltipOptions
{
    #region Properties

    /// <summary>
    /// Defines whether SVG chart tooltips are shown for points.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Defines how related points are resolved for tooltip content.
    /// </summary>
    public SvgChartInteractionMode InteractionMode { get; set; } = SvgChartInteractionMode.Nearest;

    /// <summary>
    /// Defines a callback used to format default tooltip text.
    /// </summary>
    public Func<SvgChartTooltipContext, string> Formatter { get; set; }

    /// <summary>
    /// Defines custom tooltip content.
    /// </summary>
    public RenderFragment<SvgChartTooltipContext> Template { get; set; }

    /// <summary>
    /// Defines the tooltip width in SVG viewport units.
    /// </summary>
    public double Width { get; set; } = 180;

    /// <summary>
    /// Defines the tooltip height in SVG viewport units.
    /// </summary>
    public double Height { get; set; } = 56;

    /// <summary>
    /// Defines the horizontal tooltip offset from the point anchor.
    /// </summary>
    public double OffsetX { get; set; } = 8;

    /// <summary>
    /// Defines the vertical tooltip offset from the point anchor.
    /// </summary>
    public double OffsetY { get; set; } = 8;

    #endregion
}