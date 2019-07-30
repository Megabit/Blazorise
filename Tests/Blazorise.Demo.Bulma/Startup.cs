using Blazorise.Bulma;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.Demo.Bulma
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
                .AddBulmaProviders()
                .AddFontAwesomeIcons();
        }

        public void Configure( IComponentsApplicationBuilder app )
        {
            app.Services
                .UseBulmaProviders()
                .UseFontAwesomeIcons();

            app.AddComponent<App>( "app" );
        }
    }
}
