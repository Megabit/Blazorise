using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Blazor.Http;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.BasicTestApp.Client
{
    public class Startup
    {
        public void ConfigureServices( IServiceCollection services )
        {
        }

        public void Configure( IComponentsApplicationBuilder app )
        {
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
