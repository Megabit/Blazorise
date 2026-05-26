#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Provides information about a chart zoom action.
/// </summary>
public class SvgChartZoomedEventArgs : EventArgs
{
    #region Properties

    /// <summary>
    /// Gets the viewport before the zoom action.
    /// </summary>
    public SvgChartViewport PreviousViewport { get; init; }

    /// <summary>
    /// Gets the viewport after the zoom action.
    /// </summary>
    public SvgChartViewport Viewport { get; init; }

    /// <summary>
    /// Gets the source that caused the zoom action.
    /// </summary>
    public SvgChartZoomSource Source { get; init; }

    #endregion
}