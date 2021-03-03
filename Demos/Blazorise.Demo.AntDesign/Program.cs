#region Using directives
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazorise.AntDesign;
using Blazorise.Icons.FontAwesome;
using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
#endregion

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
                .AddBlazoriseRichTextEdit( options =>
                {
                    options.UseBubbleTheme = true;
                    options.UseShowTheme = true;
                } )
                .AddAntDesignProviders()
                .AddFontAwesomeIcons();

            builder.Services.AddSingleton( new HttpClient
            {
                BaseAddress = new Uri( builder.HostEnvironment.BaseAddress )
            } );

            builder.RootComponents.Add<App>( "#app" );

            var host = builder.Build();

            await host.RunAsync();
        }
    }
}
