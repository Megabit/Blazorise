#region Using directives
using System;
using System.Globalization;
using Blazorise.Utilities;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportElementDefinitionHelper
{
    #region Methods

    internal static void BuildStyle( StyleBuilder builder, ReportElementDefinition element )
    {
        var font = element.Font;
        var appearance = element.Appearance;
        var border = element.Border;

        builder.Append( $"left:{element.X}px" );
        builder.Append( $"top:{element.Y}px" );
        builder.Append( $"width:{element.Width}px" );
        builder.Append( $"height:{element.Height}px" );

        if ( !string.IsNullOrWhiteSpace( font?.Family ) )
            builder.Append( $"font-family:{font.Family}" );

        if ( font?.Size is > 0 )
            builder.Append( $"font-size:{font.Size.Value}px" );

        if ( !string.IsNullOrWhiteSpace( font?.Color ) )
            builder.Append( $"color:{font.Color}" );

        if ( !string.IsNullOrWhiteSpace( appearance?.BackgroundColor ) )
            builder.Append( $"background-color:{appearance.BackgroundColor}" );

        builder.Append( "font-weight:700", font?.Bold == true );
        builder.Append( "font-style:italic", font?.Italic == true );
        builder.Append( "text-decoration:underline", font?.Underline == true );

        var textAlignment = ToCssTextAlignment( font?.Alignment ?? TextAlignment.Default );

        if ( textAlignment is not null )
            builder.Append( $"text-align:{textAlignment}" );

        if ( !string.IsNullOrWhiteSpace( border?.Color ) )
            builder.Append( $"border-color:{border.Color}" );

        if ( border?.Width is >= 0 )
        {
            builder.Append( $"border-width:{border.Width.Value}px" );
            builder.Append( "border-style:solid" );
        }

        if ( border?.Radius is >= 0 )
            builder.Append( $"border-radius:{border.Radius.Value}px" );

        if ( appearance?.Opacity is >= 0 and <= 1 )
            builder.Append( $"opacity:{appearance.Opacity.Value.ToString( CultureInfo.InvariantCulture )}" );

        if ( !string.IsNullOrWhiteSpace( element.Style ) )
            builder.Append( element.Style.Trim().TrimEnd( ';' ) );
    }

    internal static double? NormalizeNullablePositiveNumber( double? value )
    {
        return value is > 0 ? value : null;
    }

    internal static double? NormalizeOpacity( double? value )
    {
        if ( value is null )
            return null;

        return Math.Clamp( value.Value, 0, 1 );
    }

    internal static ReportFontDefinition EnsureFont( ReportElementDefinition element )
    {
        return element.Font ??= new();
    }

    internal static ReportAppearanceDefinition EnsureAppearance( ReportElementDefinition element )
    {
        return element.Appearance ??= new();
    }

    internal static ReportBorderDefinition EnsureBorder( ReportElementDefinition element )
    {
        return element.Border ??= new();
    }

    internal static string GetDisplayText( ReportElementDefinition element )
    {
        if ( element is null )
            return string.Empty;

        return element.Type == ReportElementType.Field
            ? ReportExpressionFormatter.FormatFieldExpression( element )
            : element.Text ?? element.Name ?? element.Type.ToString();
    }

    internal static string GetDisplayText( ReportDefinition definition, ReportElementDefinition element )
    {
        if ( element is null )
            return string.Empty;

        return element.Type == ReportElementType.Field
            ? ReportExpressionFormatter.FormatFieldExpression( definition, element )
            : element.Text ?? element.Name ?? element.Type.ToString();
    }

    internal static string NormalizeColorValue( string value )
    {
        return !string.IsNullOrWhiteSpace( value ) && value.StartsWith( "#", StringComparison.Ordinal ) && value.Length == 7
            ? value
            : "#000000";
    }

    private static string ToCssTextAlignment( TextAlignment alignment )
    {
        return alignment switch
        {
            TextAlignment.Start => "left",
            TextAlignment.End => "right",
            TextAlignment.Center => "center",
            TextAlignment.Justified => "justify",
            _ => null,
        };
    }

    #endregion
}