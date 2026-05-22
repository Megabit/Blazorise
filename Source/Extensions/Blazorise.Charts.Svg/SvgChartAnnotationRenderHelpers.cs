#region Using directives
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal static class SvgChartAnnotationRenderHelpers
{
    #region Methods

    public static SvgChartPointBounds ResolveBounds( SvgChartPluginRenderContext context, double? xMin, double? xMax, double? yMin, double? yMax, string valueAxisId )
    {
        var x1 = context.ProjectAnnotationX( xMin, context.PlotArea.Left );
        var x2 = context.ProjectAnnotationX( xMax, context.PlotArea.Right );
        var y1 = context.ProjectAnnotationY( yMin, valueAxisId, context.PlotArea.Bottom );
        var y2 = context.ProjectAnnotationY( yMax, valueAxisId, context.PlotArea.Top );
        var left = System.Math.Min( x1, x2 );
        var top = System.Math.Min( y1, y2 );

        return new()
        {
            X = left,
            Y = top,
            Width = System.Math.Abs( x2 - x1 ),
            Height = System.Math.Abs( y2 - y1 )
        };
    }

    public static (double X, double Y) ResolvePoint( SvgChartPluginRenderContext context, double? x, double? y, string valueAxisId )
    {
        return (
            context.ProjectAnnotationX( x, context.PlotArea.Left + context.PlotArea.Width / 2 ),
            context.ProjectAnnotationY( y, valueAxisId, context.PlotArea.Top + context.PlotArea.Height / 2 )
        );
    }

    public static (double X1, double Y1, double X2, double Y2) ResolveLine( SvgChartPluginRenderContext context, double? xMin, double? xMax, double? yMin, double? yMax, string valueAxisId )
    {
        return (
            context.ProjectAnnotationX( xMin, xMax.HasValue ? context.ProjectAnnotationX( xMax, context.PlotArea.Right ) : context.PlotArea.Left ),
            context.ProjectAnnotationY( yMin, valueAxisId, yMax.HasValue ? context.ProjectAnnotationY( yMax, valueAxisId, context.PlotArea.Top ) : context.PlotArea.Top ),
            context.ProjectAnnotationX( xMax, xMin.HasValue ? context.ProjectAnnotationX( xMin, context.PlotArea.Left ) : context.PlotArea.Right ),
            context.ProjectAnnotationY( yMax, valueAxisId, yMin.HasValue ? context.ProjectAnnotationY( yMin, valueAxisId, context.PlotArea.Bottom ) : context.PlotArea.Bottom )
        );
    }

    public static SvgChartPointBounds ResolveLineBounds( (double X1, double Y1, double X2, double Y2) line )
    {
        var left = System.Math.Min( line.X1, line.X2 );
        var top = System.Math.Min( line.Y1, line.Y2 );

        return new()
        {
            X = left,
            Y = top,
            Width = System.Math.Abs( line.X2 - line.X1 ),
            Height = System.Math.Abs( line.Y2 - line.Y1 )
        };
    }

    public static void RenderLabel( RenderTreeBuilder builder, ref int sequence, SvgChartPluginRenderContext context, SvgChartAnnotationLabelOptions label, SvgChartPointBounds bounds )
    {
        if ( label?.Visible != true || string.IsNullOrWhiteSpace( label.Text ) )
            return;

        var font = CreateFontOptions( context.Options.Font, label.Font ) ?? new();
        var fontSize = font.Size ?? context.Options.Font?.Size ?? 11;
        var padding = label.Padding ?? new();
        var textWidth = SvgChartRenderHelpers.EstimateTextWidth( label.Text, fontSize );
        var width = textWidth + padding.Start + padding.End;
        var height = fontSize + padding.Top + padding.Bottom;
        var anchor = ResolveLabelAnchor( bounds, label.Position, label.Offset, width, height );
        var backgroundColor = ResolveLabelBackgroundColor( label );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-annotation-label" );

        builder.OpenElement( sequence++, "rect" );
        builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( anchor.X ) );
        builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( anchor.Y ) );
        builder.AddAttribute( sequence++, "width", SvgChartRenderHelpers.Format( width ) );
        builder.AddAttribute( sequence++, "height", SvgChartRenderHelpers.Format( height ) );
        builder.AddAttribute( sequence++, "rx", SvgChartRenderHelpers.Format( label.Border?.Radius ?? 0 ) );
        builder.AddAttribute( sequence++, "fill", backgroundColor );

        if ( label.Border?.Width > 0 )
        {
            builder.AddAttribute( sequence++, "stroke", ResolveAnnotationColor( label.Border.Color, "currentColor" ) );
            builder.AddAttribute( sequence++, "stroke-width", SvgChartRenderHelpers.Format( label.Border.Width ) );
        }

        builder.CloseElement();

        builder.OpenElement( sequence++, "text" );
        builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( anchor.X + padding.Start ) );
        builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( anchor.Y + padding.Top + fontSize * 0.82 ) );
        SvgChartRenderHelpers.AddFontFamilyAttribute( builder, ref sequence, font.Family );
        builder.AddAttribute( sequence++, "font-size", SvgChartRenderHelpers.Format( fontSize ) );

        if ( !string.IsNullOrWhiteSpace( font.Weight ) )
            builder.AddAttribute( sequence++, "font-weight", font.Weight );

        builder.AddAttribute( sequence++, "fill", SvgChartRenderHelpers.ResolveFontColor( font ) );
        builder.AddContent( sequence++, label.Text );
        builder.CloseElement();

        builder.CloseElement();
    }

    public static string ResolveAnnotationBackgroundColor( Color color )
    {
        return SvgChartRenderHelpers.IsDefaultColor( color )
            ? "currentColor"
            : SvgChartRenderHelpers.ResolveColor( color, 0 );
    }

    public static string ResolveAnnotationColor( Color color, string fallback )
    {
        return SvgChartRenderHelpers.IsDefaultColor( color )
            ? fallback
            : SvgChartRenderHelpers.ResolveColor( color, 0 );
    }

    public static SvgChartAnnotationLabelOptions CreateLabelOptions( SvgChartAnnotationLabelOptions label )
    {
        if ( label is null )
            return null;

        return new()
        {
            Visible = label.Visible,
            Text = label.Text,
            Position = label.Position,
            Offset = label.Offset,
            Font = CreateFontOptions( label.Font ),
            Padding = CreateSpacing( label.Padding ),
            BackgroundColor = label.BackgroundColor,
            Border = CreateBorderOptions( label.Border )
        };
    }

    public static SvgChartBorderOptions CreateBorderOptions( SvgChartBorderOptions options )
    {
        if ( options is null )
            return null;

        return new()
        {
            Color = options.Color,
            Width = options.Width,
            Radius = options.Radius
        };
    }

    public static SvgChartFontOptions CreateFontOptions( SvgChartFontOptions options )
    {
        if ( options is null )
            return null;

        return new()
        {
            Family = options.Family,
            Size = options.Size,
            Weight = options.Weight,
            Color = options.Color
        };
    }

    private static SvgChartFontOptions CreateFontOptions( SvgChartFontOptions options, SvgChartFontOptions overrides )
    {
        if ( overrides is null )
            return CreateFontOptions( options );

        return new()
        {
            Family = overrides.Family ?? options?.Family,
            Size = overrides.Size ?? options?.Size,
            Weight = overrides.Weight ?? options?.Weight,
            Color = SvgChartRenderHelpers.IsDefaultColor( overrides.Color ) ? options?.Color : overrides.Color
        };
    }

    private static SvgChartSpacing CreateSpacing( SvgChartSpacing spacing )
    {
        if ( spacing is null )
            return new();

        return new()
        {
            Top = spacing.Top,
            End = spacing.End,
            Bottom = spacing.Bottom,
            Start = spacing.Start
        };
    }

    private static string ResolveLabelBackgroundColor( SvgChartAnnotationLabelOptions label )
    {
        return SvgChartRenderHelpers.IsDefaultColor( label.BackgroundColor )
            ? "var(--bs-body-bg, #fff)"
            : SvgChartRenderHelpers.ResolveColor( label.BackgroundColor, 0 );
    }

    private static (double X, double Y) ResolveLabelAnchor( SvgChartPointBounds bounds, SvgChartAnnotationLabelPosition position, double offset, double width, double height )
    {
        var centerX = bounds.X + bounds.Width / 2;
        var centerY = bounds.Y + bounds.Height / 2;

        return position switch
        {
            SvgChartAnnotationLabelPosition.Start => (bounds.X + offset, bounds.Y + offset),
            SvgChartAnnotationLabelPosition.End => (bounds.X + bounds.Width - width - offset, bounds.Y + bounds.Height - height - offset),
            _ => (centerX - width / 2, centerY - height / 2)
        };
    }

    #endregion
}