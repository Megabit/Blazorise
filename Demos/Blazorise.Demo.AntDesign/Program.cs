#region Using directives
using System.Threading.Tasks;
using Blazorise.AntDesign;
using Blazorise.Icons.AntDesign;
using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Demo.AntDesign;

public class Program
{
    public static async Task Main( string[] args )
    {
        var builder = WebAssemblyHostBuilder.CreateDefault( args );

        builder.Services
            .SetupDemoServices( builder.Configuration["Licensing:ProductToken"], builder.Configuration["ReCaptchaSiteKey"] )
            .AddAntDesignProviders()
            .AddAntDesignIcons();

        builder.RootComponents.Add<App>( "#app" );
        var host = builder.Build();

        await host.RunAsync();
    }
}