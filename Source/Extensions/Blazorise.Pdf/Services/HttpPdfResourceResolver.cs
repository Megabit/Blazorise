#region Using directives
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Resolves PDF image and font resources from HTTP sources.
/// </summary>
/// <remarks>
/// Automatic redirects must be disabled on the configured HTTP handler. <see cref="Config.AddBlazorisePdfHttpResources"/> does this by default for server applications.
/// </remarks>
public sealed class HttpPdfResourceResolver : PdfResourceResolver
{
    #region Members

    internal const string HttpClientName = "Blazorise.Pdf.Resources";

    private readonly HttpClient httpClient;

    private readonly PdfHttpResourceOptions options;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new HTTP PDF resource resolver.
    /// </summary>
    /// <param name="httpClientFactory">HTTP client factory.</param>
    /// <param name="options">HTTP resource options.</param>
    public HttpPdfResourceResolver( IHttpClientFactory httpClientFactory, PdfHttpResourceOptions options )
    {
        this.httpClient = ( httpClientFactory ?? throw new ArgumentNullException( nameof( httpClientFactory ) ) ).CreateClient( HttpClientName );
        this.options = options ?? throw new ArgumentNullException( nameof( options ) );
        this.options.Validate();
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task<PdfResourceContent> ResolveImageAsync( string source, CancellationToken cancellationToken = default )
    {
        if ( source?.StartsWith( "data:", StringComparison.OrdinalIgnoreCase ) == true )
            return await base.ResolveImageAsync( source, cancellationToken );

        return await ResolveHttpResource( source, cancellationToken );
    }

    /// <inheritdoc />
    public override async Task<PdfResourceContent> ResolveFontAsync( FontSource source, CancellationToken cancellationToken = default )
    {
        ValidateFontSource( source );

        if ( source.Data is { Length: > 0 } || !string.IsNullOrWhiteSpace( source.FileName ) )
            return await base.ResolveFontAsync( source, cancellationToken );

        if ( string.IsNullOrWhiteSpace( source.Url ) )
            return await base.ResolveFontAsync( source, cancellationToken );

        return await ResolveHttpResource( source.Url, cancellationToken );
    }

    private async Task<PdfResourceContent> ResolveHttpResource( string source, CancellationToken cancellationToken )
    {
        if ( string.IsNullOrWhiteSpace( source ) )
            throw new InvalidDataException( "The PDF resource URL is missing." );

        Uri resourceUri = ResolveResourceUri( source );
        string resourceDescription = DescribeResourceUri( resourceUri );

        if ( options.ResourceAllowed?.Invoke( resourceUri ) == false )
            throw new InvalidOperationException( $"The PDF HTTP resource '{resourceDescription}' is not allowed by the configured resource policy." );

        using HttpResponseMessage response = await httpClient.GetAsync( resourceUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken );

        Uri responseUri = response.RequestMessage?.RequestUri;

        if ( responseUri is not null && responseUri != resourceUri )
            throw new InvalidOperationException( $"The PDF HTTP resource '{resourceDescription}' redirected to '{DescribeResourceUri( responseUri )}'. Redirects are not allowed." );

        if ( IsRedirect( response.StatusCode ) )
        {
            Uri redirectUri = ResolveRedirectUri( resourceUri, response.Headers.Location );
            string redirectDescription = redirectUri is null ? "another location" : $"'{DescribeResourceUri( redirectUri )}'";

            throw new InvalidOperationException( $"The PDF HTTP resource '{resourceDescription}' redirected to {redirectDescription}. Redirects are not allowed." );
        }

        response.EnsureSuccessStatusCode();

        if ( response.Content.Headers.ContentLength is long contentLength && contentLength > options.MaxResourceSize )
            throw new InvalidDataException( $"The PDF HTTP resource '{resourceDescription}' exceeds the maximum allowed size of {options.MaxResourceSize} bytes." );

        byte[] data = await ReadContentAsync( response.Content, resourceDescription, cancellationToken );

        if ( data.Length == 0 )
            throw new InvalidDataException( $"The PDF HTTP resource '{resourceDescription}' returned no data." );

        return new( data, response.Content.Headers.ContentType?.MediaType );
    }

    private Uri ResolveResourceUri( string source )
    {
        if ( !Uri.TryCreate( source, UriKind.RelativeOrAbsolute, out Uri resourceUri ) )
            throw new InvalidDataException( "The PDF HTTP resource URL is invalid." );

        if ( !resourceUri.IsAbsoluteUri )
        {
            if ( httpClient.BaseAddress is null )
                throw new InvalidOperationException( "A base address is required to resolve a relative PDF HTTP resource URL." );

            resourceUri = new( httpClient.BaseAddress, resourceUri );
        }

        if ( resourceUri.Scheme != Uri.UriSchemeHttp && resourceUri.Scheme != Uri.UriSchemeHttps )
            throw new NotSupportedException( $"The PDF HTTP resource URL scheme '{resourceUri.Scheme}' is not supported." );

        return resourceUri;
    }

    private static Uri ResolveRedirectUri( Uri resourceUri, Uri location )
    {
        if ( location is null )
            return null;

        return location.IsAbsoluteUri ? location : new( resourceUri, location );
    }

    private static bool IsRedirect( HttpStatusCode statusCode )
        => statusCode is HttpStatusCode.MultipleChoices
            or HttpStatusCode.MovedPermanently
            or HttpStatusCode.Found
            or HttpStatusCode.SeeOther
            or HttpStatusCode.TemporaryRedirect
            or HttpStatusCode.PermanentRedirect;

    private async Task<byte[]> ReadContentAsync( HttpContent content, string resourceDescription, CancellationToken cancellationToken )
    {
        using Stream sourceStream = await content.ReadAsStreamAsync( cancellationToken );
        using MemoryStream targetStream = new();
        byte[] buffer = new byte[81920];
        long totalBytes = 0;
        int bytesRead;

        while ( ( bytesRead = await sourceStream.ReadAsync( buffer, cancellationToken ) ) > 0 )
        {
            totalBytes += bytesRead;

            if ( totalBytes > options.MaxResourceSize )
                throw new InvalidDataException( $"The PDF HTTP resource '{resourceDescription}' exceeds the maximum allowed size of {options.MaxResourceSize} bytes." );

            await targetStream.WriteAsync( buffer.AsMemory( 0, bytesRead ), cancellationToken );
        }

        return targetStream.ToArray();
    }

    private static string DescribeResourceUri( Uri resourceUri )
    {
        return resourceUri.GetComponents( UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.UriEscaped );
    }

    #endregion
}