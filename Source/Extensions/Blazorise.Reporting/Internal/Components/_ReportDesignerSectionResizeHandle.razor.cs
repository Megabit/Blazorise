#region Using directives
using System;
using System.Threading.Tasks;
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

    private Task OnPointerDown( PointerEventArgs eventArgs )
    {
        if ( PointerDown is not null )
            return PointerDown.Invoke( eventArgs );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Raised when pointer resizing starts on the section handle.
    /// </summary>
    [Parameter] public Func<PointerEventArgs, Task> PointerDown { get; set; }
}