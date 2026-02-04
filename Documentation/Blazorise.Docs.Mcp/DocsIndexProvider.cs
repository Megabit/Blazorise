using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Blazorise.Docs.Mcp;

internal static class DocsIndexProvider
{
    private static readonly Lazy<DocsIndex> Cached = new Lazy<DocsIndex>( LoadIndex );

    public static DocsIndex GetIndex()
    {
        return Cached.Value;
    }

    private static DocsIndex LoadIndex()
    {
        string indexPath = FindDocsIndexPath();
        string json = File.ReadAllText( indexPath, Encoding.UTF8 );

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        DocsIndex index = JsonSerializer.Deserialize<DocsIndex>( json, options );

        if ( index is null )
            throw new InvalidOperationException( "docs-index.json could not be loaded." );

        return index;
    }

    private static string FindDocsIndexPath()
    {
        string baseDirectory = AppContext.BaseDirectory;
        string localPath = Path.Combine( baseDirectory, "docs-index.json" );

        if ( File.Exists( localPath ) )
            return localPath;

        string repositoryPath = TryFindFromRoot( Directory.GetCurrentDirectory() );

        if ( repositoryPath is not null )
            return repositoryPath;

        throw new FileNotFoundException( "docs-index.json not found. Run Blazorise.Docs.Compiler to generate it.", localPath );
    }

    private static string TryFindFromRoot( string startPath )
    {
        DirectoryInfo current = new DirectoryInfo( startPath );

        while ( current is not null )
        {
            string candidate = Path.Combine( current.FullName, "Documentation", "Blazorise.Docs", "Resources", "docs-index.json" );

            if ( File.Exists( candidate ) )
                return candidate;

            current = current.Parent;
        }

        return null;
    }
}