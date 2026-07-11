#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Groups related report designer property editors.
/// </summary>
public partial class _ReportDesignerPropertyGroup
{
    #region Properties

    /// <summary>
    /// Group heading text.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Property editor content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}