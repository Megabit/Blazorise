#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the resize handles shown around a selected report element.
/// </summary>
public partial class _ReportDesignerElementResizeHandles
{
    private const string NorthWestKey = "nw";
    private const string NorthKey = "n";
    private const string NorthEastKey = "ne";
    private const string EastKey = "e";
    private const string SouthEastKey = "se";
    private const string SouthKey = "s";
    private const string SouthWestKey = "sw";
    private const string WestKey = "w";

    private Task NorthWestPointerDown( PointerEventArgs eventArgs )
        => OnPointerDown( ReportElementResizeHandle.NorthWest, eventArgs );

    private Task NorthPointerDown( PointerEventArgs eventArgs )
        => OnPointerDown( ReportElementResizeHandle.North, eventArgs );

    private Task NorthEastPointerDown( PointerEventArgs eventArgs )
        => OnPointerDown( ReportElementResizeHandle.NorthEast, eventArgs );

    private Task EastPointerDown( PointerEventArgs eventArgs )
        => OnPointerDown( ReportElementResizeHandle.East, eventArgs );

    private Task SouthEastPointerDown( PointerEventArgs eventArgs )
        => OnPointerDown( ReportElementResizeHandle.SouthEast, eventArgs );

    private Task SouthPointerDown( PointerEventArgs eventArgs )
        => OnPointerDown( ReportElementResizeHandle.South, eventArgs );

    private Task SouthWestPointerDown( PointerEventArgs eventArgs )
        => OnPointerDown( ReportElementResizeHandle.SouthWest, eventArgs );

    private Task WestPointerDown( PointerEventArgs eventArgs )
        => OnPointerDown( ReportElementResizeHandle.West, eventArgs );

    private Task OnPointerDown( ReportElementResizeHandle handle, PointerEventArgs eventArgs )
    {
        return PointerDown.InvokeAsync( new( handle, eventArgs ) );
    }

    /// <summary>
    /// Raised when resizing starts on one of the element resize handles.
    /// </summary>
    [Parameter] public EventCallback<ReportElementResizeHandleEventArgs> PointerDown { get; set; }
}
