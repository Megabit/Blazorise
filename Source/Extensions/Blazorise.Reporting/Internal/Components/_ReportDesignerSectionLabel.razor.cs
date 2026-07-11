#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the band label used by the classic report designer surface.
/// </summary>
public partial class _ReportDesignerSectionLabel
{
    #region Methods

    private Task OnContextMenu( MouseEventArgs eventArgs )
    {
        if ( ContextMenu is not null )
            return ContextMenu.Invoke( eventArgs );

        return Task.CompletedTask;
    }

    private Task OnClicked( MouseEventArgs eventArgs )
    {
        if ( Clicked is not null )
            return Clicked.Invoke( eventArgs );

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    private string Label => $"{ReportDefinitionHelper.GetSectionTypeDisplayName( Section.Type )}: {ReportDefinitionHelper.GetSectionDisplayName( Section )}";

    /// <summary>
    /// Report section displayed in the designer label.
    /// </summary>
    [Parameter] public ReportBandDefinition Section { get; set; }

    /// <summary>
    /// Raised when the label is clicked.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> Clicked { get; set; }

    /// <summary>
    /// Raised when the label context menu is requested.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ContextMenu { get; set; }

    #endregion
}