#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a box annotation for a native SVG chart.
/// </summary>
public class SvgChartBoxAnnotation : SvgChartPluginBase
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
    /// Defines the annotation rendering order among box annotations. Lower values are rendered first, behind higher values.
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
    /// Defines the annotation background color.
    /// </summary>
    [Parameter] public Color BackgroundColor { get; set; }

    /// <summary>
    /// Defines the annotation border options.
    /// </summary>
    [Parameter] public SvgChartBorderOptions Border { get; set; }

    /// <summary>
    /// Defines the annotation opacity.
    /// </summary>
    [Parameter] public double Opacity { get; set; } = 0.16;

    /// <summary>
    /// Defines annotation label options.
    /// </summary>
    [Parameter] public SvgChartAnnotationLabelOptions Label { get; set; }

    /// <inheritdoc/>
    public override SvgChartRenderLayer Layer => SvgChartRenderLayer.BeforeSeries;

    /// <inheritdoc/>
    public override void Render( SvgChartPluginRenderContext context, RenderTreeBuilder builder, ref int sequence )
    {
        if ( !Visible || context.IsRadial )
            return;

        var bounds = SvgChartAnnotationRenderHelpers.ResolveBounds( context, XMin, XMax, YMin, YMax, ValueAxisId );

        builder.OpenElement( sequence++, "rect" );
        builder.AddAttribute( sequence++, "class", "svg-chart-annotation svg-chart-box-annotation" );
        builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( bounds.X ) );
        builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( bounds.Y ) );
        builder.AddAttribute( sequence++, "width", SvgChartRenderHelpers.Format( bounds.Width ) );
        builder.AddAttribute( sequence++, "height", SvgChartRenderHelpers.Format( bounds.Height ) );
        builder.AddAttribute( sequence++, "fill", SvgChartAnnotationRenderHelpers.ResolveAnnotationBackgroundColor( BackgroundColor ) );
        builder.AddAttribute( sequence++, "opacity", SvgChartRenderHelpers.Format( Opacity ) );

        if ( Border?.Width > 0 )
        {
            builder.AddAttribute( sequence++, "stroke", SvgChartAnnotationRenderHelpers.ResolveAnnotationColor( Border.Color, "currentColor" ) );
            builder.AddAttribute( sequence++, "stroke-width", SvgChartRenderHelpers.Format( Border.Width ) );

            if ( Border.Radius > 0 )
                builder.AddAttribute( sequence++, "rx", SvgChartRenderHelpers.Format( Border.Radius ) );
        }

        builder.CloseElement();

        SvgChartAnnotationRenderHelpers.RenderLabel( builder, ref sequence, context, Label, bounds );
    }

    internal static SvgChartBoxAnnotation Create( SvgChartBoxAnnotationOptions annotation )
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
            BackgroundColor = annotation.BackgroundColor,
            Border = SvgChartAnnotationRenderHelpers.CreateBorderOptions( annotation.Border ),
            Opacity = annotation.Opacity
        };
    }

    #endregion
}