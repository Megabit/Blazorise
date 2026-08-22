#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines data point dragging options for a native SVG chart.
/// </summary>
public class SvgChartDataDragOptions
{
    #region Properties

    /// <summary>
    /// Defines whether data point dragging is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Defines the axes along which data points can be dragged.
    /// </summary>
    public SvgChartDataDragMode Mode { get; set; } = SvgChartDataDragMode.Y;

    /// <summary>
    /// Defines the optional increment to which dragged X values are snapped.
    /// </summary>
    public double? XStep { get; set; }

    /// <summary>
    /// Defines the optional increment to which dragged Y values are snapped.
    /// </summary>
    public double? YStep { get; set; }

    /// <summary>
    /// Defines the minimum pointer hit radius in SVG units.
    /// </summary>
    public double HitRadius { get; set; } = 12;

    /// <summary>
    /// Defines whether the point tooltip is shown and updated while dragging. The default is <see langword="false"/>.
    /// </summary>
    public bool ShowTooltip { get; set; }

    /// <summary>
    /// Defines an optional predicate that determines whether an individual point can be dragged.
    /// </summary>
    public Func<SvgChartPointEventArgs, bool> CanDrag { get; set; }

    #endregion
}