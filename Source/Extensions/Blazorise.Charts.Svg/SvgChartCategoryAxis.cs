#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the category axis of a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgChartCategoryAxis<TItem> : SvgChartComponentBase
{
    #region Methods

    protected override void Register()
    {
        Parent?.RegisterCategoryAxis( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterCategoryAxis( this );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the axis identifier used by series to target this category axis.
    /// </summary>
    [Parameter] public string Id { get; set; }

    /// <summary>
    /// Defines the axis position.
    /// </summary>
    [Parameter] public SvgChartAxisPosition Position { get; set; } = SvgChartAxisPosition.Auto;

    /// <summary>
    /// Defines explicit category labels.
    /// </summary>
    [Parameter] public IReadOnlyList<object> Labels { get; set; }

    /// <summary>
    /// Defines a selector used to read category labels from chart items.
    /// </summary>
    [Parameter] public Func<TItem, object> Value { get; set; }

    /// <summary>
    /// Defines the axis title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Defines grid line options for this category axis.
    /// </summary>
    [Parameter] public SvgChartGridLinesOptions GridLines { get; set; }

    /// <summary>
    /// Defines label options for this category axis.
    /// </summary>
    [Parameter] public SvgChartAxisLabelsOptions LabelsOptions { get; set; }

    #endregion
}