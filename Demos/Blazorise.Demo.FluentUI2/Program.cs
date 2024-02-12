﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.FluentUI2;
using Blazorise.Icons.FluentUI;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
#endregion

namespace Blazorise.Demo.FluentUI2;

public class Program
{
    public static async Task Main( string[] args )
    {
        var builder = WebAssemblyHostBuilder.CreateDefault( args );

        builder.Services
            .SetupDemoServices( builder.Configuration["Licensing:ProductToken"], builder.Configuration["ReCaptchaSiteKey"] )
            .AddFluentUI2Providers()
            .AddFluentUIIcons();

        builder.RootComponents.Add<App>( "#app" );
        var host = builder.Build();

        await host.RunAsync();
    }
}