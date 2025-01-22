#region Using directives

using System;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Components;

/// <summary>
/// Extension methods for building the blazorise options.
/// </summary>
public static class Config
{
    /// <summary>
    /// Adds the Blazorise Router Tabs services to the service collection.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddBlazoriseRouterTabs( this IServiceCollection serviceCollection, Action<RouterTabsOptions> options = null )
    {
        serviceCollection.AddTransient<RouterTabsService>();
        var opt = new RouterTabsOptions();
        options?.Invoke( opt );
        serviceCollection.AddSingleton( _  => opt );

        return serviceCollection;
    }
}