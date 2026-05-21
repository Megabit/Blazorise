namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the source that changed the chart viewport.
/// </summary>
public enum SvgChartZoomSource
{
    #region Values

    /// <summary>
    /// The viewport changed through an imperative API.
    /// </summary>
    Api,

    /// <summary>
    /// The viewport changed through a mouse wheel gesture.
    /// </summary>
    Wheel

    #endregion
}