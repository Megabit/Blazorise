#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Provides information about a chart pan action.
/// </summary>
public class SvgChartPannedEventArgs : EventArgs
{
    #region Properties

    /// <summary>
    /// Gets the viewport before the pan action.
    /// </summary>
    public SvgChartViewport PreviousViewport { get; init; }

    /// <summary>
    /// Gets the viewport after the pan action.
    /// </summary>
    public SvgChartViewport Viewport { get; init; }

    /// <summary>
    /// Gets the horizontal pan delta in axis units.
    /// </summary>
    public double DeltaX { get; init; }

    /// <summary>
    /// Gets the vertical pan delta in axis units.
    /// </summary>
    public double DeltaY { get; init; }

    #endregion
}