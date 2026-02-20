using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.LoadingIndicator;

/// <summary>
/// Service registration helpers for the LoadingIndicator extension.
/// </summary>
public static class Config
{
    /// <summary>
    /// Registers the LoadingIndicator service in the DI container.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the service to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLoadingIndicator( this IServiceCollection serviceCollection )
    {
        return serviceCollection.AddScoped<ILoadingIndicatorService, LoadingIndicatorService>();
    }
}