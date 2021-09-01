﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.Bulma;
using Blazorise.Demo.Data;
using Blazorise.Icons.FontAwesome;
using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Demo.Bulma
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
                .AddBlazoriseRichTextEdit( options =>
                {
                    options.UseBubbleTheme = true;
                    options.UseShowTheme = true;
                } )
                .AddBulmaProviders()
                .AddFontAwesomeIcons();

            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<EmployeeData>();

            builder.RootComponents.Add<App>( "#app" );

            var host = builder.Build();

            await host.RunAsync();
        }
    }
}