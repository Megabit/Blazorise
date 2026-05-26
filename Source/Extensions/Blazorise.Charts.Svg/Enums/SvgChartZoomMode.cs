namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines which chart axis can be zoomed or panned.
/// </summary>
public enum SvgChartZoomMode
{
    #region Values

    /// <summary>
    /// Zooms or pans along the horizontal axis.
    /// </summary>
    X,

    /// <summary>
    /// Zooms or pans along the vertical axis.
    /// </summary>
    Y,

    /// <summary>
    /// Zooms or pans along both horizontal and vertical axes.
    /// </summary>
    XY

    #endregion
}