#region Using directives
using System;
#endregion

namespace Blazorise.Gantt.Utilities;

/// <summary>
/// Provides helper methods for string comparisons.
/// </summary>
public static class StringUtils
{
    /// <summary>
    /// Determines whether two string values are equal, using ordinal ignore-case comparison.
    /// </summary>
    /// <param name="value">First value.</param>
    /// <param name="reference">Second value.</param>
    /// <returns>True when both values are non-empty and equal; otherwise false.</returns>
    public static bool IsMatch( string value, string reference )
    {
        return !string.IsNullOrWhiteSpace( value )
               && !string.IsNullOrWhiteSpace( reference )
               && string.Equals( value, reference, StringComparison.OrdinalIgnoreCase );
    }
}