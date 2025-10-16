using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.BlogRuntime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            try
            {
                using var scope = _services.CreateScope();
                var blog = scope.ServiceProvider.GetRequiredService<IBlogProvider>();

                // 1) Warm list (populates in-memory cache + lets us know the latest slugs)
                var list = await blog.GetListAsync( null, null, 0, cancellationToken );

                // 2) Warm top N posts (configurable)
                const int PREHEAT_TOP_N = 7;
                foreach ( var item in list.Where( x => x.Root == "blog" ).Take( PREHEAT_TOP_N ).Concat( list.Where( x => x.Root == "news" ).Take( PREHEAT_TOP_N ) ) )
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await blog.GetByPermalinkAsync( item.Permalink, cancellationToken );
                }

                _logger.LogInformation( "Blog preheat complete: {Count} posts warmed", Math.Min( PREHEAT_TOP_N, list.Count ) );
            }
            catch ( OperationCanceledException ) { /* shutting down */ }
            catch ( Exception ex )
            {
                _logger.LogError( ex, "Blog preheat failed" );
            }
        }, cancellationToken );

        return Task.CompletedTask;
    }

    public Task StopAsync( CancellationToken cancellationToken ) => Task.CompletedTask;
}