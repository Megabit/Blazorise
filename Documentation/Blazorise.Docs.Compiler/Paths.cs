#region Using directives
using System.IO;
#endregion

namespace Blazorise.Docs.Compiler;

public static class Paths
{
    public const string ExampleDiscriminator = "Example"; // example components must contain this string

    public static string RootDirPath
    {
        get
        {
            var directory = new DirectoryInfo( Directory.GetCurrentDirectory() );

            while ( directory is not null )
            {
                var candidate = directory.Name == "Documentation" ? directory.FullName : Path.Combine( directory.FullName, "Documentation" );

                if ( Directory.Exists( Path.Combine( candidate, "Blazorise.Docs" ) ) )
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException( "Cannot locate the Blazorise documentation sources." );
        }
    }


    public static string BlazoriseLibRoot => Path.Combine( RootDirPath, "..", "Source", "Blazorise" );
    public static string BlazoriseExtensionsRoot => Path.Combine( RootDirPath, "..", "Source", "Extensions" );
    public static string GeneratedOutputPath { get; set; } = Path.Combine( DirPath(), "obj", "DocsGenerated" );

    public static string ApiDocsPath => Path.Join( GeneratedOutputPath, "ApiDocs" );
    public static string DirPath() => Path.Combine( RootDirPath, "Blazorise.Docs" );

    public static string DocsStringSnippetsDirPath() => Path.Join( GeneratedOutputPath, "Models" );

    public static string DocStringsFilePath() => Path.Join( DocsStringSnippetsDirPath(), "Strings.generated.cs" );

    public static string SnippetsFilePath() => Path.Join( DocsStringSnippetsDirPath(), "Snippets.generated.cs" );

    public static string DocsIndexFilePath() => Path.Join( GeneratedOutputPath, "Resources", "docs-index.json" );
    public static string DocsApiIndexFilePath() => Path.Join( GeneratedOutputPath, "Resources", "docs-api-index.json" );

    public static string ExampleCodeFilesPath() => Path.Join( GeneratedOutputPath, "ExampleCodeFiles.txt" );
}