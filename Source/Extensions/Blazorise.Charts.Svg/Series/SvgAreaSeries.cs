#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines an area series for a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgAreaSeries<TItem> : SvgChartSeries<TItem>
{
    #region Properties

    /// <summary>
    /// Defines the area line stroke width.
    /// </summary>
    [Parameter] public double StrokeWidth { get; set; } = 2;

    /// <summary>
    /// Defines the area fill opacity.
    /// </summary>
    [Parameter] public double FillOpacity { get; set; } = 0.18;

    /// <summary>
    /// Defines how the area line is interpolated between data points.
    /// </summary>
    [Parameter] public SvgChartInterpolationMode Interpolation { get; set; }

    /// <summary>
    /// Defines the cubic interpolation tension.
    /// </summary>
    [Parameter] public double Tension { get; set; } = 0.4;

    internal override SvgChartType ChartType => SvgChartType.Area;

    #endregion
}