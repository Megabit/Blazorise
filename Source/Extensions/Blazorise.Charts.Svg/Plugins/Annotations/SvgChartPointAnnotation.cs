#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a point annotation for a native SVG chart.
/// </summary>
public class SvgChartPointAnnotation : SvgChartPluginBase
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
    /// Defines the annotation rendering order among point annotations. Lower values are rendered first, behind higher values.
    /// </summary>
    [Parameter] public int? Order { get; set; }

    /// <summary>
    /// Defines the annotation X value. For category charts, this is the category index.
    /// </summary>
    [Parameter] public double? X { get; set; }

    /// <summary>
    /// Defines the annotation Y value.
    /// </summary>
    [Parameter] public double? Y { get; set; }

    /// <summary>
    /// Defines the annotation point radius.
    /// </summary>
    [Parameter] public double Radius { get; set; } = 5;

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

        var point = SvgChartAnnotationRenderHelpers.ResolvePoint( context, X, Y, ValueAxisId );
        var radius = System.Math.Max( 0, Radius );
        var bounds = new SvgChartPointBounds
        {
            X = point.X - radius,
            Y = point.Y - radius,
            Width = radius * 2,
            Height = radius * 2
        };

        builder.OpenElement( sequence++, "circle" );
        builder.AddAttribute( sequence++, "class", "svg-chart-annotation svg-chart-point-annotation" );
        builder.AddAttribute( sequence++, "cx", SvgChartRenderHelpers.Format( point.X ) );
        builder.AddAttribute( sequence++, "cy", SvgChartRenderHelpers.Format( point.Y ) );
        builder.AddAttribute( sequence++, "r", SvgChartRenderHelpers.Format( radius ) );
        builder.AddAttribute( sequence++, "fill", SvgChartAnnotationRenderHelpers.ResolveAnnotationBackgroundColor( BackgroundColor ) );
        builder.AddAttribute( sequence++, "opacity", SvgChartRenderHelpers.Format( Opacity ) );

        if ( Border?.Width > 0 )
        {
            builder.AddAttribute( sequence++, "stroke", SvgChartAnnotationRenderHelpers.ResolveAnnotationColor( Border.Color, "currentColor" ) );
            builder.AddAttribute( sequence++, "stroke-width", SvgChartRenderHelpers.Format( Border.Width ) );
        }

        builder.CloseElement();

        SvgChartAnnotationRenderHelpers.RenderLabel( builder, ref sequence, context, Label, bounds );
    }

    internal static SvgChartPointAnnotation Create( SvgChartPointAnnotationOptions annotation )
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
            X = annotation.X,
            Y = annotation.Y,
            Radius = annotation.Radius,
            BackgroundColor = annotation.BackgroundColor,
            Border = SvgChartAnnotationRenderHelpers.CreateBorderOptions( annotation.Border ),
            Opacity = annotation.Opacity
        };
    }

    #endregion
}