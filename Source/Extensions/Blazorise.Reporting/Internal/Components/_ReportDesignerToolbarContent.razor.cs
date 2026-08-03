#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders shared report toolbar content for designer and preview modes.
/// </summary>
public partial class _ReportDesignerToolbarContent
{
    #region Properties

    /// <summary>
    /// Designer controlled by the toolbar.
    /// </summary>
    [Parameter, EditorRequired] public _ReportDesigner Designer { get; set; }

    /// <summary>
    /// Shows the menu for toggling designer panes.
    /// </summary>
    [Parameter] public bool ShowPanesMenu { get; set; }

    #endregion
}