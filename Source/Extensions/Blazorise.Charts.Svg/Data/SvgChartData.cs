#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Represents the data used by a native SVG chart.
/// </summary>
/// <typeparam name="TValue">The chart value type.</typeparam>
public class SvgChartData<TValue>
{
    #region Properties

    /// <summary>
    /// Defines category labels shown on the index axis.
    /// </summary>
    public List<object> Labels { get; set; } = [];

    /// <summary>
    /// Defines the chart series.
    /// </summary>
    public List<SvgChartSeriesData<TValue>> Series { get; set; } = [];

    #endregion
}