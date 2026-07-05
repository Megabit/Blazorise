using Microsoft.AspNetCore.Components.Web;

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Event arguments for pointer actions raised by report element resize handles.
/// </summary>
/// <param name="handle">Resize handle that started the pointer action.</param>
/// <param name="pointerEventArgs">Original Blazor pointer event arguments.</param>
public sealed class ReportElementResizeHandleEventArgs( ReportElementResizeHandle handle, PointerEventArgs pointerEventArgs )
{
    /// <summary>
    /// Resize handle that started the pointer action.
    /// </summary>
    public ReportElementResizeHandle Handle { get; } = handle;

    /// <summary>
    /// Original Blazor pointer event arguments.
    /// </summary>
    public PointerEventArgs PointerEventArgs { get; } = pointerEventArgs;
}
