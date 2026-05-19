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
    /// Defines the title text.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Defines the subtitle text.
    /// </summary>
    [Parameter] public string Subtitle { get; set; }

    #endregion
}