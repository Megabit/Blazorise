#region Using directives
using Blazored.LocalStorage;
using Blazorise.Captcha.ReCaptcha;
using Blazorise.Components;
using Blazorise.FluentValidation;
using Blazorise.LoadingIndicator;
using Blazorise.Reporting;
using Blazorise.Reporting.DataSources.Csv;
using Blazorise.RichTextEdit;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Demo;

public static class Config
{
    public static IServiceCollection SetupDemoServices( this IServiceCollection services, string licenseKey, string reCaptchaSiteKey, byte[] demoReportFontData = null )
    {
        services
            .AddBlazorise( options =>
            {
                options.ProductToken = licenseKey;
                options.Immediate = true;
                options.AccessibilityOptions.OnScreenKeyboard.Enabled = false;
                options.AccessibilityOptions.OnScreenKeyboard.ShowSpecialCharactersKey = true;
                options.Fonts.Add( new()
                {
                    Name = "Georgia",
                    DisplayName = "Georgia (Demo)",
                    CssFamily = "Georgia, serif",
                } );
                options.Fonts.Add( new()
                {
                    Name = "Open Sans Demo",
                    DisplayName = "Open Sans (Demo)",
                    CssFamily = "\"Open Sans Demo\", \"Open Sans\", sans-serif",
                    Regular = new()
                    {
                        Data = demoReportFontData,
                        Url = "_content/Blazorise.Demo/fonts/OpenSans-Regular.ttf",
                        Format = FontFormat.TrueType,
                    },
                } );
            } )
            .AddBlazoriseRichTextEdit( options =>
            {
                options.UseBubbleTheme = true;
                options.UseSnowTheme = true;
                options.UseTables = true;
                options.UseResize = true;
            } )
            .AddLoadingIndicator()
            .AddBlazoriseFluentValidation()
            .AddBlazoriseReporting()
            .AddBlazoriseReportingCsvDataSource()
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