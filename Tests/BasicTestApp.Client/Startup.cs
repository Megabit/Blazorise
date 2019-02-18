using System.Runtime.InteropServices;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Blazor.Http;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BasicTestApp.Client
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

            if ( RuntimeInformation.IsOSPlatform( OSPlatform.Create( "WEBASSEMBLY" ) ) )
            {
                // Needed because the test server runs on a different port than the client app,
                // and we want to test sending/receiving cookies underling this config
                WebAssemblyHttpMessageHandler.DefaultCredentials = FetchCredentialsOption.Include;
            }

            app.AddComponent<Index>( "root" );
        }
    }
}
