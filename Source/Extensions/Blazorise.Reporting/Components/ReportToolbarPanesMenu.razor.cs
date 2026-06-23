#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a toolbar dropdown used to show or hide report designer dock panes.
/// </summary>
public partial class ReportToolbarPanesMenu
{
    [CascadingParameter] internal ReportToolbarDockContext DockContext { get; set; }

    private string Text => string.IsNullOrWhiteSpace( Caption ) ? "Panes" : Caption;

    /// <summary>
    /// Text shown for the pane menu.
    /// </summary>
    [Parameter] public string Caption { get; set; } = "Panes";

    /// <summary>
    /// Icon shown for the pane menu.
    /// </summary>
    [Parameter] public IconName Icon { get; set; } = IconName.List;

    /// <summary>
    /// Shows the pane menu caption next to the icon.
    /// </summary>
    [Parameter] public bool ShowCaption { get; set; }

    /// <summary>
    /// Button color used for the pane menu.
    /// </summary>
    [Parameter] public Color Color { get; set; } = Color.Light;
}