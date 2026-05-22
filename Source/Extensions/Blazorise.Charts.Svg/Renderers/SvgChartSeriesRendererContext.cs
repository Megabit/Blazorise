#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartSeriesRendererContext
{
    #region Constructors

    public SvgChartSeriesRendererContext( SvgChartPluginRenderContext chart )
    {
        Chart = chart;
    }

    #endregion

    #region Methods

    public void AddPointInteractionAttributes( RenderTreeBuilder builder, ref int sequence, SvgChartPointEventArgs point, string color )
    {
        builder.AddAttribute( sequence++, "tabindex", "0" );
        builder.AddAttribute( sequence++, "role", "img" );
        builder.AddAttribute( sequence++, "aria-label", GetPointLabel( point ) );
        builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( Chart.EventReceiver, () => Chart.NotifyPointClicked( point, color ) ) );
        builder.AddAttribute( sequence++, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>( Chart.EventReceiver, () => Chart.NotifyPointHovered( point, color ) ) );
        builder.AddAttribute( sequence++, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>( Chart.EventReceiver, Chart.NotifyPointLeft ) );
        builder.AddAttribute( sequence++, "onfocus", EventCallback.Factory.Create<FocusEventArgs>( Chart.EventReceiver, () => Chart.ShowTooltip( point, color, false ) ) );
        builder.AddAttribute( sequence++, "onblur", EventCallback.Factory.Create<FocusEventArgs>( Chart.EventReceiver, Chart.NotifyPointLeft ) );
    }

    public string GetPointLabel( SvgChartPointEventArgs point )
    {
        return $"{point.Category}, {point.Value}. {point.SeriesName}.";
    }

    public string ResolveColor( int index )
    {
        return SvgChartRenderHelpers.ResolveColor( null, index );
    }

    #endregion

    #region Properties

    public SvgChartPluginRenderContext Chart { get; }

    #endregion
}