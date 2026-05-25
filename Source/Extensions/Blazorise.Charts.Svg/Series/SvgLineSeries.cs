#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a line series for a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgLineSeries<TItem> : SvgChartSeries<TItem>
{
    #region Properties

    /// <summary>
    /// Defines the line stroke width.
    /// </summary>
    [Parameter] public double StrokeWidth { get; set; } = 2;

    /// <summary>
    /// Defines the marker radius.
    /// </summary>
    [Parameter] public double MarkerRadius { get; set; } = 3;

    internal override SvgChartType ChartType => SvgChartType.Line;

    #endregion
}