using System;

namespace Blazorise.Docs.BlogRuntime;

public static class BlogPermalinks
{
    public static string ToCanonicalBlogPath( string permalink )
        => "/blog" + NormalizePostPath( permalink );

    public static string ToLegacyNewsPath( string permalink )
        => "/news" + NormalizePostPath( permalink );

    private static string NormalizePostPath( string permalink )
    {
        string path = ( permalink ?? string.Empty ).Trim().Replace( '\\', '/' );

        if ( path.StartsWith( "/" ) )
        {
            path = path.TrimStart( '/' );
        }

        if ( string.Equals( path, "blog", StringComparison.OrdinalIgnoreCase ) )
        {
            path = string.Empty;
        }
        else if ( string.Equals( path, "news", StringComparison.OrdinalIgnoreCase ) )
        {
            path = string.Empty;
        }
        else if ( path.StartsWith( "blog/", StringComparison.OrdinalIgnoreCase ) )
        {
            path = path["blog".Length..].TrimStart( '/' );
        }
        else if ( path.StartsWith( "news/", StringComparison.OrdinalIgnoreCase ) )
        {
            path = path["news".Length..].TrimStart( '/' );
        }

        while ( path.Contains( "//", StringComparison.Ordinal ) )
        {
            path = path.Replace( "//", "/" );
        }

        path = path.TrimEnd( '/' );

        return string.IsNullOrWhiteSpace( path )
            ? string.Empty
            : "/" + path;
    }
}