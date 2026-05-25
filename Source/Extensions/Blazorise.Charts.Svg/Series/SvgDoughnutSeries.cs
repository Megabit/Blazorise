namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a doughnut series for a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgDoughnutSeries<TItem> : SvgChartSeries<TItem>
{
    #region Properties

    internal override SvgChartType ChartType => SvgChartType.Doughnut;

    #endregion
}