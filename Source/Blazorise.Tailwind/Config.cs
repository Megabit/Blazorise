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
        { typeof( Blazorise.Accordion ), typeof( Tailwind.Accordion ) },
        { typeof( Blazorise.AccordionToggle ), typeof( Tailwind.AccordionToggle ) },
        { typeof( Blazorise.Addon ), typeof( Tailwind.Addon ) },
        { typeof( Blazorise.Badge ), typeof( Tailwind.Badge ) },
        { typeof( Blazorise.Bar ), typeof( Tailwind.Bar ) },
        { typeof( Blazorise.BarDropdown ), typeof( Tailwind.BarDropdown ) },
        { typeof( Blazorise.BarDropdownItem ), typeof( Tailwind.BarDropdownItem ) },
        { typeof( Blazorise.BarDropdownMenu ), typeof( Tailwind.BarDropdownMenu ) },
        { typeof( Blazorise.BarDropdownToggle ), typeof( Tailwind.BarDropdownToggle ) },
        { typeof( Blazorise.BarEnd ), typeof( Tailwind.BarEnd ) },
        { typeof( Blazorise.BarItem ), typeof( Tailwind.BarItem ) },
        { typeof( Blazorise.BarLink ), typeof( Tailwind.BarLink ) },
        { typeof( Blazorise.BarMenu ), typeof( Tailwind.BarMenu ) },
        { typeof( Blazorise.BarStart ), typeof( Tailwind.BarStart ) },
        { typeof( Blazorise.Breadcrumb ), typeof( Tailwind.Breadcrumb ) },
        { typeof( Blazorise.BreadcrumbItem ), typeof( Tailwind.BreadcrumbItem ) },
        { typeof( Blazorise.BreadcrumbLink ), typeof( Tailwind.BreadcrumbLink ) },
        { typeof( Blazorise.CardImage ), typeof( Tailwind.CardImage ) },
        { typeof( Blazorise.CardText ), typeof( Tailwind.CardText ) },
        { typeof( Blazorise.Carousel ), typeof( Tailwind.Carousel ) },
        { typeof( Blazorise.CarouselSlide ), typeof( Tailwind.CarouselSlide ) },
        { typeof( Blazorise.Check<> ), typeof( Tailwind.Check<> ) },
        { typeof( Blazorise.CloseButton ), typeof( Tailwind.CloseButton ) },
        { typeof( Blazorise.ColorPicker ), typeof( Tailwind.ColorPicker ) },
        { typeof( Blazorise.Dropdown ), typeof( Tailwind.Dropdown ) },
        { typeof( Blazorise.DropdownHeader ), typeof( Tailwind.DropdownHeader ) },
        { typeof( Blazorise.DropdownItem ), typeof( Tailwind.DropdownItem ) },
        { typeof( Blazorise.DropdownMenu ), typeof( Tailwind.DropdownMenu ) },
        { typeof( Blazorise.DropdownToggle ), typeof( Tailwind.DropdownToggle ) },
        { typeof( Blazorise.Button ), typeof( Tailwind.Button ) },
        { typeof( Blazorise.Field ), typeof( Tailwind.Field ) },
        { typeof( Blazorise.Modal ), typeof( Tailwind.Modal ) },
        { typeof( Blazorise.ModalContent ), typeof( Tailwind.ModalContent ) },
        { typeof( Blazorise.Offcanvas ), typeof( Tailwind.Offcanvas ) },
        { typeof( Blazorise.Progress ), typeof( Tailwind.Progress ) },
        { typeof( Blazorise.ProgressBar ), typeof( Tailwind.ProgressBar ) },
        { typeof( Blazorise.Radio<> ), typeof( Tailwind.Radio<> ) },
        { typeof( Blazorise.RadioGroup<> ), typeof( Tailwind.RadioGroup<> ) },
        { typeof( Blazorise.Step ), typeof( Tailwind.Step ) },
        { typeof( Blazorise.Switch<> ), typeof( Tailwind.Switch<> ) },
    };
}