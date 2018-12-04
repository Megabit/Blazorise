using Blazorise.Bootstrap;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.Demo.Client
{
    public class Startup
    {
        public void ConfigureServices( IServiceCollection services )
        {
            services
                .AddBootstrap()
                .AddIconProvider( IconProvider.FontAwesome );
        }

        public void Configure( IBlazorApplicationBuilder app )
        {
            app.AddComponent<App>( "app" );
        }
    }
}
