#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Renders a native SVG column chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SvgColumnChart<TItem> : SvgChart<TItem>
{
    #region Constructors

    public SvgColumnChart()
    {
        Type = SvgChartType.Column;
    }

    #endregion
}