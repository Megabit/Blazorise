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

        return resolved;
    }

    public static SvgChartTextOptions ResolveSubtitleOptions( SvgChartOptions options, IReadOnlyList<SvgChartTitle> titleComponents )
    {
        var titleComponent = titleComponents?.LastOrDefault();
        var resolved = SvgChartOptionsMapper.CreateTextOptions( CreateDefaultSubtitleOptions(), options?.Subtitle );
        var componentOptions = titleComponent?.Subtitle;

        if ( componentOptions is not null )
            resolved = SvgChartOptionsMapper.CreateTextOptions( resolved, componentOptions );

        return resolved;
    }

    public static SvgChartPlotArea BuildPlotArea( SvgChartOptions options, SvgChartTextOptions title, SvgChartTextOptions subtitle, bool hasTopLegend, bool hasBottomLegend, SvgChartRenderModel model = null )
    {
        options ??= new();

        var padding = options.PlotAreaPadding;
        var topPadding = padding?.Top ?? 24d;
        var endPadding = padding?.End ?? 18d;
        var bottomPadding = ResolveBottomPadding( options, model, padding?.Bottom );
        var startPadding = ResolveStartPadding( options, model, padding?.Start );
        var top = topPadding + GetTopTextHeight( title ) + GetTopTextHeight( subtitle );

        if ( hasTopLegend )
            top += SvgChartLegendRenderer.ResolveReservedHeight( model, options, SvgChartLegendPosition.Top );

        var bottom = options.Height - bottomPadding - ( hasBottomLegend ? SvgChartLegendRenderer.ResolveReservedHeight( model, options, SvgChartLegendPosition.Bottom ) : 0 ) - GetBottomTextHeight( title ) - GetBottomTextHeight( subtitle );
        var left = startPadding + GetStartTextWidth( title ) + GetStartTextWidth( subtitle );
        var right = options.Width - endPadding - GetEndTextWidth( title ) - GetEndTextWidth( subtitle );

        return new()
        {
            Left = left,
            Top = top,
            Right = right,
            Bottom = bottom
        };
    }

    public static bool IsTextVisible( SvgChartTextOptions text )
    {
        return text?.Visible == true && !string.IsNullOrWhiteSpace( text.Text );
    }

    public static void AddFontAttributes( RenderTreeBuilder builder, ref int sequence, SvgChartOptions options, double fallbackSize = 11, double? opacity = null )
    {
        var font = options?.Font;

        builder.AddAttribute( sequence++, "font-size", SvgChartRenderHelpers.Format( font?.Size ?? fallbackSize ) );
        SvgChartRenderHelpers.AddFontFamilyAttribute( builder, ref sequence, font?.Family );
        builder.AddAttribute( sequence++, "fill", SvgChartRenderHelpers.ResolveFontColor( font ) );

        if ( !string.IsNullOrWhiteSpace( font?.Weight ) )
            builder.AddAttribute( sequence++, "font-weight", font.Weight );

        if ( opacity is not null )
            builder.AddAttribute( sequence++, "opacity", SvgChartRenderHelpers.Format( Math.Clamp( opacity.Value, 0, 1 ) ) );
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
        builder.AddAttribute( sequence++, "text-anchor", ResolveTextAnchor( text.Alignment ) );
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

    private static double GetTopTextHeight( SvgChartTextOptions text )
    {
        return IsTextVisible( text ) && text.Position == SvgChartTextPosition.Top
            ? GetTextBlockHeight( text )
            : 0;
    }

    private static double GetBottomTextHeight( SvgChartTextOptions text )
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

    private static double ResolveStartPadding( SvgChartOptions options, SvgChartRenderModel model, double? padding )
    {
        if ( padding.HasValue )
            return padding.Value;

        const double fallback = 52d;

        if ( model?.Type != SvgChartType.Bar || model.CategoryAxis?.Labels?.Visible == false )
            return fallback;

        var fontSize = options?.Font?.Size ?? 11;
        var maxLabelWidth = model.Labels
            .Select( ( label, index ) => FormatCategoryLabel( model, label, index ) )
            .DefaultIfEmpty( string.Empty )
            .Max( label => SvgChartRenderHelpers.EstimateTextWidth( label, fontSize ) );

        return Math.Max( fallback, Math.Min( maxLabelWidth + 14, options.Width * 0.45 ) );
    }

    private static double ResolveBottomPadding( SvgChartOptions options, SvgChartRenderModel model, double? padding )
    {
        if ( padding.HasValue )
            return padding.Value;

        if ( model?.Type == SvgChartType.Bar && model.PrimaryValueAxis?.Labels?.Visible == false )
            return 18d;

        if ( model is not null && model.Type != SvgChartType.Bar && model.CategoryAxis?.Labels?.Visible == false )
            return 18d;

        const double fallback = 42d;

        var labels = model?.CategoryAxis?.Labels;

        if ( model is null || model.Type == SvgChartType.Bar || labels?.AutoSkip != true || !labels.AutoRotate || labels.MaxRotation <= 0 )
            return fallback;

        var fontSize = options?.Font?.Size ?? 11;
        var maxLabelWidth = model.Labels
            .Select( ( label, index ) => FormatCategoryLabel( model, label, index ) )
            .DefaultIfEmpty( string.Empty )
            .Max( label => SvgChartRenderHelpers.EstimateTextWidth( label, fontSize ) );

        if ( labels.MaxWidth.HasValue )
            maxLabelWidth = Math.Min( maxLabelWidth, Math.Max( 1, labels.MaxWidth.Value ) );

        var rotation = Math.Clamp( labels.MaxRotation, 0, 90 ) * Math.PI / 180d;
        var rotatedHeight = maxLabelWidth * Math.Sin( rotation ) + fontSize * Math.Cos( rotation );

        return Math.Max( fallback, Math.Min( labels.Offset + rotatedHeight + 8, options.Height * 0.35 ) );
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
            _ => padding.Start + ResolveTextAlignmentOffset( options.Width - padding.Start - padding.End, text.Alignment )
        };
    }

    private static double ResolveTextY( SvgChartOptions options, SvgChartTextOptions text, double top, double bottom, double fontSize )
    {
        var padding = text.Padding ?? new();

        return text.Position switch
        {
            SvgChartTextPosition.Bottom => options.Height - bottom - padding.Bottom,
            SvgChartTextPosition.Start => options.Height - padding.Bottom - ResolveTextAlignmentOffset( options.Height - padding.Top - padding.Bottom, text.Alignment ),
            SvgChartTextPosition.End => padding.Top + ResolveTextAlignmentOffset( options.Height - padding.Top - padding.Bottom, text.Alignment ),
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