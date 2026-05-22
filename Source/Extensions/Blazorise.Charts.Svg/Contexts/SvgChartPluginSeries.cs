#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a resolved chart series for native SVG chart plugins.
/// </summary>
public sealed class SvgChartPluginSeries
{
    #region Properties

    /// <summary>
    /// Gets the series name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the series chart type.
    /// </summary>
    public SvgChartType Type { get; init; }

    /// <summary>
    /// Gets the series values.
    /// </summary>
    public IReadOnlyList<double?> Values { get; init; } = [];

    /// <summary>
    /// Gets the series X values.
    /// </summary>
    public IReadOnlyList<double?> XValues { get; init; } = [];

    /// <summary>
    /// Gets the series Y values.
    /// </summary>
    public IReadOnlyList<double?> YValues { get; init; } = [];

    /// <summary>
    /// Gets the series radius values.
    /// </summary>
    public IReadOnlyList<double?> RadiusValues { get; init; } = [];

    /// <summary>
    /// Gets the resolved series color.
    /// </summary>
    public string Color { get; init; }

    /// <summary>
    /// Gets whether the series is hidden.
    /// </summary>
    public bool Hidden { get; init; }

    /// <summary>
    /// Gets the resolved series render order.
    /// </summary>
    public int? Order { get; init; }

    /// <summary>
    /// Gets the category axis identifier used by the series.
    /// </summary>
    public string CategoryAxisId { get; init; }

    /// <summary>
    /// Gets the value axis identifier used by the series.
    /// </summary>
    public string ValueAxisId { get; init; }

    /// <summary>
    /// Gets the resolved bar or column border radius.
    /// </summary>
    public double BorderRadius { get; init; }

    /// <summary>
    /// Gets the resolved line or area stroke width.
    /// </summary>
    public double StrokeWidth { get; init; }

    /// <summary>
    /// Gets the resolved marker radius.
    /// </summary>
    public double MarkerRadius { get; init; }

    /// <summary>
    /// Gets the resolved area fill opacity.
    /// </summary>
    public double FillOpacity { get; init; }

    #endregion
}