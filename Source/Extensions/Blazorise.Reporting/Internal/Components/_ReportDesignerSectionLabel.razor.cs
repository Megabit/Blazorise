#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the band label used by the classic report designer surface.
/// </summary>
public partial class _ReportDesignerSectionLabel
{
    private Func<MouseEventArgs, Task> NonRenderingContextMenu => EventUtil.AsNonRenderingEventHandler<MouseEventArgs>( OnContextMenuAsync );

    private string Label => $"{ReportDefinitionHelper.GetSectionTypeDisplayName( Section.Type )}: {ReportDefinitionHelper.GetSectionDisplayName( Section )}";

    private Task OnContextMenuAsync( MouseEventArgs eventArgs )
    {
        return ContextMenu?.Invoke( eventArgs ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Report section displayed in the designer label.
    /// </summary>
    [Parameter] public ReportSectionDefinition Section { get; set; }

    /// <summary>
    /// Raised when the label is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Raised when the label context menu is requested.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ContextMenu { get; set; }
}