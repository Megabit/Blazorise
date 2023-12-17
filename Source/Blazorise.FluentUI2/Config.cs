#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Modules;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.FluentUI2;

public static class Config
{
    public static IServiceCollection AddFluentUI2Providers( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
    {
        var classProvider = new FluentUI2ClassProvider();

        configureClassProvider?.Invoke( classProvider );

        serviceCollection.AddSingleton<IClassProvider>( classProvider );
        serviceCollection.AddSingleton<IStyleProvider, FluentUI2StyleProvider>();
        serviceCollection.AddSingleton<IBehaviourProvider, FluentUI2BehaviourProvider>();
        serviceCollection.AddSingleton<IThemeGenerator, FluentUI2ThemeGenerator>();

        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        serviceCollection.AddScoped<IJSModalModule, Modules.FluentUI2JSModalModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.FluentUI2JSTooltipModule>();

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>
    {
        { typeof( Blazorise.CardHeader ), typeof( FluentUI2.CardHeader ) },
        { typeof( Blazorise.TextEdit ), typeof( FluentUI2.TextEdit ) },
    };
}