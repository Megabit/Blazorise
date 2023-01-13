using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                currentCode = File.ReadAllText( Paths.SnippetsFilePath() );
            }

            var cb = new CodeBuilder();
            cb.AddHeader();
            cb.AddLine( $"namespace Blazorise.Docs.Models" );
            cb.AddLine( "{" );
            cb.IndentLevel++;
            cb.AddLine( $"public static partial class Snippets" );
            cb.AddLine( "{" );
            cb.IndentLevel++;

            var dirPath = Paths.DirPath();
            var razorFiles = Directory.EnumerateFiles( dirPath, "*.razor", SearchOption.AllDirectories );
            var snippetFiles = Directory.EnumerateFiles( dirPath, "*.snippet", SearchOption.AllDirectories );
            var csharpFiles = Directory.EnumerateFiles( dirPath, "*.csharp", SearchOption.AllDirectories );

            foreach ( var entry in razorFiles.Concat( snippetFiles ).Concat( csharpFiles ).OrderBy( e => e.Replace( "\\", "/" ), StringComparer.Ordinal ) )
            {
                var filename = Path.GetFileName( entry );
                var componentName = Path.GetFileNameWithoutExtension( filename );
                if ( !componentName.Contains( Paths.ExampleDiscriminator ) )
                    continue;
                cb.AddLine( $"public const string {componentName} = @\"{EscapeComponentSource( entry )}\";\n" );
            }

            cb.IndentLevel--;
            cb.AddLine( "}" );
            cb.IndentLevel--;
            cb.AddLine( "}" );

            if ( currentCode != cb.ToString() )
            {
                File.WriteAllText( Paths.SnippetsFilePath(), cb.ToString() );
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
        source = Regex.Replace( source, "@(namespace|layout|page) .+?\n", string.Empty );
        return source.Replace( "\"", "\"\"" ).Trim();
    }
}