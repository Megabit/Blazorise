#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Bootstrap.Providers;
using Blazorise.Modules;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Bootstrap;

public static class Config
{
    /// <summary>
    /// Adds a bootstrap providers and component mappings.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddBootstrapProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
    {
        var classProvider = new BootstrapClassProvider();

        configureClassProvider?.Invoke( classProvider );

        serviceCollection.AddSingleton<IClassProvider>( classProvider );
        serviceCollection.AddSingleton<IStyleProvider, BootstrapStyleProvider>();
        serviceCollection.AddSingleton<IBehaviourProvider, BootstrapBehaviourProvider>();
        serviceCollection.AddScoped<IThemeGenerator, BootstrapThemeGenerator>();

        serviceCollection.AddBootstrapComponents();

        serviceCollection.AddScoped<IJSModalModule, Modules.BootstrapJSModalModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.BootstrapJSTooltipModule>();

        Enumeration<Background>.SetNameBuilder( new BootstrapEnumerationNameBuilder<Background>() );
        Enumeration<TextColor>.SetNameBuilder( new BootstrapEnumerationNameBuilder<TextColor>() );

        return serviceCollection;
    }

    public static IServiceCollection AddBootstrapComponents( this IServiceCollection serviceCollection )
    {
        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>
    {
        { typeof( Blazorise.Addon ), typeof( Components.Addon ) },
        { typeof( Blazorise.BarToggler ), typeof( Components.BarToggler ) },
        { typeof( Blazorise.BarDropdown ), typeof( Components.BarDropdown ) },
        { typeof( Blazorise.BarDropdownMenu ), typeof( Components.BarDropdownMenu ) },
        { typeof( Blazorise.CardTitle ), typeof( Components.CardTitle ) },
        { typeof( Blazorise.CardSubtitle ), typeof( Components.CardSubtitle ) },
        { typeof( Blazorise.Carousel ), typeof( Components.Carousel ) },
        { typeof( Blazorise.CloseButton ), typeof( Components.CloseButton ) },
        { typeof( Blazorise.Check<> ), typeof( Components.Check<> ) },
        { typeof( Blazorise.DropdownToggle ), typeof( Components.DropdownToggle ) },
        { typeof( Blazorise.Field ), typeof( Components.Field ) },
        { typeof( Blazorise.FieldBody ), typeof( Components.FieldBody ) },
        { typeof( Blazorise.FileInput ), typeof( Components.FileInput ) },
        { typeof( Blazorise.Modal ), typeof( Components.Modal ) },
        { typeof( Blazorise.ModalContent ), typeof( Components.ModalContent) },
        { typeof( Blazorise.NumericPicker<> ), typeof( Components.NumericPicker<> ) },
        { typeof( Blazorise.Button ), typeof( Components.Button ) },
        { typeof( Blazorise.Radio<> ), typeof( Components.Radio<> ) },
        { typeof( Blazorise.Switch<> ), typeof( Components.Switch<> ) },
        { typeof( Blazorise.Step ), typeof( Components.Step ) },
    };
}