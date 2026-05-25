#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a scatter series for a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgScatterSeries<TItem> : SvgChartSeries<TItem>
{
    #region Properties

    /// <summary>
    /// Defines the scatter point radius.
    /// </summary>
    [Parameter] public double MarkerRadius { get; set; } = 4;

    internal override SvgChartType ChartType => SvgChartType.Scatter;

    #endregion
}