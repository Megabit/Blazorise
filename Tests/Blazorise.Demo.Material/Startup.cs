using Blazorise.Material;
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
                .AddIconProvider( IconProvider.FontAwesome );
        }

        public void Configure( IBlazorApplicationBuilder app )
        {
            app.AddComponent<App>( "app" );
        }
    }
}
