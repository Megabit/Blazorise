#region Using directives
using System;
using System.ComponentModel;
using System.Globalization;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Represents a report-owned color that can be resolved consistently for preview, printing, and PDF export.
/// </summary>
[TypeConverter( typeof( ReportColorTypeConverter ) )]
public readonly record struct ReportColor
{
    #region Constructors

    private ReportColor( ReportColorKind kind, string name, byte red, byte green, byte blue, double alpha )
    {
        Kind = kind;
        Name = name;
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = Math.Clamp( alpha, 0, 1 );
    }

    #endregion

    #region Operators

    /// <summary>
    /// Converts a CSS color value or report color name into a report color.
    /// </summary>
    /// <param name="value">Color value to parse.</param>
    public static implicit operator ReportColor( string value )
    {
        return FromString( value );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a report color from a named print color.
    /// </summary>
    /// <param name="name">Named color identifier.</param>
    /// <param name="alpha">Color opacity from 0 to 1.</param>
    /// <returns>A named report color.</returns>
    public static ReportColor FromName( string name, double alpha = 1 )
    {
        return string.IsNullOrWhiteSpace( name )
            ? Default
            : new( ReportColorKind.Named, name.Trim(), 0, 0, 0, alpha );
    }

    /// <summary>
    /// Creates a report color from red, green, blue, and alpha components.
    /// </summary>
    /// <param name="red">Red component from 0 to 255.</param>
    /// <param name="green">Green component from 0 to 255.</param>
    /// <param name="blue">Blue component from 0 to 255.</param>
    /// <param name="alpha">Color opacity from 0 to 1.</param>
    /// <returns>An RGB report color.</returns>
    public static ReportColor FromRgb( byte red, byte green, byte blue, double alpha = 1 )
    {
        return new( ReportColorKind.Rgb, null, red, green, blue, alpha );
    }

    /// <summary>
    /// Creates a report color from a hex color string.
    /// </summary>
    /// <param name="value">Hex color value in #RGB, #RGBA, #RRGGBB, or #RRGGBBAA form.</param>
    /// <returns>An RGB report color, or <see cref="Default"/> when parsing fails.</returns>
    public static ReportColor FromHex( string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return Default;

        string hex = value.Trim().TrimStart( '#' );

        if ( hex.Length == 3 )
            hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
        else if ( hex.Length == 4 )
            hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}{hex[3]}{hex[3]}";

        if ( hex.Length != 6 && hex.Length != 8 )
            return Default;

        if ( !TryParseHexComponent( hex, 0, out byte red ) || !TryParseHexComponent( hex, 2, out byte green ) || !TryParseHexComponent( hex, 4, out byte blue ) )
            return Default;

        double alpha = 1;

        if ( hex.Length == 8 )
        {
            if ( !TryParseHexComponent( hex, 6, out byte alphaValue ) )
                return Default;

            alpha = alphaValue / 255d;
        }

        return FromRgb( red, green, blue, alpha );
    }

    /// <summary>
    /// Creates a report color from a CSS color value or report color name.
    /// </summary>
    /// <param name="value">Color value to parse.</param>
    /// <returns>A report color.</returns>
    public static ReportColor FromString( string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return Default;

        string trimmedValue = value.Trim();

        if ( string.Equals( trimmedValue, "transparent", StringComparison.OrdinalIgnoreCase ) )
            return Transparent;

        if ( trimmedValue.StartsWith( "#", StringComparison.Ordinal ) )
            return FromHex( trimmedValue );

        if ( TryParseRgbFunction( trimmedValue, out ReportColor rgbColor ) )
            return rgbColor;

        return FromName( trimmedValue );
    }

    /// <summary>
    /// Converts the report color to a CSS color value using the default report color resolver.
    /// </summary>
    /// <returns>A CSS-compatible color value, or <c>null</c> when no color is explicitly applied.</returns>
    public string ToCssString()
    {
        return ReportDefaultColorResolver.Instance.Resolve( this ).ToCssString();
    }

    private static bool TryParseHexComponent( string value, int startIndex, out byte component )
    {
        return byte.TryParse( value.Substring( startIndex, 2 ), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out component );
    }

    private static bool TryParseRgbFunction( string value, out ReportColor color )
    {
        color = Default;

        if ( !value.StartsWith( "rgb", StringComparison.OrdinalIgnoreCase ) )
            return false;

        int startIndex = value.IndexOf( '(' );
        int endIndex = value.LastIndexOf( ')' );

        if ( startIndex < 0 || endIndex <= startIndex )
            return false;

        string[] components = value.Substring( startIndex + 1, endIndex - startIndex - 1 ).Split( ',', StringSplitOptions.TrimEntries );

        if ( components.Length < 3 )
            return false;

        if ( !TryParseColorComponent( components[0], out byte red ) || !TryParseColorComponent( components[1], out byte green ) || !TryParseColorComponent( components[2], out byte blue ) )
            return false;

        double alpha = 1;

        if ( components.Length > 3 && !TryParseAlphaComponent( components[3], out alpha ) )
            return false;

        color = FromRgb( red, green, blue, alpha );

        return true;
    }

    private static bool TryParseColorComponent( string value, out byte component )
    {
        component = 0;

        if ( string.IsNullOrWhiteSpace( value ) )
            return false;

        string trimmedValue = value.Trim();
        bool percentage = trimmedValue.EndsWith( "%", StringComparison.Ordinal );

        if ( percentage )
            trimmedValue = trimmedValue[..^1];

        if ( !double.TryParse( trimmedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue ) )
            return false;

        component = Convert.ToByte( Math.Clamp( percentage ? parsedValue * 2.55d : parsedValue, 0, 255 ) );

        return true;
    }

    private static bool TryParseAlphaComponent( string value, out double alpha )
    {
        alpha = 1;

        if ( string.IsNullOrWhiteSpace( value ) )
            return false;

        string trimmedValue = value.Trim();
        bool percentage = trimmedValue.EndsWith( "%", StringComparison.Ordinal );

        if ( percentage )
            trimmedValue = trimmedValue[..^1];

        if ( !double.TryParse( trimmedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue ) )
            return false;

        alpha = Math.Clamp( percentage ? parsedValue / 100d : parsedValue, 0, 1 );

        return true;
    }

    #endregion

    #region Properties

    /// <summary>
    /// No color is explicitly applied.
    /// </summary>
    public static ReportColor Default { get; } = new( ReportColorKind.Default, null, 0, 0, 0, 1 );

    /// <summary>
    /// Fully transparent color.
    /// </summary>
    public static ReportColor Transparent { get; } = new( ReportColorKind.Transparent, "Transparent", 0, 0, 0, 0 );

    /// <summary>
    /// Storage kind used by this color.
    /// </summary>
    public ReportColorKind Kind { get; }

    /// <summary>
    /// Named report color identifier.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Red component for explicit RGB colors.
    /// </summary>
    public byte Red { get; }

    /// <summary>
    /// Green component for explicit RGB colors.
    /// </summary>
    public byte Green { get; }

    /// <summary>
    /// Blue component for explicit RGB colors.
    /// </summary>
    public byte Blue { get; }

    /// <summary>
    /// Color opacity from 0 to 1.
    /// </summary>
    public double Alpha { get; }

    /// <summary>
    /// Indicates whether no color is explicitly applied.
    /// </summary>
    public bool IsDefault => Kind == ReportColorKind.Default;

    #endregion
}