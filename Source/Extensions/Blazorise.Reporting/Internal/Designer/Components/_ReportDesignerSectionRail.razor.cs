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
/// Renders the clickable band rail used by the report designer surface.
/// </summary>
public partial class _ReportDesignerSectionRail
{
    private string Class => ClassNames;

    private string DisplayName => ReportDefinitionHelper.GetSectionDisplayName( Section );

    private string Style => StyleNames;

    private string ToggleTitle => Section.Suppressed ? "Band is suppressed" : Collapsed ? "Expand band" : "Collapse band";

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<ReportSectionDefinition>( nameof( Section ), out _ )
             || ( parameters.TryGetValue<bool>( nameof( Selected ), out var paramSelected ) && paramSelected != Selected )
             || ( parameters.TryGetValue<bool>( nameof( Collapsed ), out var paramCollapsed ) && paramCollapsed != Collapsed ) )
            DirtyClasses();

        if ( parameters.TryGetValue<double>( nameof( Height ), out var paramHeight ) && paramHeight != Height )
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
    [Parameter] public EventCallback<MouseEventArgs> ContextMenu { get; set; }

    /// <summary>
    /// Raised when the section collapse toggle is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> ToggleClicked { get; set; }
}