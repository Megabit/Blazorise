#region Using directives
using System;
using System.IO;
using System.Linq;
using System.Text;
using Blazorise.Docs.Compiler.ExampleSources;
#endregion

namespace Blazorise.Docs.Compiler;

public class CodeSnippets
{
    public bool Execute()
    {
        var success = true;
        try
        {
            var currentCode = string.Empty;

            if ( File.Exists( Paths.SnippetsFilePath() ) )
            {
                currentCode = File.ReadAllText( Paths.SnippetsFilePath() ).NormalizeGeneratedText();
            }

            var cb = new CodeBuilder();
            cb.AddHeader();
            cb.AddLine( $"namespace Blazorise.Docs.Models" );
            cb.AddLine( "{" );
            cb.IndentLevel++;
            cb.AddLine( $"public static partial class Snippets" );
            cb.AddLine( "{" );
            cb.IndentLevel++;

            foreach ( var entry in DocsSourceFiles.Examples().OrderBy( e => e.Replace( "\\", "/" ), StringComparer.Ordinal ) )
            {
                var filename = Path.GetFileName( entry );
                var componentName = Path.GetFileNameWithoutExtension( filename );
                var isCSharp = entry.EndsWith( ".csharp" );

                if ( !isCSharp && !componentName.Contains( Paths.ExampleDiscriminator ) )
                {
                    continue;
                }

                cb.AddLine( $"public const string {componentName} = @\"{EscapeComponentSource( entry )}\";" );
                cb.AddLine();
            }

            cb.IndentLevel--;
            cb.AddLine( "}" );
            cb.IndentLevel--;
            cb.AddLine( "}" );

            var builtCode = cb.ToString().NormalizeGeneratedText();

            if ( currentCode != builtCode )
            {
                File.WriteAllText( Paths.SnippetsFilePath(), builtCode );
            }
        }
        catch ( Exception e )
        {
            Console.WriteLine( $"Error generating {Paths.SnippetsFilePath} : {e.Message}" );
            success = false;
        }

        return success;
    }

    private static string EscapeComponentSource( string path )
    {
        var source = File.ReadAllText( path, Encoding.UTF8 );
        source = PrepareSourceForCopy( path, source );
        source = ExampleSourceComposerHelpers.RemoveDocsDirectives( source );
        return source.Replace( "\"", "\"\"" ).Trim().ToCrLfLineEndings();
    }

    internal static string PrepareSourceForDisplay( string path, string source )
        => ExampleSourceComposerPipeline.PrepareForDisplay( path, source );

    internal static string PrepareSourceForCopy( string path, string source )
        => ExampleSourceComposerPipeline.PrepareForCopy( path, source );
}