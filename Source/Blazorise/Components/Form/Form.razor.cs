#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper for a regular html form element.
/// </summary>
public partial class Form : BaseComponent
{
    #region Properties

    /// <summary>
    /// Defines the content rendered inside the form.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
