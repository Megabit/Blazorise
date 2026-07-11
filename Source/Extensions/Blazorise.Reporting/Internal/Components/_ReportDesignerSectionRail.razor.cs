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
    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<ReportBandDefinition>( nameof( Section ), out _ )
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
        builder.Append( "suppressed", ReportValueResolver.ResolveStaticSuppress( Section ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"height:{ReportMeasurementConverter.ToCssPixelString( Height )}" );

        base.BuildStyles( builder );
    }

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

    private Task OnToggleClicked( MouseEventArgs eventArgs )
    {
        if ( ToggleClicked is not null )
            return ToggleClicked.Invoke( eventArgs );

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    private string DisplayName => ReportDefinitionHelper.GetSectionDisplayName( Section );

    private string ToggleTitle => ReportValueResolver.ResolveStaticSuppress( Section ) ? "Band is suppressed" : Collapsed ? "Expand band" : "Collapse band";

    /// <summary>
    /// Report section displayed in the rail.
    /// </summary>
    [Parameter] public ReportBandDefinition Section { get; set; }

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
    [Parameter] public Func<MouseEventArgs, Task> Clicked { get; set; }

    /// <summary>
    /// Raised when the section rail context menu is requested.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ContextMenu { get; set; }

    /// <summary>
    /// Raised when the section collapse toggle is clicked.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ToggleClicked { get; set; }

    #endregion
}