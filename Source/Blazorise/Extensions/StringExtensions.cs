namespace Blazorise.Extensions
{
    public static class StringExtensions
    {
        public static string EmptyToNull( this string value )
        {
            return string.IsNullOrEmpty( value ) ? null : value;
        }

        public static string ToCamelcase( this string value )
        {
            if ( !string.IsNullOrEmpty( value ) && value.Length > 1 )
            {
                return char.ToLowerInvariant( value[0] ) + value.Substring( 1 );
            }

            return value;
        }
    }
}
