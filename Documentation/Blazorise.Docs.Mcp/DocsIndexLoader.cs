#region Using directives
using System;
using System.IO;
using System.Text;
using System.Text.Json;
#endregion

namespace Blazorise.Docs.Mcp;

internal static class DocsIndexLoader
{
    internal static TIndex Load<TIndex>( string fileName ) where TIndex : class
    {
        var indexPath = FindIndexPath( fileName );
        var json = File.ReadAllText( indexPath, Encoding.UTF8 );

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        return JsonSerializer.Deserialize<TIndex>( json, options )
            ?? throw new InvalidOperationException( $"{fileName} could not be loaded." );
    }

    private static string FindIndexPath( string fileName )
    {
        var localPath = Path.Combine( AppContext.BaseDirectory, fileName );

        if ( File.Exists( localPath ) )
        {
            return localPath;
        }

        throw new FileNotFoundException( $"{fileName} not found. Build Blazorise.Docs.Mcp with GenerateDocs=true to generate and copy the documentation indexes.", localPath );
    }
}