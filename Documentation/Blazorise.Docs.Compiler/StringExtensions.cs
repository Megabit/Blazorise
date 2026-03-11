using System.Text.RegularExpressions;

namespace Blazorise.Docs.Compiler;

public static class StringExtensions
{
    public const string CrLf = "\r\n";
    public const string Lf = "\n";

    public static string ToLfLineEndings( this string value )
    {
        return NormalizeLineEndings( value, Lf );
    }

    public static string ToCrLfLineEndings( this string value )
    {
        return NormalizeLineEndings( value, CrLf );
    }

    public static string NormalizeGeneratedText( this string value )
    {
        if ( value == null )
            return null;

        return value
            .ToCrLfLineEndings()
            .TrimEnd( '\r', '\n' );
    }

    private static string NormalizeLineEndings( string value, string newLine )
    {
        if ( value == null )
            return null;

        string normalized = value.Replace( CrLf, Lf )
            .Replace( '\r', '\n' );

        return normalized.Replace( Lf, newLine );
    }
}