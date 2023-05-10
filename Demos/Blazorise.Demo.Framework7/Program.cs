#region Using directives
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazorise.Framework7;
using Blazorise.Icons.FontAwesome;
using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Demo.Framework7;

public class Program
{
    public static async Task Main( string[] args )
    {
        var builder = WebAssemblyHostBuilder.CreateDefault( args );

        builder.Services
            .SetupDemoServices( builder.Configuration["Licensing:ProductToken"] )
            .AddFramework7Providers()
            .AddFontAwesomeIcons();

        builder.RootComponents.Add<App>( "#app" );
        builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );

        await builder.Build().RunAsync();
    }
}