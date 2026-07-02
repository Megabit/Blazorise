#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the clickable band rail used by the report designer surface.
/// </summary>
public partial class _ReportDesignerSectionRail
{
    private string Class => ClassNames;

    private string DisplayName => ReportDefinitionHelper.GetSectionDisplayName( Section );

    private Func<MouseEventArgs, Task> NonRenderingContextMenu => EventUtil.AsNonRenderingEventHandler<MouseEventArgs>( OnContextMenuAsync );

    private string Style => StyleNames;

    private string ToggleTitle => Section.Suppressed ? "Band is suppressed" : Collapsed ? "Expand band" : "Collapse band";

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<ReportSectionDefinition>( nameof( Section ), out _ )
             || ( parameters.TryGetValue<bool>( nameof( Selected ), out bool paramSelected ) && paramSelected != Selected )
             || ( parameters.TryGetValue<bool>( nameof( Collapsed ), out bool paramCollapsed ) && paramCollapsed != Collapsed ) )
        {
            DirtyClasses();
        }

        if ( parameters.TryGetValue<double>( nameof( Height ), out double paramHeight ) && paramHeight != Height )
            DirtyStyles();

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-report-section-rail" );
        builder.Append( $"b-report-section-rail-{Section.Type.ToString().ToLowerInvariant()}" );
        builder.Append( "active", Selected );
        builder.Append( "collapsed", Collapsed );
        builder.Append( "suppressed", Section.Suppressed );
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"height:{ReportMeasurementConverter.ToCssPixelString( Height )}" );
    }

    private Task OnContextMenuAsync( MouseEventArgs eventArgs )
    {
        return ContextMenu?.Invoke( eventArgs ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Report section displayed in the rail.
    /// </summary>
    [Parameter] public ReportSectionDefinition Section { get; set; }

    /// <summary>
    /// Rail height in report layout units.
    /// </summary>
    [Parameter] public double Height { get; set; }

    /// <summary>
    /// Indicates that the section is currently selected.
    /// </summary>
    [Parameter] public bool Selected { get; set; }

    /// <summary>
    /// Indicates that the section content is collapsed.
    /// </summary>
    [Parameter] public bool Collapsed { get; set; }

    /// <summary>
    /// Allows the band to be collapsed and expanded from the rail.
    /// </summary>
    [Parameter] public bool AllowCollapse { get; set; }

    /// <summary>
    /// Shows the section data source text when available.
    /// </summary>
    [Parameter] public bool ShowDataSource { get; set; }

    /// <summary>
    /// Raised when the section rail is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Raised when the section rail context menu is requested.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ContextMenu { get; set; }

    /// <summary>
    /// Raised when the section collapse toggle is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> ToggleClicked { get; set; }
}