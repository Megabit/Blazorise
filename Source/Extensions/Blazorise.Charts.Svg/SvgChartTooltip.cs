#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines tooltip behavior for a native SVG chart.
/// </summary>
public class SvgChartTooltip : SvgChartComponentBase
{
    #region Methods

    protected override void Register()
    {
        Parent?.RegisterTooltip( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterTooltip( this );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether SVG chart tooltips are shown for points.
    /// </summary>
    [Parameter] public bool Enabled { get; set; } = true;

    /// <summary>
    /// Defines a callback used to format default tooltip text.
    /// </summary>
    [Parameter] public Func<SvgChartTooltipContext, string> Formatter { get; set; }

    /// <summary>
    /// Defines custom tooltip content.
    /// </summary>
    [Parameter] public RenderFragment<SvgChartTooltipContext> Template { get; set; }

    /// <summary>
    /// Defines the tooltip width in SVG viewport units.
    /// </summary>
    [Parameter] public double Width { get; set; } = 180;

    /// <summary>
    /// Defines the tooltip height in SVG viewport units.
    /// </summary>
    [Parameter] public double Height { get; set; } = 56;

    /// <summary>
    /// Defines the horizontal tooltip offset from the point anchor.
    /// </summary>
    [Parameter] public double OffsetX { get; set; } = 8;

    /// <summary>
    /// Defines the vertical tooltip offset from the point anchor.
    /// </summary>
    [Parameter] public double OffsetY { get; set; } = 8;

    #endregion
}