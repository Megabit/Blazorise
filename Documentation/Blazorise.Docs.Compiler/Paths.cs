using System.IO;
using System.Linq;

namespace Blazorise.Docs.Compiler;

public static class Paths
{
    private const string NewFilesToBuild = "NewFilesToBuild.txt";

    public const string ExampleDiscriminator = "Example"; // example components must contain this string

    public static string RootDirPath
    {
        get
        {
            var workingPath = Path.GetFullPath( "." );

            do
            {
                workingPath = Path.GetDirectoryName( workingPath );
            }
            while ( Path.GetFileName( workingPath ) != "Documentation" && !string.IsNullOrWhiteSpace( workingPath ) );

            return workingPath;
        }
    }


    public static string BlazoriseLibRoot => Path.Combine( RootDirPath, "..", "Source", "Blazorise" );
    public static string BlazoriseExtensionsRoot => Path.Combine( RootDirPath, "..", "Source", "Extensions" );
    public static string ApiDocsPath => Path.Join( DirPath(), "ApiDocs" );
    public static string DirPath() => Directory.EnumerateDirectories( RootDirPath, $"Blazorise.Docs" ).FirstOrDefault();

    public static string DocsStringSnippetsDirPath() => Path.Join( DirPath(), "Models" );

    public static string DocStringsFilePath() => Path.Join( DocsStringSnippetsDirPath(), "Strings.generated.cs" );

    public static string SnippetsFilePath() => Path.Join( DocsStringSnippetsDirPath(), "Snippets.generated.cs" );

    public static string DocsIndexFilePath() => Path.Join( DirPath(), "Resources", "docs-index.json" );
    public static string DocsApiIndexFilePath() => Path.Join( DirPath(), "Resources", "docs-api-index.json" );

    public static string NewFilesToBuildPath() => Path.Join( DirPath(), NewFilesToBuild );
}