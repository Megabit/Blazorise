using Blazorise.Bootstrap;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.Demo.Bootstrap
{
    public class Startup
    {
        public void ConfigureServices( IServiceCollection services )
        {
            services
                .AddBootstrapProviders()
                .AddIconProvider( IconProvider.FontAwesome );
        }

        public void Configure( IBlazorApplicationBuilder app )
        {
            app.AddComponent<App>( "app" );
        }
    }
}
