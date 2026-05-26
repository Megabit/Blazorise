#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a label annotation for a native SVG chart.
/// </summary>
public class SvgChartLabelAnnotation : SvgChartPluginBase
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
    /// Defines the annotation rendering order among label annotations. Lower values are rendered first, behind higher values.
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
    /// Defines annotation label options.
    /// </summary>
    [Parameter] public SvgChartAnnotationLabelOptions Label { get; set; }

    /// <inheritdoc/>
    public override void Render( SvgChartPluginRenderContext context, RenderTreeBuilder builder, ref int sequence )
    {
        if ( !Visible || context.IsRadial )
            return;

        var point = SvgChartAnnotationRenderHelpers.ResolvePoint( context, X, Y, ValueAxisId );
        var bounds = new SvgChartPointBounds
        {
            X = point.X,
            Y = point.Y,
            Width = 0,
            Height = 0
        };

        SvgChartAnnotationRenderHelpers.RenderLabel( builder, ref sequence, context, Label, bounds );
    }

    internal static SvgChartLabelAnnotation Create( SvgChartLabelAnnotationOptions annotation )
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
            Y = annotation.Y
        };
    }

    #endregion
}