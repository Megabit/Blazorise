using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Builder;

namespace Blazorise.Demo.Bootstrap
{
    public class Startup
    {
        public void ConfigureServices( IServiceCollection services )
        {
            services
                .AddBlazorise( options =>
                {
                    options.ChangeTextOnKeyPress = true;
                } )
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
        }

        public void Configure( IComponentsApplicationBuilder app )
        {
            app
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            app.AddComponent<App>( "app" );
        }
    }
}
