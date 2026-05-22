#region Using directives
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Base class for native SVG chart plugins.
/// </summary>
public abstract class SvgChartPluginBase : SvgChartComponentBase, ISvgChartPlugin
{
    #region Methods

    protected override void Register()
    {
        Parent?.RegisterPlugin( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterPlugin( this );
    }

    /// <summary>
    /// Renders the plugin SVG content into the chart.
    /// </summary>
    /// <param name="context">The chart plugin render context.</param>
    /// <param name="builder">The render tree builder.</param>
    /// <param name="sequence">The current render sequence.</param>
    public virtual void Render( SvgChartPluginRenderContext context, RenderTreeBuilder builder, ref int sequence )
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the chart render layer used by the plugin.
    /// </summary>
    public virtual SvgChartRenderLayer Layer => SvgChartRenderLayer.SeriesOverlay;

    /// <inheritdoc/>
    public virtual bool RendersContent => true;

    #endregion
}