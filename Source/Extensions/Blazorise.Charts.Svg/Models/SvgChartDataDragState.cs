namespace Blazorise.Charts.Svg;

internal sealed class SvgChartDataDragState
{
    #region Properties

    public int SeriesIndex { get; init; }

    public int PointIndex { get; init; }

    public string SeriesName { get; init; }

    public object Category { get; init; }

    public bool CategoryTracksX { get; init; }

    public bool CanDragX { get; init; }

    public bool CanDragY { get; init; }

    public bool ShouldShowTooltip { get; init; }

    public double? OriginalXValue { get; init; }

    public double? OriginalYValue { get; init; }

    public double? XValue { get; set; }

    public double? YValue { get; set; }

    public SvgChartDataDragMode Mode { get; init; }

    public SvgChartPlotArea Plot { get; init; }

    public SvgChartScale XScale { get; init; }

    public SvgChartRenderValueAxis ValueAxis { get; init; }

    public SvgChartDataPointOverride OriginalOverride { get; init; }

    #endregion
}