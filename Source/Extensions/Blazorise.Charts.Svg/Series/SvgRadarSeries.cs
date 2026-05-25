#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a radar series for a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgRadarSeries<TItem> : SvgChartSeries<TItem>
{
    #region Properties

    /// <summary>
    /// Defines the radar polygon fill opacity.
    /// </summary>
    [Parameter] public double FillOpacity { get; set; } = 0.16;

    internal override SvgChartType ChartType => SvgChartType.Radar;

    #endregion
}