#region Using directives
using Blazorise.RichTextEdit;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Demo
{
    public static class Config
    {
        public static IServiceCollection SetupDemoServices( this IServiceCollection services )
        {
            services
                .AddBlazorise( options =>
                {
                    options.Immediate = true;
                } )
                .AddBlazoriseRichTextEdit( options =>
                {
                    options.UseBubbleTheme = true;
                    options.UseShowTheme = true;
                } );

            services.AddMemoryCache();

            // register demo services to fetch test data
            services.AddScoped<Shared.Data.EmployeeData>();
            services.AddScoped<Shared.Data.CountryData>();

            return services;
        }
    }
}
