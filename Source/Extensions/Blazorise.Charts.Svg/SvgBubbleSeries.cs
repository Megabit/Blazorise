#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a bubble series for a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgBubbleSeries<TItem> : SvgChartSeries<TItem>
{
    #region Properties

    /// <summary>
    /// Defines the fallback bubble radius when no radius selector is provided.
    /// </summary>
    [Parameter] public double Radius { get; set; } = 6;

    internal override SvgChartType ChartType => SvgChartType.Bubble;

    #endregion
}