#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Modules;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Tailwind;

public static class Config
{
    /// <summary>
    /// Adds a bootstrap providers and component mappings.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddTailwindProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
    {
        var classProvider = new TailwindClassProvider();

        configureClassProvider?.Invoke( classProvider );

        serviceCollection.AddSingleton<IClassProvider>( classProvider );
        serviceCollection.AddSingleton<IStyleProvider, TailwindStyleProvider>();
        serviceCollection.AddSingleton<IBehaviourProvider, TailwindBehaviourProvider>();
        serviceCollection.AddScoped<IThemeGenerator, TailwindThemeGenerator>();

        serviceCollection.AddTailwindComponents();

        serviceCollection.AddScoped<IJSModalModule, Modules.TailwindJSModalModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.TailwindJSTooltipModule>();

        return serviceCollection;
    }

    public static IServiceCollection AddTailwindComponents( this IServiceCollection serviceCollection )
    {
        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>
    {
        { typeof( Blazorise.CardImage ), typeof( Tailwind.CardImage ) },
        { typeof( Blazorise.CardText ), typeof( Tailwind.CardText ) },
        { typeof( Blazorise.Check<> ), typeof( Tailwind.Check<> ) },
        { typeof( Blazorise.CloseButton ), typeof( Tailwind.CloseButton ) },
        { typeof( Blazorise.Modal ), typeof( Tailwind.Modal ) },
        { typeof( Blazorise.ModalContent ), typeof( Tailwind.ModalContent ) },
        { typeof( Blazorise.Radio<> ), typeof( Tailwind.Radio<> ) },
        { typeof( Blazorise.RadioGroup<> ), typeof( Tailwind.RadioGroup<> ) },
        { typeof( Blazorise.Switch<> ), typeof( Tailwind.Switch<> ) },
    };
}