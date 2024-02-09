#region Using directives
using Blazored.LocalStorage;
using Blazorise.Captcha.ReCaptcha;
using Blazorise.FluentValidation;
using Blazorise.LoadingIndicator;
using Blazorise.RichTextEdit;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Demo;

public static class Config
{
    public static IServiceCollection SetupDemoServices( this IServiceCollection services, string licenseKey )
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
            } )
            .AddLoadingIndicator()
            .AddBlazoriseFluentValidation()
            .AddBlazoriseGoogleReCaptcha( options =>
            {
                options.SiteKey = "6LdF_GopAAAAAAbxAmHmnGQKDZF5MDjZk76_5SJU";
                options.Size = ReCaptchaSize.Compact;
                options.Theme = ReCaptchaTheme.Dark;
                options.LanguageCode = "pt-PT";
            } );

        services.AddBlazoredLocalStorage();

        services.AddValidatorsFromAssembly( typeof( App ).Assembly );

        services.AddMemoryCache();

        // register demo services to fetch test data
        services.AddScoped<Shared.Data.EmployeeData>();
        services.AddScoped<Shared.Data.CountryData>();

        return services;
    }
}