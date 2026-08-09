#region Using directives
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

/// <summary>
/// Configures Web API report data source request policy and resource limits.
/// </summary>
public sealed class WebApiReportDataSourceOptions
{
    #region Members

    internal const long DefaultMaximumResponseSize = 5 * 1024 * 1024;

    internal const int DefaultMaximumCollectionItems = 10000;

    internal const int DefaultMaximumSchemaItems = 100;

    internal const int DefaultMaximumJsonDepth = 64;

    internal const int DefaultMaximumXmlDepth = 64;

    internal static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds( 30 );

    #endregion

    #region Methods

    internal void Validate()
    {
        if ( MaximumResponseSize <= 0 || MaximumResponseSize > int.MaxValue )
            throw new ArgumentOutOfRangeException( nameof( MaximumResponseSize ), $"The maximum Web API response size must be between 1 and {int.MaxValue} bytes." );

        if ( MaximumCollectionItems <= 0 )
            throw new ArgumentOutOfRangeException( nameof( MaximumCollectionItems ), "The maximum Web API collection size must be greater than zero." );

        if ( MaximumSchemaItems <= 0 )
            throw new ArgumentOutOfRangeException( nameof( MaximumSchemaItems ), "The maximum number of Web API schema items must be greater than zero." );

        if ( MaximumJsonDepth <= 0 )
            throw new ArgumentOutOfRangeException( nameof( MaximumJsonDepth ), "The maximum Web API JSON depth must be greater than zero." );

        if ( MaximumXmlDepth <= 0 )
            throw new ArgumentOutOfRangeException( nameof( MaximumXmlDepth ), "The maximum Web API XML depth must be greater than zero." );

        if ( RequestTimeout <= TimeSpan.Zero )
            throw new ArgumentOutOfRangeException( nameof( RequestTimeout ), "The Web API request timeout must be greater than zero." );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Determines whether a report-supplied absolute HTTP or HTTPS URL is allowed.
    /// A null value allows every otherwise valid URL. Server applications always reject loopback and non-public destinations before this policy can grant access.
    /// </summary>
    public Func<Uri, bool> ResourceAllowed { get; set; }

    /// <summary>
    /// Optional host callback that adds authentication or other application-owned request headers.
    /// The callback receives the validated request URL and cannot change the GET method or request URI.
    /// Only add credentials after matching the URL against an application-owned allowlist.
    /// </summary>
    public Func<IServiceProvider, Uri, HttpRequestMessage, CancellationToken, ValueTask> ConfigureRequestAsync { get; set; }

    /// <summary>
    /// Maximum number of response bytes buffered by the provider.
    /// </summary>
    public long MaximumResponseSize { get; set; } = DefaultMaximumResponseSize;

    /// <summary>
    /// Maximum number of items allowed in any JSON or XML collection.
    /// </summary>
    public int MaximumCollectionItems { get; set; } = DefaultMaximumCollectionItems;

    /// <summary>
    /// Maximum number of collection items inspected when inferring a schema.
    /// </summary>
    public int MaximumSchemaItems { get; set; } = DefaultMaximumSchemaItems;

    /// <summary>
    /// Maximum JSON document depth accepted by the built-in JSON reader.
    /// </summary>
    public int MaximumJsonDepth { get; set; } = DefaultMaximumJsonDepth;

    /// <summary>
    /// Maximum XML element depth accepted by the built-in XML reader.
    /// </summary>
    public int MaximumXmlDepth { get; set; } = DefaultMaximumXmlDepth;

    /// <summary>
    /// Maximum duration of a Web API request.
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = DefaultRequestTimeout;

    #endregion
}