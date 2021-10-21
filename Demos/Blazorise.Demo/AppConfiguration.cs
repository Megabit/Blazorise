using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            services.AddHttpClient();
            services.AddScoped<Data.EmployeeData>();
            services.AddScoped<Data.CountryData>();
            return services;
        }
    }
}
