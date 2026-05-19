#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a data series for a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public abstract class SvgChartSeries<TItem> : SvgChartComponentBase
{
    #region Methods

    protected override void Register()
    {
        Parent?.RegisterSeries( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterSeries( this );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the series name.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Defines explicit series values.
    /// </summary>
    [Parameter] public IReadOnlyList<double?> Values { get; set; }

    /// <summary>
    /// Defines a selector used to read values from chart items.
    /// </summary>
    [Parameter] public Func<TItem, double?> Value { get; set; }

    /// <summary>
    /// Defines a selector used to read X values from chart items for point-based chart types.
    /// </summary>
    [Parameter] public Func<TItem, double?> XValue { get; set; }

    /// <summary>
    /// Defines a selector used to read Y values from chart items for point-based chart types.
    /// </summary>
    [Parameter] public Func<TItem, double?> YValue { get; set; }

    /// <summary>
    /// Defines a selector used to read radius values from chart items for bubble chart types.
    /// </summary>
    [Parameter] public Func<TItem, double?> RadiusValue { get; set; }

    /// <summary>
    /// Defines the category axis identifier used by this series.
    /// </summary>
    [Parameter] public string CategoryAxisId { get; set; }

    /// <summary>
    /// Defines the value axis identifier used by this series.
    /// </summary>
    [Parameter] public string ValueAxisId { get; set; }

    /// <summary>
    /// Defines the series color. Use a Blazorise theme color, or pass a CSS color value such as <c>#4c6ef5</c>, <c>rgb(76, 110, 245)</c>, <c>hsl(228 88% 60%)</c>, or <c>var(--chart-color)</c>.
    /// </summary>
    [Parameter] public Color Color { get; set; }

    /// <summary>
    /// Defines whether the series is hidden.
    /// </summary>
    [Parameter] public bool Hidden { get; set; }

    /// <summary>
    /// Defines the series rendering order. Lower values are rendered first, behind higher values.
    /// </summary>
    [Parameter] public int? Order { get; set; }

    internal abstract SvgChartType ChartType { get; }

    #endregion
}