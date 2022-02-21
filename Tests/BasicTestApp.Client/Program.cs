#region Using directives
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
                    options.Immediate = true;
                } )
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            builder.Services.AddScoped<Blazorise.Shared.Data.EmployeeData>();
            builder.Services.AddScoped<Blazorise.Shared.Data.CountryData>();
            builder.RootComponents.Add<Index>( "root" );

            var host = builder.Build();

            await host.RunAsync();
        }
    }
}