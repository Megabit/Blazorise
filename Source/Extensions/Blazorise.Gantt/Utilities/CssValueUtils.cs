#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise.Gantt.Utilities;

/// <summary>
/// Provides helper methods for parsing CSS style values.
/// </summary>
public static class CssValueUtils
{
    /// <summary>
    /// Attempts to read a numeric CSS value and unit from a fluent sizing style.
    /// </summary>
    /// <param name="sizing">Sizing instance.</param>
    /// <param name="styleProvider">Style provider.</param>
    /// <param name="propertyName">CSS property name (for example: <c>width</c>).</param>
    /// <param name="unit">Parsed CSS unit.</param>
    /// <param name="value">Parsed numeric value.</param>
    /// <returns>True if value was parsed; otherwise false.</returns>
    public static bool TryGetNumericStyleValue( IFluentSizing sizing, IStyleProvider styleProvider, string propertyName, out string unit, out double value )
    {
        unit = null;
        value = 0d;

        if ( sizing is null
             || styleProvider is null
             || string.IsNullOrWhiteSpace( propertyName ) )
            return false;

        var style = sizing.Style( styleProvider );

        if ( string.IsNullOrWhiteSpace( style ) )
            return false;

        var propertyPrefix = $"{propertyName.Trim()}:";

        foreach ( var styleRule in style.Split( ';' ) )
        {
            var rule = styleRule?.Trim();

            if ( string.IsNullOrWhiteSpace( rule )
                 || !rule.StartsWith( propertyPrefix, StringComparison.OrdinalIgnoreCase ) )
                continue;

            var valueText = rule[propertyPrefix.Length..].Trim();

            if ( TryParseNumericValue( valueText, out value, out unit ) )
                return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to parse a numeric CSS value such as <c>120px</c>.
    /// </summary>
    /// <param name="cssValue">CSS value text.</param>
    /// <param name="value">Parsed numeric value.</param>
    /// <param name="unit">Parsed CSS unit.</param>
    /// <returns>True if value was parsed; otherwise false.</returns>
    public static bool TryParseNumericValue( string cssValue, out double value, out string unit )
    {
        value = 0d;
        unit = null;

        if ( string.IsNullOrWhiteSpace( cssValue ) )
            return false;

        var text = cssValue.Trim();
        var index = 0;

        if ( text[index] == '+' || text[index] == '-' )
            index++;

        var hasDigits = false;

        while ( index < text.Length )
        {
            var ch = text[index];

            if ( char.IsDigit( ch ) )
            {
                hasDigits = true;
                index++;
                continue;
            }

            if ( ch == '.' )
            {
                index++;
                continue;
            }

            break;
        }

        if ( !hasDigits || index >= text.Length )
            return false;

        var valueText = text[..index];
        var unitText = text[index..].Trim();

        if ( string.IsNullOrWhiteSpace( unitText ) )
            return false;

        if ( !double.TryParse( valueText, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedValue ) )
            return false;

        value = parsedValue;
        unit = unitText;

        return true;
    }
}