using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.BlogRuntime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blazorise.Docs.Server.Infrastructure;

public sealed class BlogPreheater : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BlogPreheater> _logger;

    public BlogPreheater( IServiceProvider services, ILogger<BlogPreheater> logger )
    {
        _services = services;
        _logger = logger;
    }

    public Task StartAsync( CancellationToken cancellationToken )
    {
        _ = Task.Run( async () =>
        {
            using var scope = _services.CreateScope();
            var provider = (GithubBlogProvider)scope.ServiceProvider.GetRequiredService<IBlogProvider>();
            var opt = scope.ServiceProvider.GetRequiredService<IOptions<BlogOptions>>().Value;

            // Load bundle once
            await provider.PreheatAsync( cancellationToken );

            // Optional warm a few posts (parsing markdown)
            var top = await provider.GetListAsync( null, take: 6, skip: 0, cancellationToken );
            foreach ( var it in top )
                await provider.GetByPermalinkAsync( it.Permalink, cancellationToken );

            // Optional polling
            if ( opt.RuntimeVersionPoll is TimeSpan period && period > TimeSpan.Zero )
            {
                while ( !cancellationToken.IsCancellationRequested )
                {
                    try
                    {
                        await Task.Delay( period, cancellationToken );
                        await provider.RefreshAsync( cancellationToken ); // will no-op if version unchanged
                    }
                    catch ( OperationCanceledException ) { }
                    catch ( Exception ex ) { _logger.LogWarning( ex, "Version poll failed" ); }
                }
            }
        }, cancellationToken );

        return Task.CompletedTask;
    }

    public Task StopAsync( CancellationToken cancellationToken ) => Task.CompletedTask;
}