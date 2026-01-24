using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Blazorise.Docs.Mcp;

internal static class DocsApiIndexProvider
{
    private static readonly Lazy<DocsApiIndex> Cached = new Lazy<DocsApiIndex>( LoadIndex );

    public static DocsApiIndex GetIndex()
    {
        return Cached.Value;
    }

    private static DocsApiIndex LoadIndex()
    {
        string indexPath = FindDocsApiIndexPath();
        string json = File.ReadAllText( indexPath, Encoding.UTF8 );

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        DocsApiIndex index = JsonSerializer.Deserialize<DocsApiIndex>( json, options );

        if ( index is null )
            throw new InvalidOperationException( "docs-api-index.json could not be loaded." );

        return index;
    }

    private static string FindDocsApiIndexPath()
    {
        string baseDirectory = AppContext.BaseDirectory;
        string localPath = Path.Combine( baseDirectory, "docs-api-index.json" );

        if ( File.Exists( localPath ) )
            return localPath;

        string repositoryPath = TryFindFromRoot( Directory.GetCurrentDirectory() );

        if ( repositoryPath is not null )
            return repositoryPath;

        throw new FileNotFoundException( "docs-api-index.json not found. Run Blazorise.Docs.Compiler to generate it.", localPath );
    }

    private static string TryFindFromRoot( string startPath )
    {
        DirectoryInfo current = new DirectoryInfo( startPath );

        while ( current is not null )
        {
            string candidate = Path.Combine( current.FullName, "Documentation", "Blazorise.Docs", "Resources", "docs-api-index.json" );

            if ( File.Exists( candidate ) )
                return candidate;

            current = current.Parent;
        }

        return null;
    }
}