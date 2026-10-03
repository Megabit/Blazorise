#region Using directives
using System;
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
            var startedUtc = DateTime.UtcNow;
            var timestampPath = Paths.ExampleCodeFilesPath();

            var lastCheckedUtc = File.Exists( timestampPath )
                ? File.GetLastWriteTimeUtc( timestampPath )
                : DateTime.MinValue;

            var dirPath = Paths.DirPath();
            var directoryInfo = new DirectoryInfo( dirPath );

            var razorFiles = directoryInfo.GetFiles( "*.razor", SearchOption.AllDirectories );
            var snippetFiles = directoryInfo.GetFiles( "*.snippet", SearchOption.AllDirectories );
            var csharpFiles = directoryInfo.GetFiles( "*.csharp", SearchOption.AllDirectories );

            foreach ( var entry in razorFiles.Concat( snippetFiles ).Concat( csharpFiles ) )
            {
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

                // Versioned examples must also refresh when only Blazorise.Version.props changes.
                if ( !regenerateAll && entry.LastWriteTimeUtc < lastCheckedUtc && File.Exists( markupPath )
                    && !AssetVersioning.HasVersionToken( source ) )
                {
                    continue;
                }

                var markupDir = Path.GetDirectoryName( markupPath );
                if ( !Directory.Exists( markupDir ) )
                {
                    Directory.CreateDirectory( markupDir );
                }

                var currentCode = string.Empty;
                source = CodeSnippets.PrepareSourceForDisplay( entry.FullName, source );

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
            }

            File.WriteAllText( timestampPath, generatedFiles.ToString() );

            // Preserve edits made while the generator was running for the next build.
            File.SetLastWriteTimeUtc( timestampPath, startedUtc );
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