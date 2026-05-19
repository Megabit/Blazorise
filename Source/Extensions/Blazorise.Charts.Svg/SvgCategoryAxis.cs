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
public class SvgCategoryAxis<TItem> : SvgChartComponentBase
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

    #endregion
}