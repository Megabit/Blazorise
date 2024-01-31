using System.Threading.Tasks;
using Blazorise.Bootstrap5;
using Blazorise.FluentValidation;
using Blazorise.Icons.FontAwesome;
using Blazorise.RichTextEdit;
using FluentValidation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.Docs;

public class Program
{
    public static async Task Main( string[] args )
    {
        var builder = WebAssemblyHostBuilder.CreateDefault( args );

        builder.Services
            .AddBlazorise( options =>
            {
                options.ProductToken = "";
                options.Immediate = true; // optional
            } )
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons()
            .AddBlazoriseRichTextEdit()
            .AddBlazoriseFluentValidation();

        builder.Services.AddValidatorsFromAssembly( typeof( Program ).Assembly );

        //The WASM SDK does not seem to have this. Need to figure out a way...
        //builder.Services.AddMemoryCache();
        builder.Services.AddScoped<Shared.Data.EmployeeData>();
        builder.Services.AddScoped<Shared.Data.CountryData>();
        builder.Services.AddScoped<Shared.Data.PageEntryData>();

        await builder.Build().RunAsync();
    }
}