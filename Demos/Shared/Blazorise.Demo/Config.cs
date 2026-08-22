#region Using directives
using Blazorise.Captcha.ReCaptcha;
using Blazorise.CodeEditor;
using Blazorise.Components;
using Blazorise.Demo.Setup;
using Blazorise.FluentValidation;
using Blazorise.LoadingIndicator;
using Blazorise.Pdf;
using Blazorise.Reporting;
using Blazorise.RichTextEdit;
using Blazorise.Shared.Models;
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
            .AddBlazoriseCodeEditor()
            .AddLoadingIndicator()
            .AddBlazoriseFluentValidation()
            .AddBlazoriseReporting()
            .AddBlazorisePdfHttpResources()
            .AddBlazoriseGoogleReCaptcha( options =>
            {
                options.SiteKey = reCaptchaSiteKey;
            } )
            .AddBlazoriseRouterTabs();

        services.AddScoped<PersonValidator>();
        services.AddScoped<IValidator<Person>>( serviceProvider => serviceProvider.GetRequiredService<PersonValidator>() );

        // register demo services to fetch test data
        services.AddScoped<Shared.Data.EmployeeData>();
        services.AddScoped<Shared.Data.CountryData>();

        return services;
    }
}