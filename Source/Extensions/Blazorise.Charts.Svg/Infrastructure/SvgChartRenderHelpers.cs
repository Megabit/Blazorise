#region Using directives
using System;
using System.Globalization;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal static class SvgChartRenderHelpers
{
    #region Members

    private static readonly string[] Palette =
    [
        "#4c6ef5",
        "#12b886",
        "#f59f00",
        "#e03131",
        "#845ef7",
        "#228be6"
    ];

    #endregion

    #region Methods

    public static void AddFontFamilyAttribute( RenderTreeBuilder builder, ref int sequence, string fontFamily )
    {
        if ( !string.IsNullOrWhiteSpace( fontFamily ) )
            builder.AddAttribute( sequence++, "font-family", fontFamily );
    }

    public static string ResolveColor( Color color, int index )
    {
        if ( IsDefaultColor( color ) )
            return Palette[index % Palette.Length];

        var name = color.Name;
        var fallback = ResolveColorFallback( name, index );

        if ( name.StartsWith( "#", StringComparison.Ordinal )
             || name.StartsWith( "rgb", StringComparison.OrdinalIgnoreCase )
             || name.StartsWith( "hsl", StringComparison.OrdinalIgnoreCase )
             || name.StartsWith( "var(", StringComparison.OrdinalIgnoreCase ) )
            return name;

        return $"var(--b-theme-{name}, var(--bs-{name}, {fallback}))";
    }

    public static string ResolveFontColor( SvgChartFontOptions font )
    {
        return ResolveFontColor( font?.Color );
    }

    public static string ResolveFontColor( Color color )
    {
        if ( IsDefaultColor( color ) )
            return "currentColor";

        return ResolveColor( color, 0 );
    }

    public static bool IsDefaultColor( Color color )
    {
        return color is null || color == Color.Default || string.IsNullOrWhiteSpace( color.Name );
    }

    public static string FormatTick( double value )
    {
        return value.ToString( "0.##", CultureInfo.InvariantCulture );
    }

    public static string Format( double value )
    {
        return value.ToString( "0.###", CultureInfo.InvariantCulture );
    }

    public static string FormatDuration( TimeSpan value )
    {
        return $"{value.TotalSeconds.ToString( "0.###", CultureInfo.InvariantCulture )}s";
    }

    public static string FormatDataLabelValue( object value )
    {
        return value switch
        {
            null => null,
            double doubleValue => FormatTick( doubleValue ),
            float floatValue => FormatTick( floatValue ),
            decimal decimalValue => decimalValue.ToString( "0.##", CultureInfo.InvariantCulture ),
            _ => value.ToString()
        };
    }

    public static double EstimateTextWidth( string text, double fontSize )
    {
        return Math.Max( 1, ( text?.Length ?? 0 ) * fontSize * 0.58 );
    }

    private static string ResolveColorFallback( string name, int index )
    {
        return name?.ToLowerInvariant() switch
        {
            "primary" => Palette[0],
            "secondary" => "#868e96",
            "success" => Palette[1],
            "danger" => Palette[3],
            "warning" => Palette[2],
            "info" => Palette[5],
            "light" => "#f8f9fa",
            "dark" => "#343a40",
            "link" => Palette[0],
            _ => Palette[index % Palette.Length]
        };
    }

    #endregion
}