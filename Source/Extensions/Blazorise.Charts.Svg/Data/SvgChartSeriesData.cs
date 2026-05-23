#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Represents one data series in a native SVG chart.
/// </summary>
/// <typeparam name="TValue">The chart value type.</typeparam>
public class SvgChartSeriesData<TValue>
{
    #region Properties

    /// <summary>
    /// Defines the series name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Defines the series values.
    /// </summary>
    public List<TValue> Values { get; set; } = [];

    /// <summary>
    /// Defines explicit X values for point-based chart types.
    /// </summary>
    public List<double?> XValues { get; set; } = [];

    /// <summary>
    /// Defines explicit Y values for point-based chart types.
    /// </summary>
    public List<double?> YValues { get; set; } = [];

    /// <summary>
    /// Defines explicit radius values for bubble chart types.
    /// </summary>
    public List<double?> RadiusValues { get; set; } = [];

    /// <summary>
    /// Defines the category axis identifier used by this series.
    /// </summary>
    public string CategoryAxisId { get; set; }

    /// <summary>
    /// Defines the value axis identifier used by this series.
    /// </summary>
    public string ValueAxisId { get; set; }

    /// <summary>
    /// Defines the stack group used to stack compatible series.
    /// </summary>
    public string Stack { get; set; }

    /// <summary>
    /// Defines the series color. Use a Blazorise theme color, or pass a CSS color value such as <c>#4c6ef5</c>, <c>rgb(76, 110, 245)</c>, <c>hsl(228 88% 60%)</c>, or <c>var(--chart-color)</c>.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Defines whether the series is hidden.
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// Defines the series rendering order. Lower values are rendered first, behind higher values.
    /// </summary>
    public int? Order { get; set; }

    #endregion
}