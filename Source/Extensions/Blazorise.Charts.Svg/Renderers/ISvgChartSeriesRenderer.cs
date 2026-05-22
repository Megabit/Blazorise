#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal interface ISvgChartSeriesRenderer
{
    #region Methods

    bool CanRender( SvgChartPluginSeries series );

    int GetRenderOrder( SvgChartPluginSeries series );

    void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence );

    #endregion
}