#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the title of a native SVG chart.
/// </summary>
public class SvgChartTitle : SvgChartComponentBase
{
    #region Methods

    protected override void Register()
    {
        Parent?.RegisterTitle( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterTitle( this );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines title options.
    /// </summary>
    [Parameter] public SvgChartTextOptions Title { get; set; }

    /// <summary>
    /// Defines subtitle options.
    /// </summary>
    [Parameter] public SvgChartTextOptions Subtitle { get; set; }

    #endregion
}