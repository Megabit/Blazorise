using Blazorise.RichTextEdit;
using Blazorise.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.Demo
{
    public static class AppConfiguration
    {
        public static IServiceCollection SetupDemoServices( this IServiceCollection services )
        {
            services
                .AddBlazorise( options =>
                {
                    options.ChangeTextOnKeyPress = true;
                } )
                .AddBlazoriseRichTextEdit( options =>
                {
                    options.UseBubbleTheme = true;
                    options.UseShowTheme = true;
                } );
            services.AddMemoryCache();
            services.AddHttpClient();
            services.AddScoped<Blazorise.Shared.Data.EmployeeData>();
            services.AddScoped< Blazorise.Shared.Data.CountryData >();
            return services;
        }
    }
}
