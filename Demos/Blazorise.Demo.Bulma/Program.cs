﻿#region Using directives
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazorise.Bulma;
using Blazorise.Icons.FontAwesome;
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
                .AddBulmaProviders()
                .AddFontAwesomeIcons();

            builder.Services.AddSingleton( new HttpClient
            {
                BaseAddress = new Uri( builder.HostEnvironment.BaseAddress )
            } );

            builder.RootComponents.Add<App>( "app" );

            var host = builder.Build();

            host.Services
                .UseBulmaProviders()
                .UseFontAwesomeIcons();

            await host.RunAsync();
        }
    }
}