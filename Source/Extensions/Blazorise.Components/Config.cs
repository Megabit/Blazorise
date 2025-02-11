#region Using directives
using System;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Components;

/// <summary>
/// Provides extension methods for configuring Blazorise-related services.
/// </summary>
public static class Config
{
    /// <summary>
    /// Adds the Blazorise Router Tabs services to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to which the Router Tabs services will be added.</param>
    /// <param name="options">An optional configuration action to customize the <see cref="RouterTabsOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddBlazoriseRouterTabs( this IServiceCollection serviceCollection, Action<RouterTabsOptions> options = null )
    {
        serviceCollection.AddTransient<RouterTabsService>();

        var routerTabsOptions = new RouterTabsOptions();
        options?.Invoke( routerTabsOptions );
        serviceCollection.AddSingleton( _ => routerTabsOptions );

        return serviceCollection;
    }
}