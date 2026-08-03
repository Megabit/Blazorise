#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Renders a native SVG doughnut chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SvgDoughnutChart<TItem> : SvgChart<TItem>
{
    #region Constructors

    /// <summary>
    /// Creates a circular chart with a hollow center.
    /// </summary>
    public SvgDoughnutChart()
    {
        Type = SvgChartType.Doughnut;
    }

    #endregion
}