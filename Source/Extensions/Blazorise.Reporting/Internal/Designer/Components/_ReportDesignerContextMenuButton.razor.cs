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
/// Renders one command inside the report designer context menu.
/// </summary>
public partial class _ReportDesignerContextMenuButton
{
    /// <summary>
    /// Context menu button text.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Raised when the context menu button is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }
}