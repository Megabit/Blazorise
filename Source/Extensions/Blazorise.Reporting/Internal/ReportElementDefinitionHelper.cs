#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportElementDefinitionHelper
{
    #region Methods

    internal static string BuildStyle( ReportElementDefinition element )
    {
        var font = element.Font;
        var appearance = element.Appearance;
        var border = element.Border;
        var styles = new List<string>
        {
            $"left:{element.X}px",
            $"top:{element.Y}px",
            $"width:{element.Width}px",
            $"height:{element.Height}px",
        };

        if ( !string.IsNullOrWhiteSpace( font?.Family ) )
            styles.Add( $"font-family:{font.Family}" );

        if ( font?.Size is > 0 )
            styles.Add( $"font-size:{font.Size.Value}px" );

        if ( !string.IsNullOrWhiteSpace( font?.Color ) )
            styles.Add( $"color:{font.Color}" );

        if ( !string.IsNullOrWhiteSpace( appearance?.BackgroundColor ) )
            styles.Add( $"background-color:{appearance.BackgroundColor}" );

        if ( font?.Bold == true )
            styles.Add( "font-weight:700" );

        if ( font?.Italic == true )
            styles.Add( "font-style:italic" );

        if ( font?.Underline == true )
            styles.Add( "text-decoration:underline" );

        var textAlignment = ToCssTextAlignment( font?.Alignment ?? TextAlignment.Default );

        if ( textAlignment is not null )
            styles.Add( $"text-align:{textAlignment}" );

        if ( !string.IsNullOrWhiteSpace( border?.Color ) )
            styles.Add( $"border-color:{border.Color}" );

        if ( border?.Width is >= 0 )
        {
            styles.Add( $"border-width:{border.Width.Value}px" );
            styles.Add( "border-style:solid" );
        }

        if ( border?.Radius is >= 0 )
            styles.Add( $"border-radius:{border.Radius.Value}px" );

        if ( appearance?.Opacity is >= 0 and <= 1 )
            styles.Add( $"opacity:{appearance.Opacity.Value.ToString( CultureInfo.InvariantCulture )}" );

        if ( !string.IsNullOrWhiteSpace( element.Style ) )
            styles.Add( element.Style.Trim().TrimEnd( ';' ) );

        return string.Join( ";", styles ) + ";";
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