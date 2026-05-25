namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a pie series for a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgPieSeries<TItem> : SvgChartSeries<TItem>
{
    #region Properties

    internal override SvgChartType ChartType => SvgChartType.Pie;

    #endregion
}