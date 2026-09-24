#region Using directives
using System;
using System.Linq;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Provides color calculations and formatting independently of theme generation.
/// </summary>
public static class ColorUtilities
{
    /// <summary>
    /// Converts the RGBA to RGB color format.
    /// </summary>
    /// <param name="background">The background color of the system.</param>
    /// <param name="color">The color to convert.</param>
    /// <param name="customAlpha">Alpha component of a new color value.</param>
    /// <returns>A blend of all the supplied color value.</returns>
    public static System.Drawing.Color Rgba2Rgb( System.Drawing.Color background, System.Drawing.Color color, float? customAlpha = null )
    {
        float alpha = customAlpha ?? color.A / byte.MaxValue;

        return System.Drawing.Color.FromArgb(
            (int)( ( 1 - alpha ) * background.R + alpha * color.R ),
            (int)( ( 1 - alpha ) * background.G + alpha * color.G ),
            (int)( ( 1 - alpha ) * background.B + alpha * color.B )
        );
    }

    /// <summary>
    /// Gets the relative brightness of any point in a colorspace, on a scale from 0 for black to 100 for white.
    /// </summary>
    /// <param name="color">The color from which to calculate luminance.</param>
    /// <returns>Returns the relative brightness of any point in a colorspace, on a scale from 0 for black to 100 for white.</returns>
    public static double LuminanceFromColor( System.Drawing.Color color )
    {
        // Formula from WCAG 2.0
        double[] rgb = new double[] { color.R, color.G, color.B }.Select( c =>
        {
            c /= 255d;// to 0-1 range

            return c < 0.03928 ? c / 12.92 : Math.Pow( ( c + 0.055 ) / 1.055, 2.4 );
        } ).ToArray();

        return 21.26 * rgb[0] + 71.52 * rgb[1] + 7.22 * rgb[2];
    }

