#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Blazorise.Docs.Compiler;

public class CodeExamplesMarkup
{
    public bool Execute( bool regenerateAll = false )
    {
        var generatedFiles = new StringBuilder();
        var success = true;
        var noOfFilesUpdated = 0;
        var noOfFilesCreated = 0;

        try
        {
            var cachePath = Path.Combine( Paths.GeneratedOutputPath, ".cache", "markup.json" );
            var previousCache = GenerationCache.Read<Dictionary<string, MarkupCacheEntry>>( cachePath ) ?? new();
            var currentCache = new Dictionary<string, MarkupCacheEntry>( StringComparer.Ordinal );
            var dirPath = Paths.DirPath();

            foreach ( var path in DocsSourceFiles.Examples().OrderBy( path => path, StringComparer.Ordinal ) )
            {
                var entry = new FileInfo( path );

                // We need to skip blog examples becaouse they are generated from markdown code block and we don't want to process them again
                if ( entry.Name.EndsWith( "Code.razor" )
                     || ( entry.FullName.Contains( $"{Path.DirectorySeparatorChar}Blog{Path.DirectorySeparatorChar}", StringComparison.InvariantCultureIgnoreCase ) && entry.Name.EndsWith( ".snippet" ) ) )
                {
                    continue;
                }

                var isCSharp = entry.FullName.EndsWith( ".csharp" );

                if ( !isCSharp && !entry.Name.Contains( Paths.ExampleDiscriminator ) )
                {
                    continue;
                }

                var markupRelativePath = Path.GetRelativePath( dirPath, entry.FullName )
                    .Replace( "Examples", "Code" )
                    .Replace( ".razor", "Code.html" )
                    .Replace( ".snippet", "Code.html" )
                    .Replace( ".csharp", "Code.html" );

                var markupPath = Path.Combine( Paths.GeneratedOutputPath, markupRelativePath );
                generatedFiles.AppendLine( markupRelativePath.Replace( '\\', '/' ) );

                var source = File.ReadAllText( entry.FullName, Encoding.UTF8 );
                source = CodeSnippets.PrepareSourceForDisplay( entry.FullName, source );
                var fingerprint = GenerationCache.ContentFingerprint( ( isCSharp ? "cs" : "razor" ) + source );

                if ( !regenerateAll && previousCache.TryGetValue( markupRelativePath, out var cached )
                    && cached is not null && cached.Input == fingerprint && File.Exists( markupPath )
                    && cached.Output == GenerationCache.FileFingerprint( markupPath ) )
                {
                    currentCache[markupRelativePath] = cached;
                    continue;
                }

                var markupDir = Path.GetDirectoryName( markupPath );
                if ( !Directory.Exists( markupDir ) )
                {
                    Directory.CreateDirectory( markupDir );
                }

                var currentCode = string.Empty;

                if ( File.Exists( markupPath ) )
                {
                    currentCode = File.ReadAllText( markupPath ).NormalizeGeneratedText();
                }

                var builtCode = new MarkupBuilder().Build( source, isCSharp ? "cs" : null );

                if ( currentCode != builtCode )
                {
                    File.WriteAllText( markupPath, builtCode );

                    if ( currentCode == string.Empty )
                    {
                        noOfFilesCreated++;
                    }
                    else
                    {
                        noOfFilesUpdated++;
                    }
                }

                currentCache[markupRelativePath] = new MarkupCacheEntry( fingerprint, GenerationCache.FileFingerprint( markupPath ) );
            }

            GenerationCache.WriteTextIfChanged( Paths.ExampleCodeFilesPath(), generatedFiles.ToString() );
            GenerationCache.Write( cachePath, currentCache );
        }
        catch ( Exception e )
        {
            Console.WriteLine( $"Error generating examples markup : {e.Message}" );
            success = false;
        }

        Console.WriteLine( $"Docs.Compiler updated {noOfFilesUpdated} generated files" );
        Console.WriteLine( $"Docs.Compiler generated {noOfFilesCreated} new files" );
        return success;
    }

    public sealed record MarkupCacheEntry( string Input, string Output );

    public static string AttributePostprocessing( string html )
    {
        return Regex.Replace(
            html,
            @"<span class=""htmlAttributeValue"">&quot;(?'value'.*?)&quot;</span>",
            new MatchEvaluator( m =>
            {
                var value = m.Groups["value"].Value;
                return
                    $@"<span class=""quot"">&quot;</span>{AttributeValuePostprocessing( value )}<span class=""quot"">&quot;</span>";
            } ) );
    }

    private static string AttributeValuePostprocessing( string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return value;
        if ( value == "true" || value == "false" )
            return $"<span class=\"keyword\">{value}</span>";
        if ( Regex.IsMatch( value, "^[A-Z][A-Za-z0-9]+[.][A-Za-z][A-Za-z0-9]+$" ) )
        {
            var tokens = value.Split( '.' );
            return $"<span class=\"enum\">{tokens[0]}</span><span class=\"enumValue\">.{tokens[1]}</span>";
        }

        if ( Regex.IsMatch( value, "^@[A-Za-z0-9]+$" ) )
        {
            return $"<span class=\"sharpVariable\">{value}</span>";
        }

        return $"<span class=\"htmlAttributeValue\">{value}</span>";
    }
}