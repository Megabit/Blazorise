#region Using directives
using Blazorise.FluentValidation;
using Blazorise.LoadingIndicator;
using Blazorise.RichTextEdit;
using FluentValidation;
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
                    options.LicenseKey = "CjxRAnB+Nwo9UAJ3fT01BlEAc300CD5RAHF+NQg8bjoPL2hdNiFZLSIlf20XVTJDD3tjDkAgIHwCQQpTMyF2V2oXPUsLLmJcM3QTFGJydil4EGVVaT0pWnE2Ul14BV0IAGJJVBl+IndWbFglCBE9NEp2LVMIPnBAQxFkKChpWXYCezAPbFpJCFt2JC5yaClRcTZTcD4SWg4GR1A/MUYMBWtxfhdaFzlAAW8MYTkiYnBeAUEXKSpOXxsENA1MaXUHX25hPG5pWmoTBkd/PSlHcA83a0hTeAQ8fA1qVwgXOX1cTQdlaiw3TlYEZzQGfFwjSEYINn0JVVRncQBNQVZXXxAIPFBqK2cmH3R2fCRzGRo9dHYAdgIfXXM5BUIUGk1oOCkfcwBkSloAfXh/cn5DBGMxdlVTfxRybgQzbVUXegd9XQ97VAkxFGMXfVdhEDtuUl4AeScjQGlYUEIzP0FwWAFzdgJQczkPfykkX2FFB3gZfH9pNDB2C3Z/fyNWdA5/RAFkG3YMNm1oeSRCIz4ufWAFSXEdY0h9CUd8cw==";
                    options.Immediate = true;
                } )
                .AddBlazoriseRichTextEdit( options =>
                {
                    options.UseBubbleTheme = true;
                    options.UseShowTheme = true;
                } )
                .AddLoadingIndicator()
                .AddBlazoriseFluentValidation();

            services.AddValidatorsFromAssembly( typeof( App ).Assembly );

            services.AddMemoryCache();

            // register demo services to fetch test data
            services.AddScoped<Shared.Data.EmployeeData>();
            services.AddScoped<Shared.Data.CountryData>();

            return services;
        }
    }
}
