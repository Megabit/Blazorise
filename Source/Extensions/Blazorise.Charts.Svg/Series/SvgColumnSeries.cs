#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a column series for a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgColumnSeries<TItem> : SvgChartSeries<TItem>
{
    #region Properties

    /// <summary>
    /// Defines the column border radius in SVG units.
    /// </summary>
    [Parameter] public double BorderRadius { get; set; } = 3;

    internal override SvgChartType ChartType => SvgChartType.Column;

    #endregion
}