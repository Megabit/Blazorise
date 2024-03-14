#region Using directives
using System.Threading.Tasks;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
#endregion

namespace Blazorise.Demo.Bootstrap5;

public class Program
{
    public static async Task Main( string[] args )
    {
        var builder = WebAssemblyHostBuilder.CreateDefault( args );

        builder.Services
            .SetupDemoServices( builder.Configuration["Licensing:ProductToken"], builder.Configuration["ReCaptchaSiteKey"] )
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();

        builder.RootComponents.Add<App>( "#app" );
        var host = builder.Build();

        await host.RunAsync();
    }
}