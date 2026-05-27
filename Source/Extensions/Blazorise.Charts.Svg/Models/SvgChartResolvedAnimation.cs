namespace Blazorise.Charts.Svg;

internal sealed class SvgChartResolvedAnimation
{
    #region Properties

    public bool Enabled { get; set; }

    public bool InitialRender { get; set; }

    public int Version { get; set; }

    public SvgChartResolvedGeometryAnimation Geometry { get; set; } = new();

    public SvgChartResolvedOpacityAnimation Opacity { get; set; } = new();

    public SvgChartResolvedAnimationTarget Stroke { get; set; } = new();

    public SvgChartResolvedAnimationTarget Transform { get; set; } = new();

    public SvgChartResolvedAnimationTarget Path { get; set; } = new();

    #endregion
}