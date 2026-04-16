using System;
using System.Collections.Generic;

namespace Blazorise.Docs.Server.Infrastructure;

static class PermanentRedirects
{
    private static readonly Dictionary<string, string> Map = new()
    {
        ["/checkout-order"] = "/purchase-order",
        ["/commercial/about"] = "/about",
        ["/commercial/community"] = "/community",
        ["/commercial/enterprise-plus"] = "/enterprise-plus",
        ["/commercial/pricing"] = "/pricing",
        ["/forgot-password"] = "/account/forgot-password",
        ["/login"] = "/account/login",
        ["/register"] = "/account/register",
        ["/docs/components/date"] = "/docs/components/date-input",
        ["/docs/components/file"] = "/docs/components/file-input",
        ["/docs/components/memo"] = "/docs/components/memo-input",
        ["/docs/components/numeric"] = "/docs/components/numeric-input",
        ["/docs/components/text"] = "/docs/components/text-input",
        ["/docs/components/time"] = "/docs/components/time-input",
        ["/docs/components/color"] = "/docs/components/color-input",
        ["/docs/components/dragdrop"] = "/docs/components/drag-drop",
        ["/docs/contributing"] = "https://github.com/Megabit/Blazorise/blob/master/CONTRIBUTING.md",
        ["/docs/helpers/utilities/fluent-sizing"] = "/docs/helpers/utilities/sizing",
        ["/docs/licence"] = "/docs/usage/licensing",
        ["/docs/releases"] = "/news",
        ["/docs/upgrade"] = "/docs/migration",
        ["/docs/usage/bootstrap"] = "/docs/usage/bootstrap5",
    };

    public static bool TryGetValue( string path, out string newPath )
    {
        path = NormalizePath( path );

        if ( Map.TryGetValue( path, out newPath ) )
        {
            return true;
        }

        if ( path.StartsWith( "/docs/release-notes/", StringComparison.Ordinal ) )
        {
            newPath = "/news";
            return true;
        }

        if ( IsBlocksAliasPath( path ) )
        {
            newPath = "/blocks" + path;
            return true;
        }

        newPath = null;
        return false;
    }

    private static string NormalizePath( string path )
    {
        path = path.ToLowerInvariant();

        if ( path.Length > 1 )
        {
            path = path.TrimEnd( '/' );
        }

        return path;
    }

    private static bool IsBlocksAliasPath( string path )
    {
        return path.StartsWith( "/applications/", StringComparison.Ordinal )
            || path.StartsWith( "/ecommerce/", StringComparison.Ordinal )
            || path.StartsWith( "/forms/", StringComparison.Ordinal )
            || path.StartsWith( "/marketing/", StringComparison.Ordinal )
            || path.StartsWith( "/publisher/", StringComparison.Ordinal );
    }
}