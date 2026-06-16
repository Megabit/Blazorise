#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders one icon command inside the report designer context menu toolbar.
/// </summary>
public partial class _ReportDesignerContextMenuIconButton
{
    #region Properties

    /// <summary>
    /// Icon shown inside the command button.
    /// </summary>
    [Parameter] public IconName Icon { get; set; }

    /// <summary>
    /// Command text used as the button tooltip.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Disables the command button when the command is not currently available.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Raised when the context menu command is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    #endregion
}