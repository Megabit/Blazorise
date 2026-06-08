#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
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

    /// <summary>
    /// Raised when resizing starts from the north-west handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> NorthWestPointerDown { get; set; }

    /// <summary>
    /// Raised when resizing starts from the north handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> NorthPointerDown { get; set; }

    /// <summary>
    /// Raised when resizing starts from the north-east handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> NorthEastPointerDown { get; set; }

    /// <summary>
    /// Raised when resizing starts from the east handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> EastPointerDown { get; set; }

    /// <summary>
    /// Raised when resizing starts from the south-east handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> SouthEastPointerDown { get; set; }

    /// <summary>
    /// Raised when resizing starts from the south handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> SouthPointerDown { get; set; }

    /// <summary>
    /// Raised when resizing starts from the south-west handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> SouthWestPointerDown { get; set; }

    /// <summary>
    /// Raised when resizing starts from the west handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> WestPointerDown { get; set; }
}