namespace Blazorise.Charts.Svg;

internal sealed class SvgChartDataDragState
{
    #region Properties

    public int SeriesIndex { get; init; }

    public int PointIndex { get; init; }

    public string SeriesName { get; init; }

    public SvgChartType SeriesType { get; init; }

    public object Category { get; init; }

    public bool CategoryTracksX { get; init; }

    public bool CanDragX { get; init; }

    public bool CanDragY { get; init; }

    public bool IsValueAxisHorizontal { get; init; }

    public bool IsStacked { get; init; }

    public bool ShouldShowTooltip { get; init; }

    public double? OriginalXValue { get; init; }

    public double? OriginalYValue { get; init; }

    public double? XValue { get; set; }

    public double? YValue { get; set; }

    public double PositiveStackBaseValue { get; init; }

    public double NegativeStackBaseValue { get; init; }

    public double CrossAxisCoordinate { get; init; }

    public double RadialPointerOffset { get; init; }

    public double AngularPrefixValue { get; init; }

    public double AngularOtherValue { get; init; }

    public bool UsesAngularStartBoundary { get; init; }

    public double AngularBoundaryFraction { get; set; }

    public double AngularPointerOffset { get; init; }

    public SvgChartDataDragMode Mode { get; init; }

    public SvgChartPlotArea Plot { get; init; }

    public SvgChartScale XScale { get; init; }

    public SvgChartRenderValueAxis ValueAxis { get; init; }

    public SvgChartDataPointOverride OriginalOverride { get; init; }

    #endregion
}