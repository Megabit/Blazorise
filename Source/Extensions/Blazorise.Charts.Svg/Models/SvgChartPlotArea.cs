namespace Blazorise.Charts.Svg;

internal sealed class SvgChartPlotArea
{
    #region Properties

    public double Left { get; init; }

    public double Top { get; init; }

    public double Right { get; init; }

    public double Bottom { get; init; }

    public double Width => Right - Left;

    public double Height => Bottom - Top;

    #endregion
}