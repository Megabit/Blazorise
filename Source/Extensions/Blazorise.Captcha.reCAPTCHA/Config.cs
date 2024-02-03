#region Using directives
using System;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Captcha.reCAPTCHA;

/// <summary>
/// Extension methods for building the blazorise reCAPTCHA options.
/// </summary>
public static class Config
{

    public static IServiceCollection AddGoogleReCAPTCHA( this IServiceCollection serviceCollection, Action<ReCAPTCHAOptions> configureOptions = null )
    {
        var options = new ReCAPTCHAOptions();
        configureOptions?.Invoke( options );

        serviceCollection.AddScoped( sp => options );
        return serviceCollection;
    }
}