#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Material.Providers;
using Blazorise.Modules;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Material;

public static class Config
{
    public static IServiceCollection AddMaterialProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
    {
        var classProvider = new MaterialClassProvider();

        configureClassProvider?.Invoke( classProvider );

        serviceCollection.AddSingleton<IClassProvider>( classProvider );
        serviceCollection.AddSingleton<IStyleProvider, MaterialStyleProvider>();
        serviceCollection.AddSingleton<IBehaviourProvider, MaterialBehaviourProvider>();
        serviceCollection.AddScoped<IThemeGenerator, MaterialThemeGenerator>();

        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        serviceCollection.AddScoped<IJSModalModule, Modules.MaterialJSModalModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.MaterialJSTooltipModule>();

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>( Bootstrap.Config.ComponentMap )
    {
        // material overrides
        [typeof( Blazorise.Badge )] = typeof( Components.Badge )
    };
}