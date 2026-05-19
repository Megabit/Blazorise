#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the value axis of a native SVG chart.
/// </summary>
public class SvgLinearAxis : SvgChartComponentBase
{
    #region Methods

    protected override void Register()
    {
        Parent?.RegisterLinearAxis( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterLinearAxis( this );
    }

    #endregion

    #region Properties

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
    /// Defines the axis title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    #endregion
}