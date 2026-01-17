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
using Blazorise.Docs.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
#endregion

namespace Blazorise.Docs.Server.Infrastructure;

internal sealed class ServerJobsService : IJobsService
{
    #region Members

    private const string CacheKey = "JobsFeedCache";

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes( 10 );

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient httpClient;
    private readonly IMemoryCache cache;
    private readonly JobsOptions options;

    #endregion

    #region Constructors

    public ServerJobsService( HttpClient httpClient, IMemoryCache cache, IOptions<JobsOptions> options )
    {
        this.httpClient = httpClient;
        this.cache = cache;
        this.options = options.Value ?? new JobsOptions();

        this.httpClient.DefaultRequestHeaders.UserAgent.ParseAdd( "BlazoriseJobsFeed/1.0" );
        if ( !string.IsNullOrWhiteSpace( this.options.GitHubToken ) )
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", this.options.GitHubToken );
    }

    #endregion

    #region Methods

    public async Task<IReadOnlyList<JobPost>> GetJobsAsync( CancellationToken cancellationToken = default )
    {
        if ( cache.TryGetValue( CacheKey, out IReadOnlyList<JobPost> cachedJobs ) )
            return cachedJobs;

        IReadOnlyList<JobPost> jobs = await FetchJobsAsync( cancellationToken );

        MemoryCacheEntryOptions entryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        };

        cache.Set( CacheKey, jobs, entryOptions );

        return jobs;
    }

    public async Task RefreshAsync( CancellationToken cancellationToken = default )
    {
        IReadOnlyList<JobPost> jobs = await FetchJobsAsync( cancellationToken );

        MemoryCacheEntryOptions entryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        };

        cache.Set( CacheKey, jobs, entryOptions );
    }

    private async Task<IReadOnlyList<JobPost>> FetchJobsAsync( CancellationToken cancellationToken )
    {
        if ( string.IsNullOrWhiteSpace( options.FeedUrl ) )
            throw new InvalidOperationException( "Jobs feed URL is not configured." );

        Func<HttpRequestMessage> requestFactory = () =>
        {
            HttpRequestMessage request = new HttpRequestMessage( HttpMethod.Get, options.FeedUrl );
            request.Headers.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
            return request;
        };

        using HttpResponseMessage response = await SendWithRetryAsync( requestFactory, cancellationToken );
        response.EnsureSuccessStatusCode();

        await using Stream responseStream = await response.Content.ReadAsStreamAsync( cancellationToken );
        List<JobPost> items = await JsonSerializer.DeserializeAsync<List<JobPost>>( responseStream, JsonOptions, cancellationToken ) ?? new List<JobPost>();

        return items;
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

    #endregion
}