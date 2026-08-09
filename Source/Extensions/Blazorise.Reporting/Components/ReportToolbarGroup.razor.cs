#region Using directives
using Blazorise;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Groups related report toolbar items into a single visual button group.
/// </summary>
public partial class ReportToolbarGroup
{
    #region Properties

    /// <summary>
    /// Visual role used by the underlying button group.
    /// </summary>
    [Parameter] public ButtonsRole Role { get; set; } = ButtonsRole.Addons;

    /// <summary>
    /// Size applied to all buttons inside the group.
    /// </summary>
    [Parameter] public Size Size { get; set; } = Size.Default;

    /// <summary>
    /// Toolbar items rendered inside this group.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}