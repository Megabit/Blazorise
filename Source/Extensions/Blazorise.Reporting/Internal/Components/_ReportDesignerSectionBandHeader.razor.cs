#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the horizontal band header used by the classic report designer surface.
/// </summary>
public partial class _ReportDesignerSectionBandHeader
{
    private string Class => ClassNames;

    private string DisplayName => ReportDefinitionHelper.GetSectionDisplayName( Section );

    private Func<MouseEventArgs, Task> NonRenderingContextMenu => EventUtil.AsNonRenderingEventHandler<MouseEventArgs>( OnContextMenuAsync );

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<ReportSectionDefinition>( nameof( Section ), out _ )
             || ( parameters.TryGetValue<bool>( nameof( Selected ), out bool paramSelected ) && paramSelected != Selected ) )
        {
            DirtyClasses();
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-report-section-band-header" );
        builder.Append( $"b-report-section-band-header-{Section.Type.ToString().ToLowerInvariant()}" );
        builder.Append( "active", Selected );
        builder.Append( "suppressed", Section.Suppressed );
    }

    private Task OnContextMenuAsync( MouseEventArgs eventArgs )
    {
        return ContextMenu?.Invoke( eventArgs ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Report section displayed in the band header.
    /// </summary>
    [Parameter] public ReportSectionDefinition Section { get; set; }

    /// <summary>
    /// Indicates that the section is currently selected.
    /// </summary>
    [Parameter] public bool Selected { get; set; }

    /// <summary>
    /// Shows the section data source text when available.
    /// </summary>
    [Parameter] public bool ShowDataSource { get; set; }

    /// <summary>
    /// Raised when the section band header is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Raised when the section band header context menu is requested.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ContextMenu { get; set; }
}