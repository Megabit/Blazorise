using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ColorCode;

namespace Blazorise.Docs.Compiler;

public class CodeExamplesMarkup
{
    public bool Execute()
    {
        var newFiles = new StringBuilder();
        var success = true;
        var noOfFilesUpdated = 0;
        var noOfFilesCreated = 0;

        try
        {
            var formatter = new HtmlClassFormatter();
            var lastCheckedTime = new DateTime();
            if ( File.Exists( Paths.NewFilesToBuildPath() ) )
            {
                var lastNewFilesToBuild = new FileInfo( Paths.NewFilesToBuildPath() );
                lastCheckedTime = lastNewFilesToBuild.LastWriteTime;
            }

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

                if ( !entry.Name.Contains( Paths.ExampleDiscriminator ) )
                    continue;

                var markupPath = entry.FullName
                    .Replace( "Examples", "Code" )
                    .Replace( ".razor", "Code.html" )
                    .Replace( ".snippet", "Code.html" )
                    .Replace( ".csharp", "Code.html" );

                if ( entry.LastWriteTime < lastCheckedTime && File.Exists( markupPath ) )
                {
                    continue;
                }

                var markupDir = Path.GetDirectoryName( markupPath );
                if ( !Directory.Exists( markupDir ) )
                {
                    Directory.CreateDirectory( markupDir );
                }

                //var cb = new CodeBuilder();
                var currentCode = string.Empty;
                var builtCode = string.Empty;
                var isCSharp = entry.FullName.EndsWith( ".csharp" );
                var source = File.ReadAllText( entry.FullName, Encoding.UTF8 );

                if ( File.Exists( markupPath ) )
                {
                    currentCode = File.ReadAllText( markupPath );
                }

                builtCode = new MarkupBuilder( formatter ).Build( source, isCSharp ? "cs" : null );

                if ( currentCode != builtCode )
                {
                    File.WriteAllText( markupPath, builtCode );

                    if ( currentCode == string.Empty )
                    {
                        newFiles.AppendLine( markupPath );
                        noOfFilesCreated++;
                    }
                    else
                    {
                        noOfFilesUpdated++;
                    }
                }
            }

            File.WriteAllText( Paths.NewFilesToBuildPath(), newFiles.ToString() );
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