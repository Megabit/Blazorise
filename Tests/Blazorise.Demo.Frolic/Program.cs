#region Using directives
using System.Threading.Tasks;
using Blazorise.Frolic;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Demo.Frolic
{
    public class Program
    {
        public static async Task Main( string[] args )
        {
            var builder = WebAssemblyHostBuilder.CreateDefault( args );

            builder.Services
                .AddBlazorise( options =>
                {
                    options.ChangeTextOnKeyPress = true;
                } )
                .AddFrolicProviders()
                .AddFontAwesomeIcons();

            builder.Services.AddBaseAddressHttpClient();

            builder.RootComponents.Add<App>( "app" );

            var host = builder.Build();

            host.Services
                .UseFrolicProviders()
                .UseFontAwesomeIcons();

            await host.RunAsync();
        }
    }
}