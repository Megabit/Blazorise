#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the report page surface used by the designer and viewer.
/// </summary>
public partial class _ReportDesignerPage
{
    private ElementReference pageElement;

    private string Class => ClassNames;

    private string MarginBottomStyle => BuildMarginStyle(
        WidthOffset,
        Height - MarginBottom,
        Width,
        MarginBottom );

    private string MarginLeftStyle => BuildMarginStyle(
        WidthOffset,
        MarginTop,
        MarginLeft,
        GetPrintableHeight() );

    private string MarginRightStyle => BuildMarginStyle(
        WidthOffset + ReportMeasurementConverter.ToCssPixelValue( Width - MarginRight ),
        MarginTop,
        MarginRight,
        GetPrintableHeight() );

    private string MarginSpacerStyle => $"height:{ReportMeasurementConverter.ToCssPixelString( MarginTop )}";

    private string MarginTopStyle => BuildMarginStyle(
        WidthOffset,
        0,
        Width,
        MarginTop );

    private Func<PointerEventArgs, Task> NonRenderingPointerMove => EventUtil.AsNonRenderingEventHandler<PointerEventArgs>( OnPointerMoveAsync );

    private string PrintableAreaStyle => BuildMarginStyle(
        WidthOffset + ReportMeasurementConverter.ToCssPixelValue( MarginLeft ),
        MarginTop,
        GetPrintableWidth(),
        GetPrintableHeight() );

    private string Style => StyleNames;

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<bool>( nameof( DesignMode ), out var paramDesignMode ) && paramDesignMode != DesignMode )
            DirtyClasses();

        if ( ( parameters.TryGetValue<double>( nameof( Width ), out var paramWidth ) && paramWidth != Width )
             || ( parameters.TryGetValue<double>( nameof( WidthOffset ), out var paramWidthOffset ) && paramWidthOffset != WidthOffset )
             || ( parameters.TryGetValue<double>( nameof( MinHeight ), out var paramMinHeight ) && paramMinHeight != MinHeight )
             || ( parameters.TryGetValue<double>( nameof( Height ), out var paramHeight ) && paramHeight != Height )
             || ( parameters.TryGetValue<double>( nameof( MarginLeft ), out var paramMarginLeft ) && paramMarginLeft != MarginLeft )
             || ( parameters.TryGetValue<double>( nameof( MarginTop ), out var paramMarginTop ) && paramMarginTop != MarginTop )
             || ( parameters.TryGetValue<double>( nameof( MarginRight ), out var paramMarginRight ) && paramMarginRight != MarginRight )
             || ( parameters.TryGetValue<double>( nameof( MarginBottom ), out var paramMarginBottom ) && paramMarginBottom != MarginBottom ) )
            DirtyStyles();

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-report-page" );
        builder.Append( "b-report-page-design", DesignMode );
        builder.Append( "b-report-page-preview", !DesignMode );
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"width:{ReportMeasurementConverter.FormatCssPixelValue( ReportMeasurementConverter.ToCssPixelValue( Width ) + WidthOffset )}" );
        builder.Append( $"min-height:{ReportMeasurementConverter.ToCssPixelString( MinHeight )}" );
        builder.Append( $"height:{ReportMeasurementConverter.ToCssPixelString( Height )}", !DesignMode && Height > 0 );
    }

    private Task OnPointerMoveAsync( PointerEventArgs eventArgs )
    {
        return PointerMove?.Invoke( eventArgs ) ?? Task.CompletedTask;
    }

    private static string BuildMarginStyle( double leftCssPixels, double top, double width, double height )
    {
        return $"left:{ReportMeasurementConverter.FormatCssPixelValue( leftCssPixels )}; top:{ReportMeasurementConverter.ToCssPixelString( Math.Max( 0, top ) )}; width:{ReportMeasurementConverter.ToCssPixelString( Math.Max( 0, width ) )}; height:{ReportMeasurementConverter.ToCssPixelString( Math.Max( 0, height ) )}";
    }

    private double GetPrintableHeight()
    {
        return Math.Max( 0, Height - MarginTop - MarginBottom );
    }

    private double GetPrintableWidth()
    {
        return Math.Max( 0, Width - MarginLeft - MarginRight );
    }

    /// <summary>
    /// Root page element reference used by JavaScript overlays.
    /// </summary>
    internal ElementReference Element => pageElement;

    /// <summary>
    /// Stable key used to preserve page identity across designer renders.
    /// </summary>
    [Parameter] public object PageKey { get; set; }

    /// <summary>
    /// Page width in report layout units.
    /// </summary>
    [Parameter] public double Width { get; set; }

    /// <summary>
    /// Additional screen width reserved by the designer shell.
    /// </summary>
    [Parameter] public double WidthOffset { get; set; }

    /// <summary>
    /// Minimum page height in report layout units.
    /// </summary>
    [Parameter] public double MinHeight { get; set; }

    /// <summary>
    /// Exact page height in report layout units.
    /// </summary>
    [Parameter] public double Height { get; set; }

    /// <summary>
    /// Left page margin in report layout units.
    /// </summary>
    [Parameter] public double MarginLeft { get; set; }

    /// <summary>
    /// Top page margin in report layout units.
    /// </summary>
    [Parameter] public double MarginTop { get; set; }

    /// <summary>
    /// Right page margin in report layout units.
    /// </summary>
    [Parameter] public double MarginRight { get; set; }

    /// <summary>
    /// Bottom page margin in report layout units.
    /// </summary>
    [Parameter] public double MarginBottom { get; set; }

    /// <summary>
    /// Indicates that the page is rendered in designer mode.
    /// </summary>
    [Parameter] public bool DesignMode { get; set; }

    /// <summary>
    /// Indicates that the selection box should be displayed.
    /// </summary>
    [Parameter] public bool ShowSelectionBox { get; set; }

    /// <summary>
    /// Left selection box coordinate.
    /// </summary>
    [Parameter] public double SelectionBoxX { get; set; }

    /// <summary>
    /// Top selection box coordinate.
    /// </summary>
    [Parameter] public double SelectionBoxY { get; set; }

    /// <summary>
    /// Selection box width.
    /// </summary>
    [Parameter] public double SelectionBoxWidth { get; set; }

    /// <summary>
    /// Selection box height.
    /// </summary>
    [Parameter] public double SelectionBoxHeight { get; set; }

    /// <summary>
    /// Horizontal selection offset applied when the band rail is visible.
    /// </summary>
    [Parameter] public double SelectionBoxLeftOffset { get; set; }

    /// <summary>
    /// Content rendered inside the report page.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Raised while the pointer moves across the designer page.
    /// </summary>
    [Parameter] public Func<PointerEventArgs, Task> PointerMove { get; set; }

    /// <summary>
    /// Raised when pointer selection completes on the designer page.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> PointerUp { get; set; }

    /// <summary>
    /// Raised when pointer selection is cancelled on the designer page.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> PointerCancel { get; set; }
}