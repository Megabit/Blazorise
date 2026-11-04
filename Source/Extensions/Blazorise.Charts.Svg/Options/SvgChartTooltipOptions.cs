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
    /// Defines whether the pointer must intersect a rendered data point to show the tooltip.
    /// When false, the nearest category is resolved from the plot area for cartesian charts.
    /// </summary>
    public bool Intersect { get; set; } = true;

    /// <summary>
    /// Defines a callback used to format default tooltip text.
    /// </summary>
    public Func<SvgChartTooltipContext, string> Formatter { get; set; }

    /// <summary>
    /// Defines custom tooltip content.
    /// </summary>
    public RenderFragment<SvgChartTooltipContext> Template { get; set; }

    /// <summary>
    /// Defines the configured tooltip width in CSS pixels. This is a minimum when automatic sizing is enabled.
    /// </summary>
    public double Width { get; set; } = 180;

    /// <summary>
    /// Defines the configured tooltip height in CSS pixels. This is a minimum when automatic sizing is enabled.
    /// </summary>
    public double Height { get; set; } = 56;

    /// <summary>
    /// Defines whether the tooltip grows to fit its content. Width and Height become minimum sizes.
    /// Set to false to retain fixed dimensions and clipped content.
    /// </summary>
    public bool AutoSize { get; set; } = true;

    /// <summary>
    /// Defines the tooltip font. Unspecified values inherit the chart font.
    /// </summary>
    public SvgChartFontOptions Font { get; set; }

    /// <summary>
    /// Defines content padding in CSS pixels. When null, uses the tooltip theme padding.
    /// </summary>
    public SvgChartSpacing Padding { get; set; }

    /// <summary>
    /// Defines the horizontal tooltip offset from the point anchor in SVG viewport units at the unscaled chart size. The rendered offset is unaffected by chart scaling.
    /// </summary>
    public double OffsetX { get; set; } = 8;

    /// <summary>
    /// Defines the vertical tooltip offset from the point anchor in SVG viewport units at the unscaled chart size. The rendered offset is unaffected by chart scaling.
    /// </summary>
    public double OffsetY { get; set; } = 8;

    #endregion
}