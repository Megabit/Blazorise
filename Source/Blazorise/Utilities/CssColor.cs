#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise;

/// <summary>
/// Provides helpers for creating and identifying CSS color values.
/// </summary>
public static class CssColor
{
    #region Methods

    /// <summary>
    /// Creates an RGB CSS color value.
    /// </summary>
    /// <param name="red">Red component from 0 to 255.</param>
    /// <param name="green">Green component from 0 to 255.</param>
    /// <param name="blue">Blue component from 0 to 255.</param>
    /// <returns>A CSS <c>rgb()</c> color value.</returns>
    public static string Rgb( byte red, byte green, byte blue )
    {
        return $"rgb({red},{green},{blue})";
    }

    /// <summary>
    /// Creates an RGBA CSS color value.
    /// </summary>
    /// <param name="red">Red component from 0 to 255.</param>
    /// <param name="green">Green component from 0 to 255.</param>
    /// <param name="blue">Blue component from 0 to 255.</param>
    /// <param name="alpha">Alpha component from 0 to 1.</param>
    /// <returns>A CSS <c>rgba()</c> color value.</returns>
    public static string Rgba( byte red, byte green, byte blue, double alpha )
    {
        return $"rgba({red},{green},{blue},{Format( Math.Clamp( alpha, 0, 1 ) )})";
    }

    /// <summary>
    /// Creates an HSL CSS color value.
    /// </summary>
    /// <param name="hue">Hue component in degrees.</param>
    /// <param name="saturation">Saturation component from 0 to 100.</param>
    /// <param name="lightness">Lightness component from 0 to 100.</param>
    /// <returns>A CSS <c>hsl()</c> color value.</returns>
    public static string Hsl( double hue, double saturation, double lightness )
    {
        return $"hsl({Format( hue )} {Format( Math.Clamp( saturation, 0, 100 ) )}% {Format( Math.Clamp( lightness, 0, 100 ) )}%)";
    }

    /// <summary>
    /// Creates an HSLA CSS color value.
    /// </summary>
    /// <param name="hue">Hue component in degrees.</param>
    /// <param name="saturation">Saturation component from 0 to 100.</param>
    /// <param name="lightness">Lightness component from 0 to 100.</param>
    /// <param name="alpha">Alpha component from 0 to 1.</param>
    /// <returns>A CSS <c>hsl()</c> color value with an alpha component.</returns>
    public static string Hsla( double hue, double saturation, double lightness, double alpha )
    {
        return $"hsl({Format( hue )} {Format( Math.Clamp( saturation, 0, 100 ) )}% {Format( Math.Clamp( lightness, 0, 100 ) )}% / {Format( Math.Clamp( alpha, 0, 1 ) )})";
    }

    /// <summary>
    /// Creates a CSS variable color value.
    /// </summary>
    /// <param name="name">CSS custom property name, with or without the leading <c>--</c>.</param>
    /// <param name="fallback">Optional fallback color.</param>
    /// <returns>A CSS <c>var()</c> color value.</returns>
    public static string Variable( string name, string fallback = null )
    {
        if ( string.IsNullOrWhiteSpace( name ) )
            throw new ArgumentException( "A CSS custom property name is required.", nameof( name ) );

        string normalizedName = name.Trim();

        if ( !normalizedName.StartsWith( "--", StringComparison.Ordinal ) )
            normalizedName = $"--{normalizedName}";

        return string.IsNullOrWhiteSpace( fallback )
            ? $"var({normalizedName})"
            : $"var({normalizedName},{fallback.Trim()})";
    }

    /// <summary>
    /// Determines whether a value represents an explicit CSS color rather than a Blazorise contextual color.
    /// </summary>
    /// <param name="value">Color value to inspect.</param>
    /// <returns><see langword="true"/> when the value is an explicit CSS color.</returns>
    public static bool IsValue( string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return false;

        string trimmedValue = value.Trim();

        if ( trimmedValue.Contains( ';' ) || trimmedValue.Contains( '{' ) || trimmedValue.Contains( '}' ) )
            return false;

        if ( trimmedValue.StartsWith( "#", StringComparison.Ordinal ) )
            return true;

        if ( string.Equals( trimmedValue, "transparent", StringComparison.OrdinalIgnoreCase )
             || string.Equals( trimmedValue, "currentColor", StringComparison.OrdinalIgnoreCase )
             || string.Equals( trimmedValue, "inherit", StringComparison.OrdinalIgnoreCase )
             || string.Equals( trimmedValue, "initial", StringComparison.OrdinalIgnoreCase )
             || string.Equals( trimmedValue, "revert", StringComparison.OrdinalIgnoreCase )
             || string.Equals( trimmedValue, "revert-layer", StringComparison.OrdinalIgnoreCase )
             || string.Equals( trimmedValue, "unset", StringComparison.OrdinalIgnoreCase ) )
            return true;

        return IsFunction( trimmedValue, "rgb" )
               || IsFunction( trimmedValue, "rgba" )
               || IsFunction( trimmedValue, "hsl" )
               || IsFunction( trimmedValue, "hsla" )
               || IsFunction( trimmedValue, "hwb" )
               || IsFunction( trimmedValue, "lab" )
               || IsFunction( trimmedValue, "lch" )
               || IsFunction( trimmedValue, "oklab" )
               || IsFunction( trimmedValue, "oklch" )
               || IsFunction( trimmedValue, "color" )
               || IsFunction( trimmedValue, "color-mix" )
               || IsFunction( trimmedValue, "light-dark" )
               || IsFunction( trimmedValue, "var" );
    }

    /// <summary>
    /// Resolves an explicit CSS color directly, or a contextual color through Blazorise and Bootstrap theme variables.
    /// </summary>
    /// <param name="value">Explicit or contextual color value.</param>
    /// <param name="fallback">Optional fallback used when the contextual theme variables are not defined.</param>
    /// <returns>A CSS-compatible color value.</returns>
    public static string Resolve( string value, string fallback = null )
    {
        return Resolve( value, IsValue( value ), fallback );
    }

    /// <summary>
    /// Resolves a <see cref="Color"/> through its cached CSS-value classification.
    /// </summary>
    /// <param name="color">Explicit or contextual color.</param>
    /// <param name="fallback">Optional fallback used when contextual theme variables are not defined.</param>
    /// <returns>A CSS-compatible color value.</returns>
    public static string ResolveColor( Color color, string fallback = null )
    {
        return color is null
            ? fallback
            : Resolve( color.Name, color.IsCssValue, fallback );
    }

    private static string Resolve( string value, bool isCssValue, string fallback )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return fallback;

        string trimmedValue = value.Trim();

        if ( isCssValue )
            return trimmedValue;

        return string.IsNullOrWhiteSpace( fallback )
            ? $"var(--b-theme-{trimmedValue}, var(--bs-{trimmedValue}))"
            : $"var(--b-theme-{trimmedValue}, var(--bs-{trimmedValue}, {fallback}))";
    }

    private static bool IsFunction( string value, string functionName )
    {
        return value.Length > functionName.Length + 1
               && value.StartsWith( functionName, StringComparison.OrdinalIgnoreCase )
               && value[functionName.Length] == '('
               && value.EndsWith( ')' );
    }

    private static string Format( double value )
    {
        return value.ToString( "0.###", CultureInfo.InvariantCulture );
    }

    #endregion
}