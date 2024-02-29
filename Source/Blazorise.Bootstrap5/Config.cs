#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Modules;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Bootstrap5;

public static class Config
{
    /// <summary>
    /// Adds a bootstrap providers and component mappings.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddBootstrap5Providers( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
    {
        var classProvider = new Bootstrap5ClassProvider();

        configureClassProvider?.Invoke( classProvider );

        serviceCollection.AddSingleton<IClassProvider>( classProvider );
        serviceCollection.AddSingleton<IStyleProvider, Bootstrap5StyleProvider>();
        serviceCollection.AddSingleton<IBehaviourProvider, Bootstrap5BehaviourProvider>();
        serviceCollection.AddScoped<IThemeGenerator, Bootstrap5ThemeGenerator>();

        serviceCollection.AddBootstrap5Components();

        serviceCollection.AddScoped<IJSModalModule, Modules.BootstrapJSModalModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.BootstrapJSTooltipModule>();

        return serviceCollection;
    }

    public static IServiceCollection AddBootstrap5Components( this IServiceCollection serviceCollection )
    {
        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>
    {
        { typeof( Blazorise.AccordionToggle ), typeof( Bootstrap5.AccordionToggle ) },
        { typeof( Blazorise.Addon ), typeof( Bootstrap5.Addon ) },
        { typeof( Blazorise.BarToggler ), typeof( Bootstrap5.BarToggler ) },
        { typeof( Blazorise.BarDropdown ), typeof( Bootstrap5.BarDropdown ) },
        { typeof( Blazorise.BarDropdownMenu ), typeof( Bootstrap5.BarDropdownMenu ) },
        { typeof( Blazorise.BarDropdownToggle ), typeof( Bootstrap5.BarDropdownToggle ) },
        { typeof( Blazorise.Button ), typeof( Bootstrap5.Button ) },
        { typeof( Blazorise.Card ), typeof( Bootstrap5.Card ) },
        { typeof( Blazorise.CardTitle ), typeof( Bootstrap5.CardTitle ) },
        { typeof( Blazorise.CardSubtitle ), typeof( Bootstrap5.CardSubtitle ) },
        { typeof( Blazorise.Carousel ), typeof( Bootstrap5.Carousel ) },
        { typeof( Blazorise.CloseButton ), typeof( Bootstrap5.CloseButton ) },
        { typeof( Blazorise.Check<> ), typeof( Bootstrap5.Check<> ) },
        { typeof( Blazorise.DropdownToggle ), typeof( Bootstrap5.DropdownToggle ) },
        { typeof( Blazorise.Field ), typeof( Bootstrap5.Field ) },
        { typeof( Blazorise.FieldBody ), typeof( Bootstrap5.FieldBody ) },
        { typeof( Blazorise.FileEdit ), typeof( Bootstrap5.FileEdit ) },
        { typeof( Blazorise.Modal ), typeof( Bootstrap5.Modal ) },
        { typeof( Blazorise.ModalContent ), typeof( Bootstrap5.ModalContent) },
        { typeof( Blazorise.NumericPicker<> ), typeof( Bootstrap5.NumericPicker<> ) },
        { typeof( Blazorise.Radio<> ), typeof( Bootstrap5.Radio<> ) },
        { typeof( Blazorise.Switch<> ), typeof( Bootstrap5.Switch<> ) },
        { typeof( Blazorise.Step ), typeof( Bootstrap5.Step ) },
        { typeof( Blazorise.Toast ), typeof( Bootstrap5.Toast ) },
    };
}