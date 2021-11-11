using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Blazorise.Bootstrap;
using Blazorise.Demo;
using Blazorise.Icons.FontAwesome;


var builder = WebAssemblyHostBuilder.CreateDefault( args );

builder.Services
    .SetupDemoServices()
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();

//builder.RootComponents.Add<App>( "#app" );
builder.Services.AddScoped( sp => new HttpClient { BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ) } );

await builder.Build().RunAsync();
