#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Renders a native SVG line chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SvgLineChart<TItem> : SvgChart<TItem>
{
    #region Constructors

    public SvgLineChart()
    {
        Type = SvgChartType.Line;
    }

    #endregion
}