#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a checked boolean command inside the report designer context menu.
/// </summary>
public partial class _ReportDesignerContextMenuCheckButton
{
    #region Properties

    /// <summary>
    /// Context menu command text.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Indicates that the boolean command is currently enabled.
    /// </summary>
    [Parameter] public bool Checked { get; set; }

    /// <summary>
    /// Disables the command button when the command is not currently available.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Raised when the checked command is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    #endregion
}