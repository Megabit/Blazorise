#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise.Gantt.Utilities;

/// <summary>
/// Provides helper methods for generic object values.
/// </summary>
public static class ValueUtils
{
    /// <summary>
    /// Attempts to convert value to <see cref="double"/>.
    /// </summary>
    /// <param name="value">Source value.</param>
    /// <param name="parsedValue">Converted result.</param>
    /// <returns>True when conversion succeeds; otherwise false.</returns>
    public static bool TryConvertToDouble( object value, out double parsedValue )
    {
        switch ( value )
        {
            case byte byteValue:
                parsedValue = byteValue;
                return true;
            case short shortValue:
                parsedValue = shortValue;
                return true;
            case int intValue:
                parsedValue = intValue;
                return true;
            case long longValue:
                parsedValue = longValue;
                return true;
            case float floatValue:
                parsedValue = floatValue;
                return true;
            case double doubleValue:
                parsedValue = doubleValue;
                return true;
            case decimal decimalValue:
                parsedValue = (double)decimalValue;
                return true;
            case string stringValue when double.TryParse( stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedValue ):
                return true;
            case IConvertible convertible:
                try
                {
                    parsedValue = convertible.ToDouble( CultureInfo.InvariantCulture );
                    return true;
                }
                catch
                {
                    break;
                }
        }

        parsedValue = 0d;
        return false;
    }

    /// <summary>
    /// Normalizes identifier-like value to invariant string.
    /// </summary>
    /// <param name="value">Identifier value.</param>
    /// <returns>Invariant string representation or null when value is null.</returns>
    public static string NormalizeIdentifier( object value )
    {
        if ( value is null )
            return null;

        return Convert.ToString( value, CultureInfo.InvariantCulture );
    }

    /// <summary>
    /// Compares two values using null-safe and comparable-first strategy.
    /// </summary>
    /// <param name="x">First value.</param>
    /// <param name="y">Second value.</param>
    /// <returns>Comparison result.</returns>
    public static int CompareValues( object x, object y )
    {
        if ( ReferenceEquals( x, y ) )
            return 0;

        if ( x is null )
            return -1;

        if ( y is null )
            return 1;

        if ( x is IComparable comparableX && x.GetType() == y.GetType() )
            return comparableX.CompareTo( y );

        return StringComparer.OrdinalIgnoreCase.Compare(
            Convert.ToString( x, CultureInfo.InvariantCulture ),
            Convert.ToString( y, CultureInfo.InvariantCulture ) );
    }
}