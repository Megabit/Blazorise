#region Using directives
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Demo;
using Blazorise.Icons.Material;
using Blazorise.Material;
using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
#endregion

var builder = WebAssemblyHostBuilder.CreateDefault( args );

builder.Services
    .AddBlazorise( options =>
    {
        options.ChangeTextOnKeyPress = true;
    } )
    .AddBlazoriseRichTextEdit( options =>
    {
        options.UseBubbleTheme = true;
        options.UseShowTheme = true;
    } )
    .AddMaterialProviders()
    .AddMaterialIcons();

builder.Services.AddSingleton( new HttpClient
{
    BaseAddress = new Uri( builder.HostEnvironment.BaseAddress )
} );

builder.RootComponents.Add<App>( "#app" );

var host = builder.Build();

await host.RunAsync();