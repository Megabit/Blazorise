#region Using directives
using Blazorise;
using Blazorise.AntDesign;
using Blazorise.Demo;
using Blazorise.Icons.FontAwesome;
using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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
    .AddAntDesignProviders()
    .AddFontAwesomeIcons();

builder.RootComponents.Add<App>( "#app" );

var host = builder.Build();

await host.RunAsync();