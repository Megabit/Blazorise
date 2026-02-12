using System;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Extensions;

/// <summary>
/// <see cref="NavigationManager"/> extension methods.
/// </summary>
public static class NavigationExtensions
{
    /// <param name="nav">The Navigation manager</param>
    extension( NavigationManager nav )
    {
        /// <summary>
        /// Is the given uri a match for the current uri?
        /// </summary>
        /// <param name="uri">The uri to match</param>
        /// <param name="match">Matching conditions</param>
        /// <param name="customMatch">[Optional] Custom match logic</param>
        /// <returns></returns>
        public bool IsMatch( string uri, Match match, Func<string, bool> customMatch )
        {
            var currentUriAbsolute = nav.Uri;
            var absoluteUri = nav.GetAbsoluteUri( uri );

            if ( EqualsHrefExactlyOrIfTrailingSlashAdded( currentUriAbsolute, absoluteUri ) )
            {
                return true;
            }

            if ( match == Match.Prefix && IsStrictlyPrefixWithSeparator( currentUriAbsolute, absoluteUri ) )
            {
                return true;
            }

            return customMatch?.Invoke( currentUriAbsolute ) is true;
        }

        /// <summary>
        /// Converts a relative URI into an absolute one (by resolving it relative to the
        /// current absolute URI).
        /// </summary>
        /// <param name="relativeUri">The relative URI.</param>
        /// <returns>The absolute URI.</returns>
        public string GetAbsoluteUri( string relativeUri )
        {
            try
            {
                if ( relativeUri is null )
                    return string.Empty;

                if ( relativeUri.StartsWith( "mailto:", StringComparison.OrdinalIgnoreCase ) )
                    return relativeUri;

                return nav.ToAbsoluteUri( relativeUri ).AbsoluteUri;
            }
            catch
            {
                return relativeUri;
            }
        }
    }

    private static bool EqualsHrefExactlyOrIfTrailingSlashAdded( string currentUriAbsolute, string absoluteUri )
    {
        if ( string.Equals( currentUriAbsolute, absoluteUri, StringComparison.Ordinal ) )
        {
            return true;
        }

        if ( currentUriAbsolute.Length == absoluteUri.Length - 1 )
        {
            // Special case: highlight links to http://host/path/ even if you're
            // at http://host/path (with no trailing slash)
            //
            // This is because the router accepts an absolute URI value of "same
            // as base URI but without trailing slash" as equivalent to "base URI",
            // which in turn is because it's common for servers to return the same page
            // for http://host/vdir as they do for host://host/vdir/ as it's no
            // good to display a blank page in that case.
            if ( absoluteUri[^1] == '/'
                 && absoluteUri.StartsWith( currentUriAbsolute, StringComparison.Ordinal ) )
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsStrictlyPrefixWithSeparator( string value, string prefix )
    {
        var prefixLength = prefix.Length;
        if ( value.Length > prefixLength )
        {
            return value.StartsWith( prefix, StringComparison.Ordinal )
                   && (
                       // Only match when there's a separator character either at the end of the
                       // prefix or right after it.
                       // Example: "/abc" is treated as a prefix of "/abc/def" but not "/abcdef"
                       // Example: "/abc/" is treated as a prefix of "/abc/def" but not "/abcdef"
                       prefixLength == 0
                       || !char.IsLetterOrDigit( prefix[prefixLength - 1] )
                       || !char.IsLetterOrDigit( value[prefixLength] )
                   );
        }
        else
        {
            return false;
        }
    }
}