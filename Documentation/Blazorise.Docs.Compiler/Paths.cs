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

        public string DirPath( string category ) => Directory.EnumerateDirectories( RootDirPath, $"Blazorise.Docs" ).FirstOrDefault();

        public string DocsStringSnippetsDirPath( string category ) => Path.Join( DirPath( category ), "Models" );

        public string DocStringsFilePath( string category ) => Path.Join( DocsStringSnippetsDirPath( category ), $"{category}Strings.generated.cs" );

        public string SnippetsFilePath( string category ) => Path.Join( DocsStringSnippetsDirPath( category ), $"{category}Snippets.generated.cs" );

        public string NewFilesToBuildPath( string category ) => Path.Join( DirPath( category ), NewFilesToBuild );
    }
}