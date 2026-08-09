#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

/// <summary>
/// Loads report data from HTTP API endpoints.
/// </summary>
public sealed class WebApiReportDataSourceProvider : IReportDataSourceProvider
{
    #region Members

    /// <summary>
    /// Provider type used by Web API data source definitions.
    /// </summary>
    public const string ProviderType = "web-api";

    internal const string HttpClientName = "Blazorise.Reporting.DataSources.WebApi";

    private readonly IServiceProvider serviceProvider;

    private readonly HttpClient httpClient;

    private readonly WebApiReportDataSourceOptions options;

    private readonly IReadOnlyList<IReportWebApiResponseReader> responseReaders;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a Web API report data source provider.
    /// </summary>
    /// <param name="serviceProvider">Service provider supplied to host-owned request configuration.</param>
    /// <param name="httpClientFactory">HTTP client factory.</param>
    /// <param name="options">Web API data source options.</param>
    /// <param name="responseReaders">Registered response readers.</param>
    public WebApiReportDataSourceProvider( IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory, WebApiReportDataSourceOptions options, IEnumerable<IReportWebApiResponseReader> responseReaders )
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException( nameof( serviceProvider ) );
        httpClient = ( httpClientFactory ?? throw new ArgumentNullException( nameof( httpClientFactory ) ) ).CreateClient( HttpClientName );
        this.options = options ?? throw new ArgumentNullException( nameof( options ) );
        this.options.Validate();
        this.responseReaders = responseReaders?.ToList() ?? throw new ArgumentNullException( nameof( responseReaders ) );

        if ( this.responseReaders.Count == 0 || this.responseReaders.Any( reader => reader is null || string.IsNullOrWhiteSpace( reader.Format ) ) )
            throw new InvalidOperationException( "At least one Web API response reader with a non-empty format must be registered." );

        string duplicateFormat = this.responseReaders
            .GroupBy( reader => reader.Format, StringComparer.OrdinalIgnoreCase )
            .FirstOrDefault( group => group.Count() > 1 )
            ?.Key;

        if ( duplicateFormat is not null )
            throw new InvalidOperationException( $"More than one Web API response reader is registered for format '{duplicateFormat}'." );

        if ( this.responseReaders.Any( reader => string.Equals( reader?.Format, WebApiReportDataSourceFormats.Auto, StringComparison.OrdinalIgnoreCase ) ) )
            throw new InvalidOperationException( $"'{WebApiReportDataSourceFormats.Auto}' is reserved for automatic Web API response format detection." );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public async Task<ReportDataSourceSchema> GetSchemaAsync( ReportDataSourceDefinition definition, CancellationToken cancellationToken = default )
    {
        ReportDataSourceResult result = await ReadResponse( definition, cancellationToken );

        return result.Schema;
    }

    /// <inheritdoc />
    public Task<ReportDataSourceResult> LoadDataAsync( ReportDataSourceDefinition definition, ReportDataSourceLoadContext context, CancellationToken cancellationToken = default )
    {
        return ReadResponse( definition, cancellationToken );
    }

    private async Task<ReportDataSourceResult> ReadResponse( ReportDataSourceDefinition definition, CancellationToken cancellationToken )
    {
        string url = GetRequiredSetting( definition, WebApiReportDataSourceSettings.Url );
        string headers = GetSetting( definition, WebApiReportDataSourceSettings.Headers );
        string responseFormat = GetSetting( definition, WebApiReportDataSourceSettings.ResponseFormat ) ?? WebApiReportDataSourceFormats.Auto;
        string selector = GetSetting( definition, WebApiReportDataSourceSettings.DataSelector );

        ValidateResponseFormat( responseFormat );

        Uri requestUri = CreateRequestUri( url );
        string resourceDescription = DescribeRequestUri( requestUri );

        if ( options.ResourceAllowed?.Invoke( requestUri ) == false )
            throw new InvalidOperationException( $"The Web API report data source URL '{resourceDescription}' is not allowed by the configured resource policy." );

        using CancellationTokenSource timeoutSource = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken );
        timeoutSource.CancelAfter( options.RequestTimeout );

