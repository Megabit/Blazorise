using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Blazorise.Docs.Compiler.ExampleSources;

internal static class ExampleSourceComposerHelpers
{
    internal const string CopyPasteReadyMarker = "@* copy-paste-ready *@";

    private const string RazorDocsDirectivePattern = "@(namespace|layout|page) .+?\\r?\\n";

    public static string RemoveDocsDirectives( string source )
        => Regex.Replace( source, RazorDocsDirectivePattern, string.Empty );

    public static string ExtractSupportTypes( string path )
    {
        string source = File.ReadAllText( path, Encoding.UTF8 );
        return ExtractSupportTypesFromSource( source );
    }

    public static string ExtractSupportTypesFromSource( string source )
    {
        source = Regex.Replace( source, "(?m)^using .+;\\r?\\n", string.Empty );
        source = Regex.Replace( source, "(?m)^namespace .+;\\r?\\n", string.Empty );
        return source.Trim();
    }

    public static string AddRequiredUsings( string path, string source, IReadOnlyList<string> requiredUsings )
    {
        string[] missingUsings = requiredUsings
            .Where( requiredUsing => !Regex.IsMatch( source, $"(?m)^{Regex.Escape( requiredUsing )}\\r?$" ) )
            .ToArray();

        if ( missingUsings.Length == 0 )
            return source;

        Match namespaceDirective = Regex.Match( source, "@namespace[^\\r\\n]+\\r?\\n" );

        if ( !namespaceDirective.Success )
            throw new InvalidOperationException( $"Example '{path}' must declare a namespace before its copy-ready imports can be composed." );

        string imports = string.Join( Environment.NewLine, missingUsings ) + Environment.NewLine;
        return source.Insert( namespaceDirective.Index + namespaceDirective.Length, imports );
    }

    public static string AppendToCodeBlock( string path, string source, string content )
    {
        int codeBlockEnd = source.LastIndexOf( '}' );

        if ( codeBlockEnd < 0 || !source.Contains( "@code", StringComparison.Ordinal ) )
            throw new InvalidOperationException( $"Example '{path}' must end with an @code block so its support code can be inlined." );

        string indentedContent = IndentLines( content, 4 );
        return source.Insert( codeBlockEnd, $"{Environment.NewLine}{Environment.NewLine}{indentedContent}{Environment.NewLine}" );
    }

    public static string IndentLines( string source, int spaces )
    {
        string indentation = new( ' ', spaces );
        return string.Join(
            Environment.NewLine,
            source.Split( ["\r\n", "\n"], StringSplitOptions.None ).Select( line => $"{indentation}{line}" ) );
    }

    public static void ValidateComposedSource( string path, string source, IReadOnlyList<string> forbiddenDependencies )
    {
        string forbiddenDependency = forbiddenDependencies.FirstOrDefault( dependency => source.Contains( dependency, StringComparison.Ordinal ) );

        if ( forbiddenDependency is not null )
            throw new InvalidOperationException( $"Composed example '{path}' references docs-only dependency '{forbiddenDependency}'." );
    }
}