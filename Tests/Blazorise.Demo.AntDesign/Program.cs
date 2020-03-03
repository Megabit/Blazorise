using System.Threading.Tasks;
using Blazorise.AntDesign;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Blazor.Hosting;

namespace Blazorise.Demo.AntDesign
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
                .AddAntDesignProviders()
                .AddFontAwesomeIcons();

            builder.RootComponents.Add<App>( "app" );

            var host = builder.Build();

            host.Services
                .UseAntDesignProviders()
                .UseFontAwesomeIcons();

            await host.RunAsync();
        }
    }
}
