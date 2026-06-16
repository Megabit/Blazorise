#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a report band section and its designer interaction shell.
/// </summary>
public partial class _ReportDesignerSection
{
    private readonly ClassBuilder bodyClassBuilder;
    private readonly StyleBuilder bodyStyleBuilder;

    private ElementReference bodyElement;

    private string BodyClass => bodyClassBuilder.Class;

    private string BodyStyle => bodyStyleBuilder.Styles;

    private Func<MouseEventArgs, Task> NonRenderingBodyContextMenu => EventUtil.AsNonRenderingEventHandler<MouseEventArgs>( OnBodyContextMenuAsync );

    private Func<DragEventArgs, Task> NonRenderingDragOver => EventUtil.AsNonRenderingEventHandler<DragEventArgs>( OnDragOverAsync );

    private Func<PointerEventArgs, Task> NonRenderingPointerMove => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerMoveAsync );

    private string SectionClass => ClassNames;

    private string SectionStyle => StyleNames;

    /// <summary>
    /// Initializes a new _ReportDesignerSection component instance.
    /// </summary>
    public _ReportDesignerSection()
    {
        bodyClassBuilder = new( BuildBodyClasses );
        bodyStyleBuilder = new( BuildBodyStyles );
    }

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<ReportSectionDefinition>( nameof( Section ), out _ ) )
        {
            bodyClassBuilder.Dirty();
            DirtyClasses();
            DirtyStyles();
        }

        if ( ( parameters.TryGetValue<bool>( nameof( RailVisible ), out bool paramRailVisible ) && paramRailVisible != RailVisible )
             || ( parameters.TryGetValue<bool>( nameof( ExternalDragActive ), out bool paramExternalDragActive ) && paramExternalDragActive != ExternalDragActive ) )
        {
            bodyClassBuilder.Dirty();
        }

        if ( ( parameters.TryGetValue<bool>( nameof( RailVisible ), out bool paramRailVisibleForStyle ) && paramRailVisibleForStyle != RailVisible )
             || ( parameters.TryGetValue<double>( nameof( BodyLeft ), out double paramBodyLeft ) && paramBodyLeft != BodyLeft )
             || ( parameters.TryGetValue<double>( nameof( BodyWidth ), out double paramBodyWidth ) && paramBodyWidth != BodyWidth ) )
        {
            bodyStyleBuilder.Dirty();
        }

        if ( ( parameters.TryGetValue<bool>( nameof( Active ), out bool paramActive ) && paramActive != Active )
             || ( parameters.TryGetValue<bool>( nameof( Collapsed ), out bool paramCollapsed ) && paramCollapsed != Collapsed ) )
        {
            DirtyClasses();
        }

        if ( parameters.TryGetValue<double>( nameof( Height ), out double paramHeight ) && paramHeight != Height )
            DirtyStyles();

        return base.SetParametersAsync( parameters );
    }

    private void BuildBodyClasses( ClassBuilder builder )
    {
        builder.Append( "b-report-section-body" );
        builder.Append( "b-report-section-body-rail", RailVisible );
        builder.Append( "b-report-section-body-external-drag", ExternalDragActive );
        builder.Append( "disabled", Section.Suppressed );
    }

    private void BuildBodyStyles( StyleBuilder builder )
    {
        builder.Append( $"left:{BodyLeft}px", RailVisible );
        builder.Append( $"width:{ReportMeasurementConverter.ToCssPixelString( BodyWidth )}", RailVisible );
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-report-section" );
        builder.Append( Section.Class );
        builder.Append( "active", Active );
        builder.Append( "collapsed", Collapsed );
        builder.Append( "suppressed", Section.Suppressed );
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"height:{ReportMeasurementConverter.ToCssPixelString( Height )}" );
        builder.Append( $"background-color:{Section.Appearance?.BackgroundColor}!important", !string.IsNullOrWhiteSpace( Section.Appearance?.BackgroundColor ) );
        builder.Append( $"opacity:{Section.Appearance?.Opacity}", Section.Appearance?.Opacity is not null );
        builder.Append( $"border-color:{Section.Border?.Color}!important", !string.IsNullOrWhiteSpace( Section.Border?.Color ) );

        if ( Section.Border?.Width is { } borderWidth )
        {
            builder.Append( $"border-width:{ReportMeasurementConverter.ToCssPixelString( borderWidth )}" );
            builder.Append( "border-style:solid" );
        }

        if ( Section.Border?.Radius is { } borderRadius )
            builder.Append( $"border-radius:{ReportMeasurementConverter.ToCssPixelString( borderRadius )}" );

        builder.Append( Section.Style?.Trim().TrimEnd( ';' ) );
    }

    private Task OnDragOverAsync( DragEventArgs eventArgs )
    {
        return DragOver?.Invoke( bodyElement, eventArgs ) ?? Task.CompletedTask;
    }

    private Task OnPointerMoveAsync( PointerEventArgs eventArgs )
    {
        return PointerMove?.Invoke( eventArgs ) ?? Task.CompletedTask;
    }

    private Task OnDropAsync( DragEventArgs eventArgs )
    {
        return Drop?.Invoke( bodyElement, eventArgs ) ?? Task.CompletedTask;
    }

    private Task OnBodyContextMenuAsync( MouseEventArgs eventArgs )
    {
        return BodyContextMenu?.Invoke( eventArgs ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Stable key used to preserve band identity across designer renders.
    /// </summary>
    [Parameter] public object SectionKey { get; set; }

    /// <summary>
    /// Report section rendered on the report surface.
    /// </summary>
    [Parameter] public ReportSectionDefinition Section { get; set; }

    /// <summary>
    /// Section height in report layout units.
    /// </summary>
    [Parameter] public double Height { get; set; }

    /// <summary>
    /// Left offset of the section body when the band rail is visible.
    /// </summary>
    [Parameter] public double BodyLeft { get; set; }

    /// <summary>
    /// Section body width in report layout units when the band rail is visible.
    /// </summary>
    [Parameter] public double BodyWidth { get; set; }

    /// <summary>
    /// Indicates that the section is rendered in designer mode.
    /// </summary>
    [Parameter] public bool DesignMode { get; set; }

    /// <summary>
    /// Indicates that the section is currently selected.
    /// </summary>
    [Parameter] public bool Active { get; set; }

    /// <summary>
    /// Indicates that the section content is collapsed.
    /// </summary>
    [Parameter] public bool Collapsed { get; set; }

    /// <summary>
    /// Indicates that the section uses the left band rail.
    /// </summary>
    [Parameter] public bool RailVisible { get; set; }

    /// <summary>
    /// Allows designer interactions inside the section body.
    /// </summary>
    [Parameter] public bool Editable { get; set; }

    /// <summary>
    /// Indicates that a field or toolbox item is being dragged over the designer.
    /// </summary>
    [Parameter] public bool ExternalDragActive { get; set; }

    /// <summary>
    /// Allows the band rail toggle to collapse and expand this section.
    /// </summary>
    [Parameter] public bool AllowBandCollapse { get; set; }

    /// <summary>
    /// Shows the band data source in the section rail when available.
    /// </summary>
    [Parameter] public bool ShowBandDataSource { get; set; }

    /// <summary>
    /// Indicates that a drag preview should be rendered inside the section body.
    /// </summary>
    [Parameter] public bool ShowDragPreview { get; set; }

    /// <summary>
    /// Element type used by the drag preview.
    /// </summary>
    [Parameter] public ReportElementType DragPreviewElementType { get; set; }

    /// <summary>
    /// Text shown by the drag preview.
    /// </summary>
    [Parameter] public string DragPreviewText { get; set; }

    /// <summary>
    /// Left drag preview coordinate.
    /// </summary>
    [Parameter] public double DragPreviewX { get; set; }

    /// <summary>
    /// Top drag preview coordinate.
    /// </summary>
    [Parameter] public double DragPreviewY { get; set; }

    /// <summary>
    /// Drag preview width.
    /// </summary>
    [Parameter] public double DragPreviewWidth { get; set; }

    /// <summary>
    /// Drag preview height.
    /// </summary>
    [Parameter] public double DragPreviewHeight { get; set; }

    /// <summary>
    /// Content rendered inside the section body.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Raised when the section rail is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> RailClicked { get; set; }

    /// <summary>
    /// Raised when the band collapse toggle is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> ToggleClicked { get; set; }

    /// <summary>
    /// Raised when the section rail context menu is requested.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> RailContextMenu { get; set; }

    /// <summary>
    /// Raised when the section body context menu is requested.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> BodyContextMenu { get; set; }

    /// <summary>
    /// Raised when marquee selection starts inside the section body.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> BodyPointerDown { get; set; }

    /// <summary>
    /// Raised while a toolbox or field item is dragged over the section body.
    /// </summary>
    [Parameter] public Func<ElementReference, DragEventArgs, Task> DragOver { get; set; }

    /// <summary>
    /// Raised when a toolbox or field item is dropped onto the section body.
    /// </summary>
    [Parameter] public Func<ElementReference, DragEventArgs, Task> Drop { get; set; }

    /// <summary>
    /// Raised while pointer element interaction continues inside the section body.
    /// </summary>
    [Parameter] public Func<PointerEventArgs, Task> PointerMove { get; set; }

    /// <summary>
    /// Raised when pointer element interaction completes inside the section body.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> PointerUp { get; set; }

    /// <summary>
    /// Raised when pointer element interaction is cancelled inside the section body.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> PointerCancel { get; set; }

    /// <summary>
    /// Raised when the editable section body is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> BodyClicked { get; set; }

    /// <summary>
    /// Raised when band resizing starts from the section resize handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> ResizePointerDown { get; set; }
}