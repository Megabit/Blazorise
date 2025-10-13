#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Modules;
using Blazorise.Tailwind.Providers;
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

        Enumeration<Background>.SetNameBuilder( new TailwindEnumerationNameBuilder<Background>() );
        Enumeration<TextColor>.SetNameBuilder( new TailwindEnumerationNameBuilder<TextColor>() );

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
        { typeof( Blazorise.Accordion ), typeof( Components.Accordion ) },
        { typeof( Blazorise.AccordionToggle ), typeof( Components.AccordionToggle ) },
        { typeof( Blazorise.Addon ), typeof( Components.Addon ) },
        { typeof( Blazorise.Badge ), typeof( Components.Badge ) },
        { typeof( Blazorise.Bar ), typeof( Components.Bar ) },
        { typeof( Blazorise.BarDropdown ), typeof( Components.BarDropdown ) },
        { typeof( Blazorise.BarDropdownItem ), typeof( Components.BarDropdownItem ) },
        { typeof( Blazorise.BarDropdownMenu ), typeof( Components.BarDropdownMenu ) },
        { typeof( Blazorise.BarDropdownToggle ), typeof( Components.BarDropdownToggle ) },
        { typeof( Blazorise.BarEnd ), typeof( Components.BarEnd ) },
        { typeof( Blazorise.BarItem ), typeof( Components.BarItem ) },
        { typeof( Blazorise.BarLink ), typeof( Components.BarLink ) },
        { typeof( Blazorise.BarMenu ), typeof( Components.BarMenu ) },
        { typeof( Blazorise.BarStart ), typeof( Components.BarStart ) },
        { typeof( Blazorise.BarToggler ), typeof( Components.BarToggler ) },
        { typeof( Blazorise.Breadcrumb ), typeof( Components.Breadcrumb ) },
        { typeof( Blazorise.BreadcrumbItem ), typeof( Components.BreadcrumbItem ) },
        { typeof( Blazorise.BreadcrumbLink ), typeof( Components.BreadcrumbLink ) },
        { typeof( Blazorise.CardImage ), typeof( Components.CardImage ) },
        { typeof( Blazorise.CardText ), typeof( Components.CardText ) },
        { typeof( Blazorise.Carousel ), typeof( Components.Carousel ) },
        { typeof( Blazorise.CarouselSlide ), typeof( Components.CarouselSlide ) },
        { typeof( Blazorise.Check<> ), typeof( Components.Check<> ) },
        { typeof( Blazorise.CloseButton ), typeof( Components.CloseButton ) },
        { typeof( Blazorise.ColorPicker ), typeof( Components.ColorPicker ) },
        { typeof( Blazorise.Dropdown ), typeof( Components.Dropdown ) },
        { typeof( Blazorise.DropdownHeader ), typeof( Components.DropdownHeader ) },
        { typeof( Blazorise.DropdownItem ), typeof( Components.DropdownItem ) },
        { typeof( Blazorise.DropdownMenu ), typeof( Components.DropdownMenu ) },
        { typeof( Blazorise.DropdownToggle ), typeof( Components.DropdownToggle ) },
        { typeof( Blazorise.Button ), typeof( Components.Button ) },
        { typeof( Blazorise.Field ), typeof( Components.Field ) },
        { typeof( Blazorise.Modal ), typeof( Components.Modal ) },
        { typeof( Blazorise.ModalContent ), typeof( Components.ModalContent ) },
        { typeof( Blazorise.Offcanvas ), typeof( Components.Offcanvas ) },
        { typeof( Blazorise.Progress ), typeof( Components.Progress ) },
        { typeof( Blazorise.ProgressBar ), typeof( Components.ProgressBar ) },
        { typeof( Blazorise.Radio<> ), typeof( Components.Radio<> ) },
        { typeof( Blazorise.RadioGroup<> ), typeof( Components.RadioGroup<> ) },
        { typeof( Blazorise.Step ), typeof( Components.Step ) },
        { typeof( Blazorise.Switch<> ), typeof( Components.Switch<> ) },
        { typeof( Blazorise.Toast ), typeof( Components.Toast ) },
    };
}