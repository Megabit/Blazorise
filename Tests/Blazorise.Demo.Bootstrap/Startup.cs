using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
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
                .AddFontAwesomeIcons();
        }

        public void Configure( IBlazorApplicationBuilder app )
        {
            app.Services
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            app.AddComponent<App>( "app" );
        }
    }
}
