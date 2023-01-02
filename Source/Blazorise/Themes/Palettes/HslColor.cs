using System;

namespace Blazorise;

/// <summary>
/// HlsColor represents colors by their Hue-Saturation-Luminosity value rather than the traditional RGB value.
/// </summary>
public struct HslColor
{
    /// <summary>
    /// Creates instance of the class with the specified hue, saturation, and luminosity.
    /// </summary>
    /// <param name="hue">Starting hue value.</param>
    /// <param name="saturation">Starting saturation value.</param>
    /// <param name="luminosity">Starting luminosity value.</param>
    public HslColor( double hue, double saturation, double luminosity )
    {
        Hue = hue;
        Saturation = saturation;
        Luminosity = luminosity;
    }

    /// <summary>
    /// Gets or sets the Hue property.
    /// </summary>
    public double Hue { get; set; }

    /// <summary>
    /// Gets or sets the Saturation property.
    /// </summary>
    public double Saturation { get; set; }

    /// <summary>
    /// Gets or sets the Luminosity property.
    /// </summary>
    public double Luminosity { get; set; }

    /// <summary>
    /// Represents a color that is null.-
    /// </summary>
    public static readonly HslColor Empty = new HslColor( 0, 0, 0 );

    /// <summary>
    /// Converts an HslColor value to a Color.
    /// </summary>
    /// <returns>Converted color.</returns>
    public System.Drawing.Color ToColor()
    {
        var h = Hue;
        var s = Saturation / 100;
        var l = Luminosity / 100;

        var c = ( 1 - Math.Abs( 2 * l - 1 ) ) * s;
        var x = c * ( 1 - Math.Abs( ( ( h / 60 ) % 2 ) - 1 ) );
        var m = l - c / 2;
        var r = 0d;
        var g = 0d;
        var b = 0d;

        if ( h >= 0 && h < 60 )
        {
            r = c;
            g = x;
            b = 0;
        }
        else if ( h >= 60 && h < 120 )
        {
            r = x;
            g = c;
            b = 0;
        }
        else if ( h >= 120 && h < 180 )
        {
            r = 0;
            g = c;
            b = x;
        }
        else if ( h >= 180 && h < 240 )
        {
            r = 0;
            g = x;
            b = c;
        }
        else if ( h >= 240 && h < 300 )
        {
            r = x;
            g = 0;
            b = c;
        }
        else if ( h >= 300 && h < 360 )
        {
            r = c;
            g = 0;
            b = x;
        }

        return System.Drawing.Color.FromArgb( 255,
            (int)Math.Round( ( r + m ) * 255 ),
            (int)Math.Round( ( g + m ) * 255 ),
            (int)Math.Round( ( b + m ) * 255 ) );
    }
}