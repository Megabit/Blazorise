using Blazorise.Material;
using Blazorise.Icons.Material;
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
                .AddMaterialIcons();
        }

        public void Configure( IBlazorApplicationBuilder app )
        {
            app.Services
                .UseMaterialProviders()
                .UseMaterialIcons();

            app.AddComponent<App>( "app" );
        }
    }
}
