#region Using directives
using System;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Represents the editable HSV color and its opacity independently of the browser.
/// </summary>
/// <param name="Hue">Hue in degrees, from 0 to 360.</param>
/// <param name="Saturation">Saturation as a percentage, from 0 to 100.</param>
/// <param name="Brightness">Brightness as a percentage, from 0 to 100.</param>
/// <param name="Alpha">Opacity, from 0 to 1.</param>
internal readonly record struct ColorPickerColor( double Hue, double Saturation, double Brightness, double Alpha )
{
    /// <summary>
    /// Attempts to parse a CSS color into its HSV channels and opacity.
    /// </summary>
    /// <param name="value">The CSS color to parse.</param>
    /// <param name="color">The parsed color when parsing succeeds.</param>
    /// <returns><see langword="true"/> when the color can be parsed; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse( string value, out ColorPickerColor color )
    {
        color = default;

        if ( !HtmlColorCodeParser.TryParse( value, out var parsedColor, out var alpha ) )
        {
            return false;
        }

        var maximum = Math.Max( parsedColor.R, Math.Max( parsedColor.G, parsedColor.B ) );
        var minimum = Math.Min( parsedColor.R, Math.Min( parsedColor.G, parsedColor.B ) );

        color = new(
            parsedColor.GetHue(),
            maximum == 0 ? 0 : ( maximum - minimum ) * 100d / maximum,
            maximum * 100d / 255,
            alpha );

        return true;
    }

    /// <summary>
    /// Formats the color as a hexadecimal CSS value.
    /// </summary>
    public string ToHexString() => ColorUtilities.ToHex( ToColor() );

    /// <summary>
    /// Formats the color as a CSS rgba() value.
    /// </summary>
    public string ToRgbaString()
    {
        var color = ToColor();

        return CssColor.Rgba( color.R, color.G, color.B, Alpha );
    }

    /// <summary>
    /// Converts the HSV channels and opacity to an ARGB color with byte channel values.
    /// </summary>
    private System.Drawing.Color ToColor()
    {
        var hue = ( Hue % 360 + 360 ) % 360 / 60;
        var saturation = Math.Clamp( Saturation, 0, 100 ) / 100;
        var brightness = Math.Clamp( Brightness, 0, 100 ) / 100;
        var chroma = brightness * saturation;
        var secondary = chroma * ( 1 - Math.Abs( hue % 2 - 1 ) );
        var offset = brightness - chroma;

        var (red, green, blue) = hue switch
        {
            < 1 => (chroma, secondary, 0d),
            < 2 => (secondary, chroma, 0d),
            < 3 => (0d, chroma, secondary),
            < 4 => (0d, secondary, chroma),
            < 5 => (secondary, 0d, chroma),
            _ => (chroma, 0d, secondary),
        };

        return System.Drawing.Color.FromArgb(
            (int)Math.Round( Math.Clamp( Alpha, 0, 1 ) * 255 ),
            (int)Math.Round( ( red + offset ) * 255 ),
            (int)Math.Round( ( green + offset ) * 255 ),
            (int)Math.Round( ( blue + offset ) * 255 ) );
    }
}