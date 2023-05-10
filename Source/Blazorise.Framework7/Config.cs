#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Modules;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Framework7;

public static class Config
{
    /// <summary>
    /// Adds a Framework7 providers and component mappings.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddFramework7Providers( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
    {
        var classProvider = new Framework7ClassProvider();

        configureClassProvider?.Invoke( classProvider );

        serviceCollection.AddSingleton<IClassProvider>( classProvider );
        serviceCollection.AddSingleton<IStyleProvider, Framework7StyleProvider>();
        serviceCollection.AddSingleton<IBehaviourProvider, Framework7BehaviourProvider>();
        serviceCollection.AddScoped<IThemeGenerator, Framework7ThemeGenerator>();

        serviceCollection.AddFramework7Components();

        serviceCollection.AddScoped<IJSModalModule, Modules.Framework7JSModalModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.Framework7JSTooltipModule>();

        return serviceCollection;
    }

    public static IServiceCollection AddFramework7Components( this IServiceCollection serviceCollection )
    {
        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>
    {
        
    };
}