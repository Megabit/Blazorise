#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines streaming animation options for a native SVG chart.
/// </summary>
public class SvgChartStreamingAnimationOptions
{
    #region Properties

    /// <summary>
    /// Defines whether the streaming viewport should animate between appended values.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Defines the animation duration used when the streaming viewport advances. When zero, the refresh interval is used.
    /// </summary>
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;

    #endregion
}