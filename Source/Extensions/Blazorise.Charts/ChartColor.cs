#region Using directives
using System;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Blazorise.Utilities;
#endregion

namespace Blazorise.Charts;

public struct ChartColor
{
    #region Constructors

    public ChartColor( byte red, byte green, byte blue )
        : this( red, green, blue, 1f )
    {
    }

    public ChartColor( byte red, byte green, byte blue, float alpha )
    {
        R = red;
        G = green;
        B = blue;
        A = alpha;
    }

    public ChartColor( float red, float green, float blue )
        : this( red, green, blue, 1f )
    {
    }

    public ChartColor( float red, float green, float blue, float alpha )
    {
        R = (byte)( red * 255 );
        G = (byte)( green * 255 );
        B = (byte)( blue * 255 );
        A = alpha;
    }

    #endregion

    #region Operators

    /// <summary>
    /// Implicitly converts from string value to <see cref="ChartColor"/>.
    /// </summary>
    /// <param name="value">String value.</param>
    public static implicit operator ChartColor( string value )
    {
        if ( value.StartsWith( '#' ) )
            return HexStringToColor( value );
        else if ( value.StartsWith( "rgb" ) )
            return CssRgbaFunctionToColor( value );

        throw new FormatException( $"Invalid chart color format: {value}" );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Checks for characters that are Hexadecimal
    /// </summary>
    private static Regex IsHexDigit = new( "[abcdefABCDEF\\d]+", RegexOptions.Compiled );

    /// <summary>
    /// Extract only the hex digits from a string.
    /// </summary>
    /// <param name="input">A string to extract.</param>
    /// <returns>A new hex string.</returns>
    private static string ExtractHexDigits( string input )
    {
        var sb = new StringBuilder();
        var result = IsHexDigit.Matches( input );

        foreach ( System.Text.RegularExpressions.Match item in result )
        {
            sb.Append( item.Value );
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts the hexadecimal string into a <see cref="System.Drawing.Color">Color</see> value.
    /// </summary>
    /// <param name="hexColor">A color represented as hexadecimal string.</param>
    /// <returns>Parsed color value or <see cref="System.Drawing.Color.Empty">Empty</see> if failed.</returns>
    private static ChartColor HexStringToColor( string hexColor )
    {
        var hc = ExtractHexDigits( hexColor );

        if ( hc.Length == 3 )
            hc = string.Format( "{0}{0}{1}{1}{2}{2}", hc[0], hc[1], hc[2] );

        if ( hc.Length < 6 )
            return Empty;

        try
        {
            var r = byte.Parse( hc.Substring( 0, 2 ), NumberStyles.HexNumber );
            var g = byte.Parse( hc.Substring( 2, 2 ), NumberStyles.HexNumber );
            var b = byte.Parse( hc.Substring( 4, 2 ), NumberStyles.HexNumber );

            if ( hc.Length == 8 )
            {
                var a = byte.Parse( hc.Substring( 6, 2 ), NumberStyles.HexNumber );

                return new ChartColor( r, g, b, a / 255f );
            }

            return new ChartColor( r, g, b );
        }
        catch
        {
            return Empty;
        }
    }

    /// <summary>
    /// Converts the function call into into a <see cref="System.Drawing.Color">Color</see> value.
    /// </summary>
    /// <param name="cssColor">A color represented as (rgb or rgba) function call.</param>
    /// <returns>Parsed color value or <see cref="System.Drawing.Color.Empty">Empty</see> if failed.</returns>
    private static ChartColor CssRgbaFunctionToColor( string cssColor )
    {
        int left = cssColor.IndexOf( '(' );
        int right = cssColor.IndexOf( ')' );

        if ( 0 > left || 0 > right )
            throw new FormatException( $"Invalid rgb or rgba function format: {cssColor}" );

        var noBrackets = cssColor.Substring( left + 1, right - left - 1 );

        var parts = noBrackets.Split( ',' );

        if ( parts.Length < 3 )
            throw new FormatException( $"Invalid rgb format: {cssColor}" );

        var r = byte.Parse( parts[0], CultureInfo.InvariantCulture );
        var g = byte.Parse( parts[1], CultureInfo.InvariantCulture );
        var b = byte.Parse( parts[2], CultureInfo.InvariantCulture );

        if ( 3 == parts.Length )
        {
            return new ChartColor( r, g, b );
        }
        else if ( 4 == parts.Length )
        {
            var a = float.Parse( parts[3], CultureInfo.InvariantCulture );

            return new ChartColor( r, g, b, a );
        }

        return Empty;
    }

    /// <summary>
    /// Implicitly convert color to the string representation that is understood by the ChartJs.
    /// </summary>
    /// <param name="color">The string formated color.</param>
    public static implicit operator string( ChartColor color ) => color.ToJsRgba();

    /// <summary>
    /// Creates the new color based on the supplied color component values.
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static ChartColor FromRgba( byte red, byte green, byte blue, float alpha ) => new( red, green, blue, alpha );

    /// <summary>
    /// Creates a new color based on the supplied HTML color code.
    /// </summary>
    /// <param name="code">The HTML color code to parse</param>
    /// <returns><see cref="ChartColor"/></returns>
    public static ChartColor FromHtmlColorCode( string code )
    {
        if ( code == null )
        {
            throw new ArgumentNullException( nameof( code ) );
        }

        if ( HtmlColorCodeParser.TryParse( code, out var red, out var green, out var blue ) )
        {
            return new( red, green, blue );
        }

        throw new ArgumentException( $"The \"{code}\" doesn't represent a valid HTML color code.", nameof( code ) );
    }

    /// <summary>
    /// Converts the color to the js function call.
    /// </summary>
    /// <returns></returns>
    public string ToJsRgba() => $"rgba({R},{G},{B},{( A ?? 0 ).ToString( CultureInfo.InvariantCulture )})";

    /// <inheritdoc/>
    public override string ToString() => ToJsRgba();

    #endregion

    #region Properties

    /// <summary>
    /// Represents a color that is null.
    /// </summary>
    public static readonly ChartColor Empty = new ChartColor( 0, 0, 0, 0 );

    /// <summary>
    /// Gets or sets the red component value of color structure.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public byte? R { get; set; } = 0;

    /// <summary>
    /// Gets or sets the green component value of color structure.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public byte? G { get; set; } = 0;

    /// <summary>
    /// Gets or sets the blue component value of color structure.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public byte? B { get; set; } = 0;

    /// <summary>
    /// Gets or sets the alpha component value of color structure.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public float? A { get; set; } = 0;

    #endregion
}