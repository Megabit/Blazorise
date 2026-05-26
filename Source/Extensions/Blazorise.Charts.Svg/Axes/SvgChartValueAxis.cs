#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the value axis of a native SVG chart.
/// </summary>
public class SvgChartValueAxis : SvgChartComponentBase
{
    #region Methods

    protected override void Register()
    {
        Parent?.RegisterValueAxis( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterValueAxis( this );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the axis identifier used by series to target this value axis.
    /// </summary>
    [Parameter] public string Id { get; set; }

    /// <summary>
    /// Defines the axis position.
    /// </summary>
    [Parameter] public SvgChartAxisPosition Position { get; set; } = SvgChartAxisPosition.Auto;

    /// <summary>
    /// Defines whether the value axis includes zero.
    /// </summary>
    [Parameter] public bool BeginAtZero { get; set; } = true;

    /// <summary>
    /// Defines a custom minimum value.
    /// </summary>
    [Parameter] public double? Min { get; set; }

    /// <summary>
    /// Defines a custom maximum value.
    /// </summary>
    [Parameter] public double? Max { get; set; }

    /// <summary>
    /// Defines the number of axis ticks.
    /// </summary>
    [Parameter] public int TickCount { get; set; } = 5;

    /// <summary>
    /// Defines whether compatible series are stacked on this axis.
    /// </summary>
    [Parameter] public bool Stacked { get; set; }

    /// <summary>
    /// Defines a callback used to format value axis tick labels.
    /// </summary>
    [Parameter] public Func<SvgChartAxisTickContext, string> TickFormatter { get; set; }

    /// <summary>
    /// Defines grid line options for this value axis.
    /// </summary>
    [Parameter] public SvgChartGridLinesOptions GridLines { get; set; } = new();

    /// <summary>
    /// Defines the axis title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    #endregion
}