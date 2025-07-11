#region Using directives
using Blazored.LocalStorage;
using Blazorise.Captcha.ReCaptcha;
using Blazorise.Components;
using Blazorise.FluentValidation;
using Blazorise.LoadingIndicator;
using Blazorise.RichTextEdit;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Demo;

public static class Config
{
    public static IServiceCollection SetupDemoServices( this IServiceCollection services, string licenseKey, string reCaptchaSiteKey )
    {
        services
            .AddBlazorise( options =>
            {
                options.ProductToken = licenseKey;
                options.Immediate = true;
            } )
            .AddBlazoriseRichTextEdit( options =>
            {
                options.UseBubbleTheme = true;
                options.UseShowTheme = true;
                options.UseTables = true;
                options.UseResize = true;
            } )
            .AddLoadingIndicator()
            .AddBlazoriseFluentValidation()
            .AddBlazoriseGoogleReCaptcha( options =>
            {
                options.SiteKey = reCaptchaSiteKey;
            } )
            .AddBlazoriseRouterTabs();
        
        services.AddBlazoredLocalStorage();

        services.AddValidatorsFromAssembly( typeof( App ).Assembly );

        services.AddMemoryCache();

        // register demo services to fetch test data
        services.AddScoped<Shared.Data.EmployeeData>();
        services.AddScoped<Shared.Data.CountryData>();

        return services;
    }
}