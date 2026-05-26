#region Using directives
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a plugin that can render additional SVG content inside a native SVG chart.
/// </summary>
public interface ISvgChartPlugin
{
    #region Methods

    /// <summary>
    /// Renders the plugin SVG content into the chart.
    /// </summary>
    /// <param name="context">The chart plugin render context.</param>
    /// <param name="builder">The render tree builder.</param>
    /// <param name="sequence">The current render sequence.</param>
    void Render( SvgChartPluginRenderContext context, RenderTreeBuilder builder, ref int sequence );

    #endregion

    #region Properties

    /// <summary>
    /// Gets the chart render layer used by the plugin.
    /// </summary>
    SvgChartRenderLayer Layer { get; }

    /// <summary>
    /// Gets whether the plugin renders SVG content directly.
    /// </summary>
    bool RendersContent { get; }

    #endregion
}