    /// <summary>
    /// Converts the color to a 6 digit hexadecimal, or 8 digit hexadecimal string if alpha is defined.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    /// <returns>A 6 or 8 hexadecimal digit representation of color value.</returns>
    public static string ToHex( System.Drawing.Color color )
    {
        if ( color.A < 255 )
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";

        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    /// <summary>
    /// Converts the color 8 digit hexadecimal string.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    /// <returns>A 8 hexadecimal representation of color value.</returns>
    public static string ToHexRGBA( System.Drawing.Color color )
    {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
    }

    /// <summary>
    /// Converts the hslColor to a 6 digit hexadecimal, or 8 digit hexadecimal string if alpha is defined.
    /// </summary>
    /// <param name="hslColor">Color to convert.</param>
    /// <returns>A 6 or 8 hexadecimal digit representation of color value.</returns>
    public static string ToHex( HslColor hslColor )
    {
        System.Drawing.Color color = hslColor.ToColor();

        return ToHex( color );
    }

    /// <summary>
    /// Applied the transparency to the supplied color.
    /// </summary>
    /// <param name="color">Color value.</param>
    /// <param name="alpha">The alpha component. Valid values are 0 through 255.</param>
    /// <returns>New transparent color.</returns>
    public static System.Drawing.Color Transparency( System.Drawing.Color color, int alpha )
    {
        return System.Drawing.Color.FromArgb( alpha, color.R, color.G, color.B );
    }

    /// <summary>
    /// Darkens the color based on the defined percentage.
    /// </summary>
    /// <param name="color">Color to darken.</param>
    /// <param name="percentage">Percentage of how much to darken the color.</param>
    /// <returns>Darkened color.</returns>
    public static System.Drawing.Color Darken( System.Drawing.Color color, float percentage )
    {
        return ChangeColorBrightness( color, -1 * percentage / 100f );
    }

    /// <summary>
    /// Lightens the color based on the defined percentage.
    /// </summary>
    /// <param name="color">Color to lighten.</param>
    /// <param name="percentage">Percentage of how much to lighten the color.</param>
    /// <returns>Lightened color.</returns>
    public static System.Drawing.Color Lighten( System.Drawing.Color color, float percentage )
    {
        return ChangeColorBrightness( color, percentage / 100f );
    }

    /// <summary>
    /// Inverts the supplied color.
    /// </summary>
    /// <param name="color">Color to invert.</param>
    /// <returns>Inverted color.</returns>
    public static System.Drawing.Color Invert( System.Drawing.Color color )
    {
        return System.Drawing.Color.FromArgb( 255 - color.R, 255 - color.G, 255 - color.B );
    }

    /// <summary>
    /// Applies the correction factor on a color to make it brighter.
    /// </summary>
    /// <param name="color">Color to brighten.</param>
    /// <param name="correctionFactor">How much to correct the color.</param>
    /// <returns>Brightened color.</returns>
    public static System.Drawing.Color ChangeColorBrightness( System.Drawing.Color color, float correctionFactor )
    {
        float red = color.R;
        float green = color.G;
        float blue = color.B;

        if ( correctionFactor < 0 )
        {
            correctionFactor = 1 + correctionFactor;
            red *= correctionFactor;
            green *= correctionFactor;
            blue *= correctionFactor;
        }
        else
        {
            red = ( 255 - red ) * correctionFactor + red;
            green = ( 255 - green ) * correctionFactor + green;
            blue = ( 255 - blue ) * correctionFactor + blue;
        }

        return System.Drawing.Color.FromArgb( color.A, (int)red, (int)green, (int)blue );
    }

    /// <summary>
    /// Blends the two color based on the supplied percentage.
    /// </summary>
    /// <param name="color">First color.</param>
    /// <param name="color2">Second color.</param>
    /// <param name="percentage">The level of blend.</param>
    /// <returns>Combination of two colors.</returns>
    public static System.Drawing.Color Blend( System.Drawing.Color color, System.Drawing.Color color2, float percentage )
    {
        float alpha = percentage / 100f;
        byte r = (byte)( ( color.R * alpha ) + color2.R * ( 1f - alpha ) );
        byte g = (byte)( ( color.G * alpha ) + color2.G * ( 1f - alpha ) );
        byte b = (byte)( ( color.B * alpha ) + color2.B * ( 1f - alpha ) );
        return System.Drawing.Color.FromArgb( r, g, b );
    }

    /// <summary>
    /// Sass-compatible mix of two colors by weight percentage (0..100).
    /// Equivalent to Sass mix($c1, $c2, $weight).
    /// Weight favors c1. Handles alpha like Sass/Dart Sass.
    /// </summary>
    public static System.Drawing.Color Mix( System.Drawing.Color c1, System.Drawing.Color c2, double weightPercent )
    {
        // Clamp weight
        double p = Math.Max( 0.0, Math.Min( 100.0, weightPercent ) ) / 100.0;

        // Convert channels to [0..255], alpha to [0..1]
        double r1 = c1.R, g1 = c1.G, b1 = c1.B, a1 = c1.A / 255.0;
        double r2 = c2.R, g2 = c2.G, b2 = c2.B, a2 = c2.A / 255.0;

        // Sass/Dart Sass algorithm
        double w = p * 2.0 - 1.0;
        double a = a1 - a2;

        double w1;
        double wa = w * a;

        if ( Math.Abs( wa + 1.0 ) < 1e-12 )
        {
            // Avoid division by zero; fall back
            w1 = w;
        }
        else
        {
            w1 = ( w + a ) / ( 1.0 + wa );
        }

        w1 = ( w1 + 1.0 ) / 2.0;
        double w2 = 1.0 - w1;

        // Combine RGB
        int r = (int)Math.Round( r1 * w1 + r2 * w2 );
        int g = (int)Math.Round( g1 * w1 + g2 * w2 );
        int b = (int)Math.Round( b1 * w1 + b2 * w2 );

        // Combine alpha (simple weighted blend per Sass)
        double aOut = a1 * p + a2 * ( 1.0 - p );
        int aByte = (int)Math.Round( aOut * 255.0 );

        r = Clamp8( r );
        g = Clamp8( g );
        b = Clamp8( b );
        aByte = Clamp8( aByte );

        return System.Drawing.Color.FromArgb( aByte, r, g, b );

        static int Clamp8( int v ) => v < 0 ? 0 : ( v > 255 ? 255 : v );
    }

    /// <summary>
    /// Sass tint-color($color, $weight) = mix(white, $color, $weight).
    /// </summary>
    public static System.Drawing.Color TintColor( System.Drawing.Color baseColor, double weightPercent )
        => Mix( System.Drawing.Color.FromArgb( 255, 255, 255, 255 ), baseColor, weightPercent );

    /// <summary>
    /// Sass shade-color($color, $weight) = mix(black, $color, $weight).
    /// </summary>
    public static System.Drawing.Color ShadeColor( System.Drawing.Color baseColor, double weightPercent )
        => Mix( System.Drawing.Color.FromArgb( 255, 0, 0, 0 ), baseColor, weightPercent );

    /// <summary>
    /// Selects the dark or light foreground using the supplied brightness threshold.
    /// </summary>
    public static System.Drawing.Color Contrast( System.Drawing.Color background, System.Drawing.Color dark, System.Drawing.Color light, byte luminanceThreshold = 150 )
    {
        double luminance = ( 299 * background.R + 587 * background.G + 114 * background.B ) / 1000d;

        return luminance > luminanceThreshold ? dark : light;
    }

    /// <summary>
    /// Preserves the preferred foreground when it meets the minimum contrast ratio.
    /// Otherwise selects the highest contrast among the supplied colors, white, and black.
    /// </summary>
    public static System.Drawing.Color GetAccessibleContrastColor( System.Drawing.Color background, System.Drawing.Color dark, System.Drawing.Color light, byte luminanceThreshold = 150, double minimumContrastRatio = 4.5d )
    {
        System.Drawing.Color preferred = Contrast( background, dark, light, luminanceThreshold );

        if ( GetContrastRatio( background, preferred ) >= minimumContrastRatio )
            return preferred;

        System.Drawing.Color best = preferred;
        double bestRatio = 0;

        foreach ( System.Drawing.Color candidate in new[] { light, dark, System.Drawing.Color.White, System.Drawing.Color.Black } )
        {
            double ratio = GetContrastRatio( background, candidate );

            if ( ratio > bestRatio )
            {
                best = candidate;
                bestRatio = ratio;
            }
        }

        return best;
    }

    /// <summary>
    /// Calculates the contrast ratio after compositing the foreground over the background's RGB channels.
    /// </summary>
    public static double GetContrastRatio( System.Drawing.Color background, System.Drawing.Color foreground )
    {
        // Mix opaque RGB channels using foreground opacity; Sass alpha weighting does not apply here.
        System.Drawing.Color opaque = Mix(
            System.Drawing.Color.FromArgb( foreground.R, foreground.G, foreground.B ),
            System.Drawing.Color.FromArgb( background.R, background.G, background.B ),
            foreground.A / 255d * 100 );
        // LuminanceFromColor uses a 0..100 scale.
        double backgroundLuminance = LuminanceFromColor( background ) / 100d;
        double foregroundLuminance = LuminanceFromColor( opaque ) / 100d;

        return ( Math.Max( backgroundLuminance, foregroundLuminance ) + .05d )
            / ( Math.Min( backgroundLuminance, foregroundLuminance ) + .05d );
    }
}