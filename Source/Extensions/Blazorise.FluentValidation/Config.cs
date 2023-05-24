#region Using directives
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.FluentValidation;

public static class Config
{
    /// <summary>
    /// Adds an implementation of the FluentValidationHandler implementation of the IValidationHandler interface
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBlazoriseFluentValidation( this IServiceCollection services )
    {
        services.AddScoped<FluentValidationHandler>();

        return services;
    }
}