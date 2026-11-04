#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal static class SvgChartTextRenderer
{
    #region Methods

    public static void Render( RenderTreeBuilder builder, ref int sequence, SvgChartOptions options, SvgChartTextOptions title, SvgChartTextOptions subtitle )
    {
        var top = 0d;
        var bottom = 0d;
        var start = 0d;
        var end = 0d;

        Render( builder, ref sequence, options, title, ref top, ref bottom, ref start, ref end );
        Render( builder, ref sequence, options, subtitle, ref top, ref bottom, ref start, ref end );
    }

    public static SvgChartTextOptions ResolveTitleOptions( SvgChartOptions options, IReadOnlyList<SvgChartTitle> titleComponents )
    {
        var titleComponent = titleComponents?.LastOrDefault();
        var resolved = SvgChartOptionsMapper.CreateTextOptions( CreateDefaultTitleOptions(), options?.Title );
        var componentOptions = titleComponent?.Title;

        if ( componentOptions is not null )
            resolved = SvgChartOptionsMapper.CreateTextOptions( resolved, componentOptions );

        if ( titleComponent is not null )
            resolved.Visible = titleComponent.ResolveVisible( resolved.Visible ?? true );

        return resolved;
    }

    public static SvgChartTextOptions ResolveSubtitleOptions( SvgChartOptions options, IReadOnlyList<SvgChartTitle> titleComponents )
    {
        var titleComponent = titleComponents?.LastOrDefault();
        var resolved = SvgChartOptionsMapper.CreateTextOptions( CreateDefaultSubtitleOptions(), options?.Subtitle );
        var componentOptions = titleComponent?.Subtitle;

        if ( componentOptions is not null )
            resolved = SvgChartOptionsMapper.CreateTextOptions( resolved, componentOptions );

        if ( titleComponent is not null )
            resolved.Visible = titleComponent.ResolveVisible( resolved.Visible ?? true );

        return resolved;
    }

    public static SvgChartPlotArea BuildPlotArea( SvgChartOptions options, SvgChartTextOptions title, SvgChartTextOptions subtitle, SvgChartLegendOptions legend, SvgChartRenderModel model = null )
    {
        options ??= new();

        var padding = options.PlotAreaPadding;
        var isRadialChart = model is not null && SvgChartGeometry.IsRadialChart( model.Type );
        var topPadding = padding?.Top ?? ( isRadialChart ? 8d : Math.Max( 24d, ResolveValueAxisLabelExtent( options, model ) ) );
        var endPadding = isRadialChart ? padding?.End ?? 8d : ResolveEndPadding( options, model, padding?.End );
        var startPadding = isRadialChart ? padding?.Start ?? 8d : ResolveStartPadding( options, model, padding?.Start );
        var hasCartesianAxes = model is not null && !SvgChartGeometry.IsRadialChart( model );
        var isBarChart = hasCartesianAxes && SvgChartGeometry.IsBarChart( model );

        var categoryAxisTitleSize = hasCartesianAxes
            ? ResolveAxisTitleReservedSize( options, model.CategoryAxis?.Title )
            : 0;

        var valueAxisTitleSize = hasCartesianAxes
            ? ResolveAxisTitleReservedSize( options, model.PrimaryValueAxis?.Title )
            : 0;

        var endAxisTitleSize = hasCartesianAxes && !isBarChart
            ? model.ValueAxes
                .Where( axis => axis != model.PrimaryValueAxis && axis.Position == SvgChartAxisPosition.Right )
                .Select( axis => ResolveAxisTitleReservedSize( options, axis.Title ) )
                .DefaultIfEmpty( 0 )
                .Sum()
            : 0;

        var top = topPadding + GetTopTextHeight( title ) + GetTopTextHeight( subtitle );

        if ( legend?.Visible == true && legend.Position == SvgChartLegendPosition.Top )
        {
            top += SvgChartLegendRenderer.ResolveReservedHeight( model, options, legend );
        }

        var left = startPadding + ( isBarChart ? categoryAxisTitleSize : valueAxisTitleSize ) + GetStartTextWidth( title ) + GetStartTextWidth( subtitle );
        var right = options.Width - endPadding - endAxisTitleSize - GetEndTextWidth( title ) - GetEndTextWidth( subtitle );
        var horizontalPlot = new SvgChartPlotArea
        {
            Left = left,
            Right = Math.Max( left, right ),
        };

        var bottomPadding = isRadialChart ? padding?.Bottom ?? 8d : ResolveBottomPadding( options, model, padding?.Bottom, horizontalPlot );
        var legendHeight = legend?.Visible == true && legend.Position == SvgChartLegendPosition.Bottom
            ? SvgChartLegendRenderer.ResolveReservedHeight( model, options, legend )
            : 0;

        var bottom = options.Height - bottomPadding - ( isBarChart ? valueAxisTitleSize : categoryAxisTitleSize ) - legendHeight - GetBottomTextHeight( title ) - GetBottomTextHeight( subtitle );

        return new()
        {
            Left = left,
            Top = top,
            Right = Math.Max( left, right ),
            Bottom = Math.Max( top, bottom ),
        };
    }

    public static bool IsTextVisible( SvgChartTextOptions text )
    {
        return text?.Visible == true && !string.IsNullOrWhiteSpace( text.Text );
    }

    public static void AddFontAttributes( RenderTreeBuilder builder, ref int sequence, SvgChartOptions options, double fallbackSize = 11, double? opacity = null )
    {
        AddFontAttributes( builder, ref sequence, options, null, fallbackSize, opacity );
    }

    public static void AddFontAttributes( RenderTreeBuilder builder, ref int sequence, SvgChartOptions options, SvgChartFontOptions overrides, double fallbackSize = 11, double? opacity = null )
    {
        SvgChartFontOptions font = ResolveFont( options, overrides );

        builder.AddAttribute( sequence++, "font-size", SvgChartRenderHelpers.Format( font?.Size ?? fallbackSize ) );
        SvgChartRenderHelpers.AddFontFamilyAttribute( builder, ref sequence, font?.Family );
        builder.AddAttribute( sequence++, "fill", SvgChartRenderHelpers.ResolveFontColor( font ) );

        if ( !string.IsNullOrWhiteSpace( font?.Weight ) )
            builder.AddAttribute( sequence++, "font-weight", font.Weight );

        if ( opacity is not null )
            builder.AddAttribute( sequence++, "opacity", SvgChartRenderHelpers.Format( Math.Clamp( opacity.Value, 0, 1 ) ) );
    }

    internal static SvgChartFontOptions ResolveFont( SvgChartOptions options, SvgChartFontOptions overrides )
    {
        return SvgChartOptionsMapper.CreateFontOptions( options?.Font, overrides );
    }

    internal static double ResolveFontSize( SvgChartOptions options, SvgChartFontOptions overrides, double fallbackSize = 11 )
    {
        return ResolveFont( options, overrides )?.Size ?? fallbackSize;
    }

    private static void Render( RenderTreeBuilder builder, ref int sequence, SvgChartOptions options, SvgChartTextOptions text, ref double top, ref double bottom, ref double start, ref double end )
    {
        if ( !IsTextVisible( text ) )
            return;

        var font = text.Font ?? new();
        var fontSize = font.Size ?? 12;
        var x = ResolveTextX( options, text, start, end, fontSize );
        var y = ResolveTextY( options, text, top, bottom, fontSize );

        var transform = text.Position is SvgChartTextPosition.Start or SvgChartTextPosition.End
            ? $"rotate(-90 {SvgChartRenderHelpers.Format( x )} {SvgChartRenderHelpers.Format( y )})"
            : null;

        builder.OpenElement( sequence++, "text" );
        builder.AddAttribute( sequence++, "class", text.Position == SvgChartTextPosition.Top ? "svg-chart-title" : "svg-chart-subtitle" );
        builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x ) );
        builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( y ) );
        builder.AddAttribute( sequence++, "text-anchor", ResolveTextAnchor( text.Alignment ?? SvgChartTextAlignment.Center ) );
        builder.AddAttribute( sequence++, "font-size", SvgChartRenderHelpers.Format( fontSize ) );
        builder.AddAttribute( sequence++, "fill", ResolveTextColor( options, text ) );
        SvgChartRenderHelpers.AddFontFamilyAttribute( builder, ref sequence, font.Family ?? options.Font?.Family );

        if ( !string.IsNullOrWhiteSpace( font.Weight ) )
            builder.AddAttribute( sequence++, "font-weight", font.Weight );

        if ( text.Opacity.HasValue )
            builder.AddAttribute( sequence++, "opacity", SvgChartRenderHelpers.Format( Math.Clamp( text.Opacity.Value, 0, 1 ) ) );

        if ( transform is not null )
            builder.AddAttribute( sequence++, "transform", transform );

        builder.AddContent( sequence++, text.Text );
        builder.CloseElement();

        switch ( text.Position )
        {
            case SvgChartTextPosition.Bottom:
                bottom += GetTextBlockHeight( text );
                break;
            case SvgChartTextPosition.Start:
                start += GetTextBlockWidth( text );
                break;
            case SvgChartTextPosition.End:
                end += GetTextBlockWidth( text );
                break;
            default:
                top += GetTextBlockHeight( text );
                break;
        }
    }

    private static SvgChartTextOptions CreateDefaultTitleOptions()
    {
        return new()
        {
            Font = new()
            {
                Size = 16,
                Weight = "600",
            },
            Opacity = 1,
            Padding = new()
            {
                Top = 8,
            },
        };
    }

    private static SvgChartTextOptions CreateDefaultSubtitleOptions()
    {
        return new()
        {
            Font = new()
            {
                Size = 12,
            },
            Opacity = 0.7,
            Padding = new()
            {
                Top = 7,
            },
        };
    }

    internal static double GetTopTextHeight( SvgChartTextOptions text )
    {
        return IsTextVisible( text ) && text.Position == SvgChartTextPosition.Top
            ? GetTextBlockHeight( text )
            : 0;
    }

    internal static double GetBottomTextHeight( SvgChartTextOptions text )
    {
        return IsTextVisible( text ) && text.Position == SvgChartTextPosition.Bottom
            ? GetTextBlockHeight( text )
            : 0;
    }

    private static double GetStartTextWidth( SvgChartTextOptions text )
    {
        return IsTextVisible( text ) && text.Position == SvgChartTextPosition.Start
            ? GetTextBlockWidth( text )
            : 0;
    }

    private static double GetEndTextWidth( SvgChartTextOptions text )
    {
        return IsTextVisible( text ) && text.Position == SvgChartTextPosition.End
            ? GetTextBlockWidth( text )
            : 0;
    }

    internal static double ResolveStartPadding( SvgChartOptions options, SvgChartRenderModel model, double? padding )
    {
        if ( padding.HasValue )
        {
            return padding.Value;
        }

        if ( model?.PrimaryValueAxis is not null
            && !SvgChartGeometry.IsBarChart( model )
            && !SvgChartGeometry.IsRadialChart( model ) )
        {
            return ResolveValueAxisLabelPadding( options, model.PrimaryValueAxis );
        }

        if ( model is null || !SvgChartGeometry.IsBarChart( model ) )
        {
            return 0;
        }

        var labels = model.CategoryAxis?.Labels ?? new();

        if ( labels.Visible == false )
        {
            return 0;
        }

        var fontSize = ResolveFontSize( options, labels.Font );
        var maxLabelWidth = model.Labels
            .Select( ( label, index ) => SvgChartAxesRenderer.FormatAxisLabel( FormatCategoryLabel( model, label, index < model.CategoryLabelIndexes.Count ? model.CategoryLabelIndexes[index] : index ), labels, options ) )
            .DefaultIfEmpty( string.Empty )
            .Max( label => SvgChartRenderHelpers.EstimateTextWidth( label, fontSize, options, labels.Font ) );

        return Math.Max( 0, maxLabelWidth + ( labels.Offset ?? 10 ) );
    }

    internal static double ResolveBottomPadding( SvgChartOptions options, SvgChartRenderModel model, double? padding, SvgChartPlotArea plot )
    {
        if ( padding.HasValue )
        {
            return padding.Value;
        }

        if ( model is null )
        {
            return 0;
        }

        var labels = SvgChartGeometry.IsBarChart( model ) ? model.PrimaryValueAxis?.Labels : model.CategoryAxis?.Labels;
        labels ??= new();
        var valueLabelExtent = ResolveValueAxisLabelExtent( options, model );

        if ( labels.Visible == false )
        {
            return valueLabelExtent;
        }

        var fontSize = ResolveFontSize( options, labels.Font );
        var labelHeight = fontSize;

        if ( !SvgChartGeometry.IsBarChart( model ) )
        {
            var horizontalLabels = SvgChartAxesRenderer.ResolveHorizontalCategoryLabels( model );
            var placement = SvgChartAxesRenderer.ResolveHorizontalAxisLabelPlacement( model, plot, labels, horizontalLabels.Count, horizontalLabels.Formatter );
            var rotation = Math.Abs( placement.Rotation ) * Math.PI / 180d;
            var labelWidth = Enumerable.Range( 0, horizontalLabels.Count )
                .Select( index => SvgChartAxesRenderer.FormatAxisLabel( horizontalLabels.Formatter( index ), labels, options ) )
                .DefaultIfEmpty( string.Empty )
                .Max( label => SvgChartRenderHelpers.EstimateTextWidth( label, fontSize, options, labels.Font ) );

            labelHeight = labelWidth * Math.Sin( rotation ) + fontSize * Math.Cos( rotation );
        }

        return Math.Max( valueLabelExtent, ( labels.Offset ?? 24 ) + labelHeight );
    }

    private static double ResolveValueAxisLabelExtent( SvgChartOptions options, SvgChartRenderModel model )
    {
        if ( model is null || SvgChartGeometry.IsBarChart( model ) || SvgChartGeometry.IsRadialChart( model ) )
        {
            return 0;
        }

        return model.ValueAxes
            .Where( axis => ( axis == model.PrimaryValueAxis || axis.Position == SvgChartAxisPosition.Right ) && axis.Labels?.Visible != false && axis.Ticks.Count > 0 )
            .Select( axis => Math.Max( 0, ResolveFontSize( options, axis.Labels?.Font ) ) / 2 )
            .DefaultIfEmpty( 0 )
            .Max();
    }

    internal static double ResolveEndPadding( SvgChartOptions options, SvgChartRenderModel model, double? padding )
    {
        if ( padding.HasValue )
        {
            return padding.Value;
        }

        if ( model is null || SvgChartGeometry.IsBarChart( model ) || SvgChartGeometry.IsRadialChart( model ) )
        {
            return 18d;
        }

        var axes = model.ValueAxes
            .Where( axis => axis != model.PrimaryValueAxis && axis.Position == SvgChartAxisPosition.Right )
            .ToList();

        return axes.Count == 0
            ? 18d
            : axes.Sum( axis => ResolveValueAxisLabelPadding( options, axis ) ) + Math.Max( 0, axes.Count - 1 ) * Math.Max( 0, options.Layout?.AxisSpacing ?? 8 );
    }

    internal static double ResolveRightAxisLabelPadding( SvgChartOptions options, SvgChartRenderModel model, SvgChartRenderValueAxis axis )
    {
        if ( options.PlotAreaPadding?.End is not double padding )
        {
            return ResolveValueAxisLabelPadding( options, axis );
        }

        var axes = model.ValueAxes.Where( item => item != model.PrimaryValueAxis && item.Position == SvgChartAxisPosition.Right ).ToList();
        var total = axes.Sum( item => ResolveValueAxisLabelPadding( options, item ) );
        var available = Math.Max( 0, padding - Math.Max( 0, axes.Count - 1 ) * SvgChartAxesRenderer.ResolveRightAxisSpacing( model ) );

        return total > 0 ? available * ResolveValueAxisLabelPadding( options, axis ) / total : available / Math.Max( 1, axes.Count );
    }

    private static double ResolveValueAxisLabelPadding( SvgChartOptions options, SvgChartRenderValueAxis axis )
    {
        var labels = axis.Labels ?? new();

        if ( labels.Visible == false )
        {
            return 0;
        }

        return Math.Max( 0, SvgChartAxesRenderer.EstimateValueAxisLabelsWidth( options, axis ) + ( labels.Offset ?? 10 ) );
    }

    internal static double ResolveAxisTitleReservedSize( SvgChartOptions options, SvgChartAxisTitleOptions title )
    {
        return string.IsNullOrWhiteSpace( title?.Text )
            ? 0
            : ResolveFontSize( options, title.Font ) + title.EffectiveTopPadding + title.EffectiveBottomPadding;
    }

    private static string FormatCategoryLabel( SvgChartRenderModel model, object value, int index )
    {
        return model.CategoryTickFormatter?.Invoke( new()
        {
            Value = value,
            Index = index,
            CategoryAxis = true,
            AxisId = model.CategoryAxis?.Id
        } ) ?? SvgChartRenderHelpers.FormatDataLabelValue( value );
    }

    private static double GetTextBlockHeight( SvgChartTextOptions text )
    {
        var padding = text.Padding ?? new();
        var fontSize = text.Font?.Size ?? 12;

        return padding.Top + fontSize + padding.Bottom;
    }

    private static double GetTextBlockWidth( SvgChartTextOptions text )
    {
        var padding = text.Padding ?? new();
        var fontSize = text.Font?.Size ?? 12;

        return padding.Start + fontSize + padding.End;
    }

    private static double ResolveTextX( SvgChartOptions options, SvgChartTextOptions text, double start, double end, double fontSize )
    {
        var padding = text.Padding ?? new();

        return text.Position switch
        {
            SvgChartTextPosition.Start => start + padding.Start + fontSize,
            SvgChartTextPosition.End => options.Width - end - padding.End - fontSize,
            _ => padding.Start + ResolveTextAlignmentOffset( options.Width - padding.Start - padding.End, text.Alignment ?? SvgChartTextAlignment.Center )
        };
    }

    private static double ResolveTextY( SvgChartOptions options, SvgChartTextOptions text, double top, double bottom, double fontSize )
    {
        var padding = text.Padding ?? new();

        return text.Position switch
        {
            SvgChartTextPosition.Bottom => options.Height - bottom - padding.Bottom,
            SvgChartTextPosition.Start => options.Height - padding.Bottom - ResolveTextAlignmentOffset( options.Height - padding.Top - padding.Bottom, text.Alignment ?? SvgChartTextAlignment.Center ),
            SvgChartTextPosition.End => padding.Top + ResolveTextAlignmentOffset( options.Height - padding.Top - padding.Bottom, text.Alignment ?? SvgChartTextAlignment.Center ),
            _ => top + padding.Top + fontSize
        };
    }

    private static double ResolveTextAlignmentOffset( double size, SvgChartTextAlignment alignment )
    {
        return alignment switch
        {
            SvgChartTextAlignment.Start => 0,
            SvgChartTextAlignment.End => size,
            _ => size / 2
        };
    }

    private static string ResolveTextAnchor( SvgChartTextAlignment alignment )
    {
        return alignment switch
        {
            SvgChartTextAlignment.Start => "start",
            SvgChartTextAlignment.End => "end",
            _ => "middle"
        };
    }

    private static string ResolveTextColor( SvgChartOptions options, SvgChartTextOptions text )
    {
        var textColor = text?.Font?.Color;
        return SvgChartRenderHelpers.ResolveFontColor( SvgChartRenderHelpers.IsDefaultColor( textColor ) ? options?.Font?.Color : textColor );
    }

    #endregion
}