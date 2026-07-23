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

    private DockContent Content => Context?.Content;

    [CascadingParameter] internal DockLayoutContext Context { get; set; }

    /// <summary>
    /// Identifies the rendered content node.
    /// </summary>
    [Parameter] public string NodeId { get; set; }

    #endregion
}