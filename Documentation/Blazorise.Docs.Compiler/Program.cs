#region Using directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        var docsInputs = DocsSourceFiles.Enumerate( Paths.DirPath() )
            .Where( file => Path.GetExtension( file ) is ".razor" or ".snippet" or ".csharp" or ".cs" )
            .ToArray();

        var examplesResult = true;

        if ( !searchDataOnly )
        {
            Directory.CreateDirectory( Paths.DocsStringSnippetsDirPath() );
            examplesResult = GenerationCache.Run( "examples", docsInputs, GetExampleOutputs,
                () => new CodeSnippets().Execute() && new CodeExamplesMarkup().Execute( regenerateExamples ), force: regenerateExamples );
        }

        var apiOutputRoot = Path.GetFullPath( string.IsNullOrWhiteSpace( apiDocsOutputPath ) ? Paths.ApiDocsPath : apiDocsOutputPath );
        var apiInputs = DocsSourceFiles.ApiDirectories().SelectMany( directory => DocsSourceFiles.Enumerate( directory, "*.cs" ) );
        var apiResult = GenerationCache.Run( "api", apiInputs, () => GetApiOutputs( apiOutputRoot, searchDataOnly ),
            () => new ComponentsApiDocsGenerator( apiOutputRoot ).Execute( searchDataOnly ), context: apiOutputRoot + searchDataOnly );

        var searchResult = GenerationCache.Run( "search", docsInputs,
            () => new[] { Paths.DocsIndexFilePath() }, () => new DocsIndexGenerator().Execute() );

        Console.WriteLine( $"Blazorise.Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} milliseconds." );
        return examplesResult && apiResult && searchResult ? 0 : 1;
    }

    private static IEnumerable<string> GetExampleOutputs()
    {
        var manifestPath = Paths.ExampleCodeFilesPath();
        var outputs = new[] { manifestPath, Paths.SnippetsFilePath() };

        return File.Exists( manifestPath )
            ? outputs.Concat( File.ReadAllLines( manifestPath ).Select( file => Path.Combine( Paths.GeneratedOutputPath, file ) ) )
            : outputs;
    }

    private static IEnumerable<string> GetApiOutputs( string root, bool searchDataOnly )
        => searchDataOnly || !Directory.Exists( root )
            ? new[] { Paths.DocsApiIndexFilePath() }
            : new[] { Paths.DocsApiIndexFilePath() }.Concat( Directory.EnumerateFiles( root, "*.ApiDocs.cs" ) );

    private static string GetArgValue( string[] args, string name )
    {
        if ( args is null || args.Length == 0 )
        {
            return null;
        }

        for ( var i = 0; i < args.Length; i++ )
        {
            var arg = args[i];

            if ( string.Equals( arg, name, StringComparison.OrdinalIgnoreCase ) )
            {
                if ( i + 1 < args.Length )
                {
                    return args[i + 1];
                }

                return null;
            }

            var prefix = name + "=";

            if ( arg.StartsWith( prefix, StringComparison.OrdinalIgnoreCase ) )
            {
                return arg.Substring( prefix.Length );
            }
        }

        return null;
    }
}