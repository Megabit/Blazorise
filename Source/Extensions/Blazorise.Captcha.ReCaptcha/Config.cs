#region Using directives
using System;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Captcha.ReCaptcha;

/// <summary>
/// Extension methods for building the blazorise ReCaptcha options.
/// </summary>
public static class Config
{
    /// <summary>
    /// Adds Google reCAPTCHA as the application's Blazorise captcha implementation.
    /// </summary>
    /// <param name="serviceCollection">Application services receiving the captcha registrations.</param>
    /// <param name="configureOptions">Optional callback for the site key and widget preferences.</param>
    /// <returns>The same service collection for fluent configuration.</returns>
    public static IServiceCollection AddBlazoriseGoogleReCaptcha( this IServiceCollection serviceCollection, Action<ReCaptchaOptions> configureOptions )
    {
        var options = new ReCaptchaOptions();
        configureOptions?.Invoke( options );

        serviceCollection.AddScoped( sp => options );
        serviceCollection.AddTransient( typeof( Blazorise.Captcha.Captcha ), typeof( Blazorise.Captcha.ReCaptcha.ReCaptcha ) );
       
        return serviceCollection;
    }
}