using System.IO;
using System.Linq;

namespace Blazorise.Docs.Compiler
{
    public class Paths
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

        public string DirPath() => Directory.EnumerateDirectories( RootDirPath, $"Blazorise.Docs" ).FirstOrDefault();

        public string DocsStringSnippetsDirPath() => Path.Join( DirPath(), "Models" );

        public string DocStringsFilePath() => Path.Join( DocsStringSnippetsDirPath(), "Strings.generated.cs" );

        public string SnippetsFilePath() => Path.Join( DocsStringSnippetsDirPath(), "Snippets.generated.cs" );

        public string NewFilesToBuildPath() => Path.Join( DirPath(), NewFilesToBuild );
    }
}