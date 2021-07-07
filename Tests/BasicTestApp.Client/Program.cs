#region Using directives
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
#endregion

namespace BasicTestApp.Client
{
    [ExcludeFromCodeCoverage]
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
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            builder.RootComponents.Add<Index>( "root" );

            var host = builder.Build();

            await host.RunAsync();
        }
    }
}