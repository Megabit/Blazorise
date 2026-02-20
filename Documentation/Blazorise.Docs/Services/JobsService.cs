#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.Models;
using Blazorise.Docs.Options;
using Microsoft.Extensions.Options;
#endregion

namespace Blazorise.Docs.Services;

/// <summary>
/// Provides access to the jobs feed.
/// </summary>
public interface IJobsService
{
    /// <summary>
    /// Gets the list of job posts from the configured feed.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>A list of job posts.</returns>
    Task<IReadOnlyList<JobPost>> GetJobsAsync( CancellationToken cancellationToken = default );

    /// <summary>
    /// Forces a refresh of the jobs feed.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    Task RefreshAsync( CancellationToken cancellationToken = default );
}

/// <summary>
/// Client-side jobs service with ETag-based caching for browser hosts.
/// </summary>
public sealed class JobsService : IJobsService
{
    #region Members

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes( 10 );

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient httpClient;
    private readonly JobsOptions options;
    private readonly SemaphoreSlim refreshLock = new( 1, 1 );

    private IReadOnlyList<JobPost> cachedJobs = Array.Empty<JobPost>();
    private DateTimeOffset? cachedAt;
    private string cachedEtag;

    #endregion

    #region Constructors

    public JobsService( HttpClient httpClient, IOptions<JobsOptions> options )
    {
        this.httpClient = httpClient;
        this.options = options.Value ?? new JobsOptions();
    }

    #endregion

    #region Methods

    public async Task<IReadOnlyList<JobPost>> GetJobsAsync( CancellationToken cancellationToken = default )
    {
        if ( IsCacheFresh() )
            return cachedJobs;

        await refreshLock.WaitAsync( cancellationToken );
        try
        {
            if ( IsCacheFresh() )
                return cachedJobs;

            JobsFetchResult result = await FetchJobsAsync( cancellationToken );

            if ( result.NotModified && cachedJobs.Count > 0 )
            {
                cachedAt = DateTimeOffset.UtcNow;
                return cachedJobs;
            }

            cachedJobs = result.Jobs ?? Array.Empty<JobPost>();
            cachedEtag = result.ETag;
            cachedAt = DateTimeOffset.UtcNow;

            return cachedJobs;
        }
        finally
        {
            refreshLock.Release();
        }
    }

    public async Task RefreshAsync( CancellationToken cancellationToken = default )
    {
        await refreshLock.WaitAsync( cancellationToken );
        try
        {
            JobsFetchResult result = await FetchJobsAsync( cancellationToken );

            if ( result.NotModified && cachedJobs.Count > 0 )
            {
                cachedAt = DateTimeOffset.UtcNow;
                return;
            }

            cachedJobs = result.Jobs ?? Array.Empty<JobPost>();
            cachedEtag = result.ETag;
            cachedAt = DateTimeOffset.UtcNow;
        }
        finally
        {
            refreshLock.Release();
        }
    }

    private bool IsCacheFresh()
    {
        if ( cachedAt is null )
            return false;
        if ( cachedJobs.Count == 0 )
            return false;

        TimeSpan age = DateTimeOffset.UtcNow - cachedAt.Value;
        return age < CacheDuration;
    }

    private async Task<JobsFetchResult> FetchJobsAsync( CancellationToken cancellationToken )
    {
        if ( string.IsNullOrWhiteSpace( options.FeedUrl ) )
            throw new InvalidOperationException( "Jobs feed URL is not configured." );

        Func<HttpRequestMessage> requestFactory = () =>
        {
            HttpRequestMessage request = new HttpRequestMessage( HttpMethod.Get, options.FeedUrl );
            request.Headers.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
            if ( !string.IsNullOrWhiteSpace( options.GitHubToken ) )
                request.Headers.Authorization = new AuthenticationHeaderValue( "Bearer", options.GitHubToken );
            if ( !string.IsNullOrWhiteSpace( cachedEtag ) )
                request.Headers.IfNoneMatch.Add( EntityTagHeaderValue.Parse( cachedEtag ) );
            return request;
        };

        using HttpResponseMessage response = await SendWithRetryAsync( requestFactory, cancellationToken );

        if ( response.StatusCode == HttpStatusCode.NotModified )
        {
            return new JobsFetchResult
            {
                NotModified = true,
                ETag = cachedEtag
            };
        }

        response.EnsureSuccessStatusCode();

        string etag = response.Headers.ETag?.ToString();
        await using Stream responseStream = await response.Content.ReadAsStreamAsync( cancellationToken );
        List<JobPost> items = await JsonSerializer.DeserializeAsync<List<JobPost>>( responseStream, JsonOptions, cancellationToken ) ?? new List<JobPost>();

        return new JobsFetchResult
        {
            Jobs = items,
            ETag = etag
        };
    }

    private async Task<HttpResponseMessage> SendWithRetryAsync( Func<HttpRequestMessage> requestFactory, CancellationToken cancellationToken )
    {
        int attempt = 0;
        int maxAttempts = 3;

        while ( true )
        {
            using HttpRequestMessage request = requestFactory();
            try
            {
                HttpResponseMessage response = await httpClient.SendAsync( request, HttpCompletionOption.ResponseHeadersRead, cancellationToken );
                if ( IsTransientStatusCode( response.StatusCode ) && attempt < maxAttempts - 1 )
                {
                    response.Dispose();
                    attempt++;
                    await DelayForRetryAsync( attempt, cancellationToken );
                    continue;
                }

                return response;
            }
            catch ( HttpRequestException ) when ( attempt < maxAttempts - 1 )
            {
                attempt++;
                await DelayForRetryAsync( attempt, cancellationToken );
            }
            catch ( TaskCanceledException ) when ( !cancellationToken.IsCancellationRequested && attempt < maxAttempts - 1 )
            {
                attempt++;
                await DelayForRetryAsync( attempt, cancellationToken );
            }
        }
    }

    private static bool IsTransientStatusCode( HttpStatusCode statusCode )
    {
        return statusCode == HttpStatusCode.RequestTimeout
               || statusCode == HttpStatusCode.BadGateway
               || statusCode == HttpStatusCode.ServiceUnavailable
               || statusCode == HttpStatusCode.GatewayTimeout;
    }

    private static Task DelayForRetryAsync( int attempt, CancellationToken cancellationToken )
    {
        int delayMilliseconds = 250 * attempt;
        return Task.Delay( delayMilliseconds, cancellationToken );
    }

    private sealed class JobsFetchResult
    {
        public IReadOnlyList<JobPost> Jobs { get; set; } = Array.Empty<JobPost>();
        public string ETag { get; set; }
        public bool NotModified { get; set; }
    }

    #endregion
}