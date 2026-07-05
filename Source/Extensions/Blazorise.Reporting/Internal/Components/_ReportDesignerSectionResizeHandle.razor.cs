#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the resize handle used to change a band height with the mouse.
/// </summary>
public partial class _ReportDesignerSectionResizeHandle
{
    private const string Key = "section-resize";

    /// <summary>
    /// Raised when pointer resizing starts on the section handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> PointerDown { get; set; }
}