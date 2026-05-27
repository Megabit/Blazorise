#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the legend of a native SVG chart.
/// </summary>
public class SvgChartLegend : SvgChartComponentBase
{
    #region Methods

    protected override void Register()
    {
        Parent?.RegisterLegend( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterLegend( this );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether the legend is visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the legend position.
    /// </summary>
    [Parameter] public SvgChartLegendPosition Position { get; set; } = SvgChartLegendPosition.Bottom;

    #endregion
}