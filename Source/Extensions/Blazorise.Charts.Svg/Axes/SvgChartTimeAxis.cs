#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a time axis of a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgChartTimeAxis<TItem> : SvgChartCategoryAxis<TItem>
{
    #region Properties

    /// <summary>
    /// Defines a selector used to read time values from chart items.
    /// </summary>
    [Parameter] public Func<TItem, DateTime?> TimeValue { get; set; }

    /// <summary>
    /// Defines the preferred unit used to format time labels.
    /// </summary>
    [Parameter] public SvgChartTimeUnit Unit { get; set; } = SvgChartTimeUnit.Auto;

    /// <summary>
    /// Defines how time values are mapped on the axis.
    /// </summary>
    [Parameter] public SvgChartTimeScale Scale { get; set; } = SvgChartTimeScale.Ordinal;

    /// <summary>
    /// Defines the time label format. When not set, the format is inferred from <see cref="Unit"/>.
    /// </summary>
    [Parameter] public string Format { get; set; }

    /// <summary>
    /// Defines the culture name used to format time labels.
    /// </summary>
    [Parameter] public string Culture { get; set; }

    #endregion
}