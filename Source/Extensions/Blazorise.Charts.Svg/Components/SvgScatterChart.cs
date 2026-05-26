#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Renders a native SVG scatter chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SvgScatterChart<TItem> : SvgChart<TItem>
{
    #region Constructors

    public SvgScatterChart()
    {
        Type = SvgChartType.Scatter;
    }

    #endregion
}