#region Using directives
using System;
using System.Diagnostics;
using System.IO;
using Blazorise.Docs.Compiler.ApiDocsGenerator;
#endregion

namespace Blazorise.Docs.Compiler;

class Program
{
    static int Main( string[] args )
    {
        var stopWatch = Stopwatch.StartNew();

        var outputPath = GetArgValue( args, "--output-path" );

        if ( !string.IsNullOrWhiteSpace( outputPath ) )
        {
            Paths.GeneratedOutputPath = Path.GetFullPath( outputPath );
        }

        var apiDocsOutputPath = GetArgValue( args, "--api-docs-path" );
        var regenerateExamples = bool.TryParse( GetArgValue( args, "--regenerate-examples" ), out var regenerateAll ) && regenerateAll;
        var searchDataOnly = bool.TryParse( GetArgValue( args, "--search-data-only" ), out var generateSearchDataOnly ) && generateSearchDataOnly;

        var codeSnippetResult = true;
        var codeExamplesResult = true;

        if ( !searchDataOnly )
        {
            Directory.CreateDirectory( Paths.DocsStringSnippetsDirPath() );
            codeSnippetResult = new CodeSnippets().Execute();
            codeExamplesResult = new CodeExamplesMarkup().Execute( regenerateExamples );
        }

        var apiDocsGenerator = new ComponentsApiDocsGenerator( apiDocsOutputPath ).Execute( searchDataOnly );
        var docsIndexGenerator = new DocsIndexGenerator().Execute();

        Console.WriteLine( $"Blazorise.Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} milliseconds." );
        return codeSnippetResult && codeExamplesResult && apiDocsGenerator && docsIndexGenerator ? 0 : 1;
    }

    private static string GetArgValue( string[] args, string name )
    {
        if ( args is null || args.Length == 0 )
            return null;

        for ( int i = 0; i < args.Length; i++ )
        {
            string arg = args[i];

            if ( string.Equals( arg, name, StringComparison.OrdinalIgnoreCase ) )
            {
                if ( i + 1 < args.Length )
                    return args[i + 1];

                return null;
            }

            string prefix = name + "=";
            if ( arg.StartsWith( prefix, StringComparison.OrdinalIgnoreCase ) )
                return arg.Substring( prefix.Length );
        }

        return null;
    }
}