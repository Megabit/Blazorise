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
/// Renders the temporary element preview shown while dragging toolbox or field items over the report surface.
/// </summary>
public partial class _ReportDesignerDragPreview
{
    private const string Key = "drag-preview";

    private string Class => ClassNames;

    private string Style => StyleNames;

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<ReportElementType>( nameof( ElementType ), out var paramElementType ) && paramElementType != ElementType )
            DirtyClasses();

        if ( ( parameters.TryGetValue<double>( nameof( X ), out var paramX ) && paramX != X )
             || ( parameters.TryGetValue<double>( nameof( Y ), out var paramY ) && paramY != Y )
             || ( parameters.TryGetValue<double>( nameof( Width ), out var paramWidth ) && paramWidth != Width )
             || ( parameters.TryGetValue<double>( nameof( Height ), out var paramHeight ) && paramHeight != Height ) )
            DirtyStyles();

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-report-drag-preview" );
        builder.Append( $"b-report-element-{ElementType.ToString().ToLowerInvariant()}" );
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"left:{ReportMeasurementConverter.ToCssPixelString( X )}" );
        builder.Append( $"top:{ReportMeasurementConverter.ToCssPixelString( Y )}" );
        builder.Append( $"width:{ReportMeasurementConverter.ToCssPixelString( Width )}" );
        builder.Append( $"height:{ReportMeasurementConverter.ToCssPixelString( Height )}" );
    }

    /// <summary>
    /// Type of report element represented by the preview.
    /// </summary>
    [Parameter] public ReportElementType ElementType { get; set; }

    /// <summary>
    /// Text shown inside the preview.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Left preview coordinate.
    /// </summary>
    [Parameter] public double X { get; set; }

    /// <summary>
    /// Top preview coordinate.
    /// </summary>
    [Parameter] public double Y { get; set; }

    /// <summary>
    /// Preview width.
    /// </summary>
    [Parameter] public double Width { get; set; }

    /// <summary>
    /// Preview height.
    /// </summary>
    [Parameter] public double Height { get; set; }
}