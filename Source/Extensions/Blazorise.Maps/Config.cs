#region Using directives
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Maps;

/// <summary>
/// Provides extension methods for registering Blazorise Maps services.
/// </summary>
public static class Config
{
    /// <summary>
    /// Registers the Blazorise Maps extension.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddBlazoriseMaps( this IServiceCollection services )
    {
        return services;
    }
}