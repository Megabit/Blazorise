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
    public static IServiceCollection AddBlazoriseGoogleReCaptcha( this IServiceCollection serviceCollection, Action<ReCaptchaOptions> configureOptions )
    {
        var options = new ReCaptchaOptions();
        configureOptions?.Invoke( options );

        serviceCollection.AddScoped( sp => options );
        serviceCollection.AddTransient( typeof( Blazorise.Captcha.Captcha ), typeof( Blazorise.Captcha.ReCaptcha.ReCaptcha ) );
       
        return serviceCollection;
    }
}