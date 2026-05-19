#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Renders a native SVG pie chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SvgPieChart<TItem> : SvgChart<TItem>
{
    #region Constructors

    public SvgPieChart()
    {
        Type = SvgChartType.Pie;
    }

    #endregion
}