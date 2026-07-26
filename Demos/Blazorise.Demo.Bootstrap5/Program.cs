#region Using directives
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazorise.Bootstrap5;
using Blazorise.Icons.Bootstrap;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
#endregion

namespace Blazorise.Demo.Bootstrap5;

public class Program
{
    public static async Task Main( string[] args )
    {
        var builder = WebAssemblyHostBuilder.CreateDefault( args );

        using HttpClient httpClient = new()
        {
            BaseAddress = new Uri( builder.HostEnvironment.BaseAddress )
        };

        byte[] demoReportFontData = await httpClient.GetByteArrayAsync( "_content/Blazorise.Demo/fonts/OpenSans-Regular.ttf" );

        builder.Services
            .SetupDemoServices( builder.Configuration["Licensing:ProductToken"], builder.Configuration["ReCaptchaSiteKey"], demoReportFontData )
            .AddBootstrap5Providers()
            .AddBootstrapIcons();

        builder.RootComponents.Add<WasmApp>( "#app" );
        var host = builder.Build();

        await host.RunAsync();
    }
}