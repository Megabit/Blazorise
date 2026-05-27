namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a polar area series for a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgPolarAreaSeries<TItem> : SvgChartSeries<TItem>
{
    #region Properties

    internal override SvgChartType ChartType => SvgChartType.PolarArea;

    #endregion
}