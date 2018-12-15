using Blazorise.Bulma;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.Demo.Bulma
{
    public class Startup
    {
        public void ConfigureServices( IServiceCollection services )
        {
            services
                .AddBulmaProviders()
                .AddIconProvider( IconProvider.FontAwesome );
        }

        public void Configure( IBlazorApplicationBuilder app )
        {
            app.AddComponent<App>( "app" );
        }
    }
}
