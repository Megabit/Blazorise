using Blazorise.RichTextEdit;
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
            services.AddScoped<Shared.Data.EmployeeData>();
            services.AddScoped<Shared.Data.CountryData >();
            return services;
        }
    }
}
