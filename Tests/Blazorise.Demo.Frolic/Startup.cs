using Blazorise.Frolic;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.Demo.Frolic
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
                .AddFrolicProviders()
                .AddFontAwesomeIcons();
        }

        public void Configure( IComponentsApplicationBuilder app )
        {
            app.Services
                .UseFrolicProviders()
                .UseFontAwesomeIcons();

            app.AddComponent<App>( "app" );
        }
    }
}
