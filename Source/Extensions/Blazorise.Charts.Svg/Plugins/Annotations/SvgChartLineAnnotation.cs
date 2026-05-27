#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a line annotation for a native SVG chart.
/// </summary>
public class SvgChartLineAnnotation : SvgChartPluginBase
{
    #region Properties

    /// <summary>
    /// Defines whether the annotation is visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the annotation name.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Defines the value axis identifier used by this annotation.
    /// </summary>
    [Parameter] public string ValueAxisId { get; set; }

    /// <summary>
    /// Defines the annotation rendering order among line annotations. Lower values are rendered first, behind higher values.
    /// </summary>
    [Parameter] public int? Order { get; set; }

    /// <summary>
    /// Defines the annotation start X value. For category charts, this is the category index where <c>-0.5</c> represents the left plot edge.
    /// </summary>
    [Parameter] public double? XMin { get; set; }

    /// <summary>
    /// Defines the annotation end X value. For category charts, this is the category index where <c>-0.5</c> represents the left plot edge.
    /// </summary>
    [Parameter] public double? XMax { get; set; }

    /// <summary>
    /// Defines the annotation start Y value.
    /// </summary>
    [Parameter] public double? YMin { get; set; }

    /// <summary>
    /// Defines the annotation end Y value.
    /// </summary>
    [Parameter] public double? YMax { get; set; }

    /// <summary>
    /// Defines the annotation line border options.
    /// </summary>
    [Parameter] public SvgChartBorderOptions Border { get; set; }

    /// <summary>
    /// Defines the annotation line dash pattern.
    /// </summary>
    [Parameter] public string DashPattern { get; set; }

    /// <summary>
    /// Defines the annotation opacity.
    /// </summary>
    [Parameter] public double Opacity { get; set; } = 1;

    /// <summary>
    /// Defines annotation label options.
    /// </summary>
    [Parameter] public SvgChartAnnotationLabelOptions Label { get; set; }

    /// <inheritdoc/>
    public override void Render( SvgChartPluginRenderContext context, RenderTreeBuilder builder, ref int sequence )
    {
        if ( !Visible || context.IsRadial )
            return;

        var line = SvgChartAnnotationRenderHelpers.ResolveLine( context, XMin, XMax, YMin, YMax, ValueAxisId );

        builder.OpenElement( sequence++, "line" );
        builder.AddAttribute( sequence++, "class", "svg-chart-annotation svg-chart-line-annotation" );
        builder.AddAttribute( sequence++, "x1", SvgChartRenderHelpers.Format( line.X1 ) );
        builder.AddAttribute( sequence++, "y1", SvgChartRenderHelpers.Format( line.Y1 ) );
        builder.AddAttribute( sequence++, "x2", SvgChartRenderHelpers.Format( line.X2 ) );
        builder.AddAttribute( sequence++, "y2", SvgChartRenderHelpers.Format( line.Y2 ) );
        builder.AddAttribute( sequence++, "stroke", SvgChartAnnotationRenderHelpers.ResolveAnnotationColor( Border?.Color, "currentColor" ) );
        builder.AddAttribute( sequence++, "stroke-width", SvgChartRenderHelpers.Format( Border?.Width > 0 ? Border.Width : 2 ) );
        builder.AddAttribute( sequence++, "stroke-linecap", "round" );
        builder.AddAttribute( sequence++, "opacity", SvgChartRenderHelpers.Format( Opacity ) );

        if ( !string.IsNullOrWhiteSpace( DashPattern ) )
            builder.AddAttribute( sequence++, "stroke-dasharray", DashPattern );

        builder.CloseElement();

        SvgChartAnnotationRenderHelpers.RenderLabel( builder, ref sequence, context, Label, SvgChartAnnotationRenderHelpers.ResolveLineBounds( line ) );
    }

    internal static SvgChartLineAnnotation Create( SvgChartLineAnnotationOptions annotation )
    {
        if ( annotation is null )
            return null;

        return new()
        {
            Visible = annotation.Visible,
            Name = annotation.Name,
            ValueAxisId = annotation.ValueAxisId,
            Order = annotation.Order,
            Label = SvgChartAnnotationRenderHelpers.CreateLabelOptions( annotation.Label ),
            XMin = annotation.XMin,
            XMax = annotation.XMax,
            YMin = annotation.YMin,
            YMax = annotation.YMax,
            Border = SvgChartAnnotationRenderHelpers.CreateBorderOptions( annotation.Border ),
            DashPattern = annotation.DashPattern,
            Opacity = annotation.Opacity
        };
    }

    #endregion
}