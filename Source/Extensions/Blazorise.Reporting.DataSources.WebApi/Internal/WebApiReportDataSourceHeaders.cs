#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

internal static class WebApiReportDataSourceHeaders
{
    #region Members

    private const int MaximumHeaderCount = 32;

    private const int MaximumHeaderNameLength = 128;

    private const int MaximumHeaderValueLength = 4096;

    private static readonly HashSet<string> RestrictedHeaders = new( StringComparer.OrdinalIgnoreCase )
    {
        "Connection",
        "Content-Length",
        "Cookie",
        "Cookie2",
        "Expect",
        "Host",
        "Keep-Alive",
        "Proxy-Connection",
        "Proxy-Authorization",
        "TE",
        "Trailer",
        "Transfer-Encoding",
        "Upgrade",
        "X-HTTP-Method",
        "X-HTTP-Method-Override",
        "X-Method-Override",
    };

    #endregion

    #region Methods

    public static string Serialize( IReadOnlyDictionary<string, string> headers )
    {
        if ( headers is null || headers.Count == 0 )
            return null;

        return string.Join( "\n", headers
            .OrderBy( header => header.Key, StringComparer.OrdinalIgnoreCase )
            .Select( header => $"{header.Key}: {header.Value}" ) );
    }

    public static void Apply( string serializedHeaders, HttpRequestMessage request )
    {
        if ( string.IsNullOrWhiteSpace( serializedHeaders ) )
            return;

        string[] lines = serializedHeaders
            .Replace( "\r\n", "\n", StringComparison.Ordinal )
            .Replace( '\r', '\n' )
            .Split( '\n' );

        if ( lines.Count( line => !string.IsNullOrWhiteSpace( line ) ) > MaximumHeaderCount )
            throw new InvalidOperationException( $"Web API report data sources support at most {MaximumHeaderCount} request headers." );

        foreach ( string line in lines )
        {
            if ( string.IsNullOrWhiteSpace( line ) )
                continue;

            int separatorIndex = line.IndexOf( ':' );

            if ( separatorIndex <= 0 )
                throw new InvalidOperationException( "Each Web API request header must use the 'Name: Value' format." );

            string name = line[..separatorIndex].Trim();
            string value = line[( separatorIndex + 1 )..].Trim();

            Validate( name, value );

            if ( !request.Headers.TryAddWithoutValidation( name, value ) )
                throw new InvalidOperationException( $"Web API request header '{name}' is not valid for a GET request." );
        }
    }

    private static void Validate( string name, string value )
    {
        if ( name.Length == 0 || name.Length > MaximumHeaderNameLength || name.Any( character => !IsTokenCharacter( character ) ) )
            throw new InvalidOperationException( $"Web API request header name '{name}' is invalid." );

        if ( RestrictedHeaders.Contains( name ) )
            throw new InvalidOperationException( $"Web API request header '{name}' is controlled by the HTTP client and cannot be set by a report." );

        if ( value.Length > MaximumHeaderValueLength || value.Contains( '\r' ) || value.Contains( '\n' ) )
            throw new InvalidOperationException( $"Web API request header '{name}' has an invalid value." );
    }

    private static bool IsTokenCharacter( char character )
    {
        return char.IsAsciiLetterOrDigit( character )
            || character is '!' or '#' or '$' or '%' or '&' or '\'' or '*' or '+' or '-' or '.' or '^' or '_' or '`' or '|' or '~';
    }

    #endregion
}