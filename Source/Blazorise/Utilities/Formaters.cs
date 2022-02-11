#region Using directives
using System;
#endregion

namespace Blazorise.Utilities
{
    /// <summary>
    /// Helper methods for easier formating of values into strings.
    /// </summary>
    public static class Formaters
    {
        /// <summary>
        /// Formats the supplied value to it's valid string representation.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <param name="dateFormat">Date format.</param>
        /// <returns>Returns value formatted as string.</returns>
        public static string FormatDateValueAsString<TValue>( TValue value, string dateFormat )
        {
            return value switch
            {
                null => null,
                DateTime datetime => datetime.ToString( dateFormat ),
                DateTimeOffset datetimeOffset => datetimeOffset.ToString( dateFormat ),
                DateOnly dateOnly => dateOnly.ToString( dateFormat ),
                _ => throw new InvalidOperationException( $"Unsupported type {value.GetType()}" ),
            };
        }
    }
}
