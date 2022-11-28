#region Using directives
using System;
using System.Collections.Generic;
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
        { typeof( Blazorise.Addon ), typeof( Bootstrap.Addon ) },
        { typeof( Blazorise.BarToggler ), typeof( Bootstrap.BarToggler ) },
        { typeof( Blazorise.BarDropdown ), typeof( Bootstrap.BarDropdown ) },
        { typeof( Blazorise.BarDropdownMenu ), typeof( Bootstrap.BarDropdownMenu ) },
        { typeof( Blazorise.CardTitle ), typeof( Bootstrap.CardTitle ) },
        { typeof( Blazorise.CardSubtitle ), typeof( Bootstrap.CardSubtitle ) },
        { typeof( Blazorise.Carousel ), typeof( Bootstrap.Carousel ) },
        { typeof( Blazorise.CloseButton ), typeof( Bootstrap.CloseButton ) },
        { typeof( Blazorise.Check<> ), typeof( Bootstrap.Check<> ) },
        { typeof( Blazorise.DropdownToggle ), typeof( Bootstrap.DropdownToggle ) },
        { typeof( Blazorise.Field ), typeof( Bootstrap.Field ) },
        { typeof( Blazorise.FieldBody ), typeof( Bootstrap.FieldBody ) },
        { typeof( Blazorise.FileEdit ), typeof( Bootstrap.FileEdit ) },
        { typeof( Blazorise.Modal ), typeof( Bootstrap.Modal ) },
        { typeof( Blazorise.ModalContent ), typeof( Bootstrap.ModalContent) },
        { typeof( Blazorise.NumericPicker<> ), typeof( Bootstrap.NumericPicker<> ) },
        { typeof( Blazorise.Button ), typeof( Bootstrap.Button ) },
        { typeof( Blazorise.Radio<> ), typeof( Bootstrap.Radio<> ) },
        { typeof( Blazorise.Switch<> ), typeof( Bootstrap.Switch<> ) },
        { typeof( Blazorise.Step ), typeof( Bootstrap.Step ) },
    };
}