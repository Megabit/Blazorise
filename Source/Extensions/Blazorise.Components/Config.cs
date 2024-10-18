#region Using directives
using Blazorise.Components;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise;

/// <summary>
/// Extension methods for building the blazorise options.
/// </summary>
public static class Config
{
    /// <summary>
    /// Adds the Blazorise Router Tabs services to the service collection.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddBlazoriseRouterTabs( this IServiceCollection serviceCollection )
    {
        serviceCollection.AddTransient<RouterTabsService>();
        return serviceCollection;
    }
}