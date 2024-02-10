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
                //options.SiteKey = "6LdF_GopAAAAAAbxAmHmnGQKDZF5MDjZk76_5SJU"; //checkbox
                //options.SiteKey = "6LfqW20pAAAAAC9xJUNgc4z5D3OkR6MGI_d1s5PH"; //invisible
                options.SiteKey = "6LeALm4pAAAAAFl2GqcmFGxbJ0wUHKbk5mIZ3RX_"; //v3
                options.Size = ReCaptchaSize.Invisible;
                options.Theme = ReCaptchaTheme.Dark;
                options.LanguageCode = "pt-PT";
                options.Badge = ReCaptchaBadgePosition.BottomEnd;
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