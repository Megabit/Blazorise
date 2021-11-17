namespace Blazorise.Extensions
{
    /// <summary>
    /// Various extension methods for string object.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Check if supplied string consists of empty spaces and returns null if it is.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <returns>Null of string is empty.</returns>
        public static string EmptyToNull( this string value )
        {
            return string.IsNullOrEmpty( value ) ? null : value;
        }

        /// <summary>
        /// Converts all string characters to lower-case except for the first character.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>Camelcased string.</returns>
        public static string ToCamelcase( this string value )
        {
            if ( !string.IsNullOrEmpty( value ) && value.Length > 1 )
            {
                return char.ToLowerInvariant( value[0] ) + value[1..];
            }

            return value;
        }
    }
}
