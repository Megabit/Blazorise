using System.IO;
using System.Linq;

namespace Blazorise.Docs.Compiler
{
    public class Paths
    {
        private const string DocsDirectory = "Blazorise.Docs";
        private const string SnippetsFile = "Snippets.generated.cs";
        private const string DocStringsFile = "DocStrings.generated.cs";
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

        public string DocsDirPath => Directory.EnumerateDirectories( RootDirPath, DocsDirectory ).FirstOrDefault();

        public string DocsStringSnippetsDirPath => Path.Join( DocsDirPath, "Models" );

        public string DocStringsFilePath => Path.Join( DocsStringSnippetsDirPath, DocStringsFile );

        public string SnippetsFilePath => Path.Join( DocsStringSnippetsDirPath, SnippetsFile );

        public string NewFilesToBuildPath => Path.Join( DocsDirPath, NewFilesToBuild );
    }
}