        try
        {
            using HttpRequestMessage request = new( HttpMethod.Get, requestUri );
            WebApiReportDataSourceHeaders.Apply( headers, request );

            if ( !OperatingSystem.IsBrowser() )
                await WebApiPublicNetworkGuard.EnsurePublicDestinationAsync( requestUri, timeoutSource.Token );

            if ( options.ConfigureRequestAsync is not null )
                await options.ConfigureRequestAsync( serviceProvider, requestUri, request, timeoutSource.Token );

            if ( request.Method != HttpMethod.Get || !Equals( request.RequestUri, requestUri ) || request.Content is not null )
                throw new InvalidOperationException( "Web API request configuration cannot change the GET method or request URI, or add request content." );

            using HttpResponseMessage response = await httpClient.SendAsync( request, HttpCompletionOption.ResponseHeadersRead, timeoutSource.Token );

            if ( !Equals( response.RequestMessage?.RequestUri, requestUri ) || IsRedirect( response.StatusCode ) )
                throw new InvalidOperationException( $"The Web API report data source URL '{resourceDescription}' returned a redirect. Redirects are not allowed." );

            if ( !response.IsSuccessStatusCode )
                throw new InvalidOperationException( $"The Web API report data source URL '{resourceDescription}' returned HTTP status {(int)response.StatusCode}." );

            if ( response.Content.Headers.ContentLength is long contentLength && contentLength > options.MaximumResponseSize )
                throw new InvalidDataException( $"The Web API report data source response exceeds the configured limit of {options.MaximumResponseSize} bytes." );

            ReadOnlyMemory<byte> content = await ReadResponseBytes( response, timeoutSource.Token );
            string mediaType = response.Content.Headers.ContentType?.MediaType;
            IReportWebApiResponseReader responseReader = ResolveResponseReader( responseFormat, mediaType, content );
            ReportDataSourceResult result = await responseReader.ReadAsync( content, selector, timeoutSource.Token );

            if ( result?.Schema is null )
                throw new InvalidOperationException( $"The Web API response reader '{responseReader.Format}' returned no schema." );

            return result;
        }
        catch ( OperationCanceledException ) when ( !cancellationToken.IsCancellationRequested )
        {
            throw new TimeoutException( $"The Web API report data source URL '{resourceDescription}' exceeded the configured request timeout." );
        }
    }

    private async Task<ReadOnlyMemory<byte>> ReadResponseBytes( HttpResponseMessage response, CancellationToken cancellationToken )
    {
        using Stream source = await response.Content.ReadAsStreamAsync( cancellationToken );
        using MemoryStream target = new();
        byte[] buffer = new byte[81920];
        long totalBytes = 0;
        int bytesRead;

        while ( ( bytesRead = await source.ReadAsync( buffer, cancellationToken ) ) > 0 )
        {
            totalBytes += bytesRead;

            if ( totalBytes > options.MaximumResponseSize )
                throw new InvalidDataException( $"The Web API report data source response exceeds the configured limit of {options.MaximumResponseSize} bytes." );

            await target.WriteAsync( buffer.AsMemory( 0, bytesRead ), cancellationToken );
        }

        return target.ToArray();
    }

    private IReportWebApiResponseReader ResolveResponseReader( string responseFormat, string mediaType, ReadOnlyMemory<byte> content )
    {
        if ( !string.Equals( responseFormat, WebApiReportDataSourceFormats.Auto, StringComparison.OrdinalIgnoreCase ) )
        {
            IReportWebApiResponseReader configuredReader = responseReaders.FirstOrDefault( reader => string.Equals( reader.Format, responseFormat, StringComparison.OrdinalIgnoreCase ) );

            return configuredReader ?? throw new InvalidOperationException( $"No Web API response reader is registered for format '{responseFormat}'." );
        }

        IReportWebApiResponseReader detectedReader = responseReaders.FirstOrDefault( reader => reader.CanRead( mediaType, content ) );

        return detectedReader ?? throw new InvalidOperationException( "The Web API response format could not be detected. Select an explicitly registered response format." );
    }

    private void ValidateResponseFormat( string responseFormat )
    {
        if ( string.IsNullOrWhiteSpace( responseFormat ) )
            throw new InvalidOperationException( "The Web API response format cannot be empty." );

        if ( !string.Equals( responseFormat, WebApiReportDataSourceFormats.Auto, StringComparison.OrdinalIgnoreCase )
             && !responseReaders.Any( reader => string.Equals( reader.Format, responseFormat, StringComparison.OrdinalIgnoreCase ) ) )
        {
            throw new InvalidOperationException( $"No Web API response reader is registered for format '{responseFormat}'." );
        }
    }

    private static Uri CreateRequestUri( string url )
    {
        if ( url.Contains( "\\", StringComparison.Ordinal )
             || !Uri.TryCreate( url, UriKind.Absolute, out Uri requestUri )
             || ( requestUri.Scheme != Uri.UriSchemeHttp && requestUri.Scheme != Uri.UriSchemeHttps )
             || string.IsNullOrWhiteSpace( requestUri.Host ) )
        {
            throw new InvalidOperationException( "The Web API report data source URL must be an absolute HTTP or HTTPS URL." );
        }

        if ( !string.IsNullOrEmpty( requestUri.UserInfo ) || !string.IsNullOrEmpty( requestUri.Fragment ) )
            throw new InvalidOperationException( "The Web API report data source URL cannot contain credentials or a fragment." );

        return requestUri;
    }

    private static string DescribeRequestUri( Uri requestUri )
    {
        return requestUri.GetComponents( UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.UriEscaped );
    }

    private static bool IsRedirect( HttpStatusCode statusCode )
    {
        return statusCode is HttpStatusCode.MultipleChoices
            or HttpStatusCode.MovedPermanently
            or HttpStatusCode.Found
            or HttpStatusCode.SeeOther
            or HttpStatusCode.TemporaryRedirect
            or HttpStatusCode.PermanentRedirect;
    }

    private static string GetRequiredSetting( ReportDataSourceDefinition definition, string key )
    {
        string value = GetSetting( definition, key );

        if ( string.IsNullOrWhiteSpace( value ) )
            throw new InvalidOperationException( $"Web API report data source setting '{key}' is required." );

        return value;
    }

    private static string GetSetting( ReportDataSourceDefinition definition, string key )
    {
        if ( definition?.Settings is null || !definition.Settings.TryGetValue( key, out object value ) )
            return null;

        return Convert.ToString( value, CultureInfo.InvariantCulture );
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public string Type => ProviderType;

    /// <inheritdoc />
    public string DisplayName => "REST / Web API";

    /// <inheritdoc />
    public Type EditorComponentType => typeof( _WebApiReportDataSourceEditor );

    #endregion
}