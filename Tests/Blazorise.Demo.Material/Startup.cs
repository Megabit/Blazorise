using Blazorise.Material;
using Blazorise.Icons.Material;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Builder;

namespace Blazorise.Demo.Material
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
                .AddMaterialProviders()
                .AddMaterialIcons();
        }

        public void Configure( IComponentsApplicationBuilder app )
        {
            app
                .UseMaterialProviders()
                .UseMaterialIcons();

            app.AddComponent<App>( "app" );
        }
    }
}
