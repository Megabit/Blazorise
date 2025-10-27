using System.Text.RegularExpressions;

namespace Blazorise.Docs.BlogRuntime;

public static class StringExtensions
{
    public static string ToLfLineEndings( this string value )
    {
        if ( value == null )
            return null;

        return Regex.Replace( value, @"\r?\n", "\n" );
    }
}