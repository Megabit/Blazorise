using Blazorise.Material;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.Demo.Material
{
    public class Startup
    {
        public void ConfigureServices( IServiceCollection services )
        {
            services
                .AddMaterialProviders()
                .AddFontAwesomeIcons();
        }

        public void Configure( IBlazorApplicationBuilder app )
        {
            app.Services
                .UseMaterialProviders()
                .UseFontAwesomeIcons();

            app.AddComponent<App>( "app" );
        }
    }
}
