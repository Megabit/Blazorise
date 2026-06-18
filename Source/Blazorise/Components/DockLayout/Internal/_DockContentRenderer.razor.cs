#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders the central content of a dock layout tree.
/// </summary>
public partial class _DockContentRenderer : ComponentBase
{
    #region Properties

    private DockContent Content => Layout?.Content;

    /// <summary>
    /// Gets or sets the owner dock layout.
    /// </summary>
    [Parameter] public DockLayout Layout { get; set; }

    #endregion
}