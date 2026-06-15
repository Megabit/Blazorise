#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a nested group of report designer context menu commands.
/// </summary>
public partial class _ReportDesignerContextSubMenu
{
    #region Properties

    /// <summary>
    /// Context submenu label.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Nested context menu commands.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}