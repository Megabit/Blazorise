using Microsoft.Extensions.DependencyInjection;

namespace Blazorise.LoadingIndicator;

public static class Config
{
    /// <summary>
    /// Register LoadingIndicator service.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddLoadingIndicator( this IServiceCollection serviceCollection )
    {
        return serviceCollection.AddScoped<ILoadingIndicatorService, LoadingIndicatorService>();
    }
}