#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.AntDesign.Providers;
using Blazorise.Modules;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.AntDesign;

public static class Config
{
    /// <summary>
    /// Adds a ant design providers and component mappings.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddAntDesignProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
    {
        var classProvider = new AntDesignClassProvider();

        configureClassProvider?.Invoke( classProvider );

        serviceCollection.AddSingleton<IClassProvider>( classProvider );
        serviceCollection.AddSingleton<IStyleProvider, AntDesignStyleProvider>();
        serviceCollection.AddSingleton<IBehaviourProvider, AntDesignBehaviourProvider>();
        serviceCollection.AddScoped<IThemeGenerator, AntDesignThemeGenerator>();

        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        serviceCollection.AddScoped<IJSModalModule, Modules.AntDesignJSModalModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.AntDesignJSTooltipModule>();

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>
    {
        { typeof( Blazorise.AccordionHeader ), typeof( Components.AccordionHeader ) },
        { typeof( Blazorise.Addon ), typeof( Components.Addon ) },
        { typeof( Blazorise.AddonLabel ), typeof( Components.AddonLabel ) },
        { typeof( Blazorise.Addons ), typeof( Components.Addons ) },
        { typeof( Blazorise.AlertDescription ), typeof( Components.AlertDescription ) },
        { typeof( Blazorise.AlertMessage ), typeof( Components.AlertMessage ) },
        { typeof( Blazorise.Badge ), typeof( Components.Badge ) },
        { typeof( Blazorise.Bar ), typeof( Components.Bar ) },
        { typeof( Blazorise.BarBrand ), typeof( Components.BarBrand ) },
        { typeof( Blazorise.BarDropdown ), typeof( Components.BarDropdown ) },
        { typeof( Blazorise.BarDropdownDivider ), typeof( Components.BarDropdownDivider ) },
        { typeof( Blazorise.BarDropdownItem ), typeof( Components.BarDropdownItem ) },
        { typeof( Blazorise.BarDropdownMenu ), typeof( Components.BarDropdownMenu ) },
        { typeof( Blazorise.BarDropdownToggle ), typeof( Components.BarDropdownToggle ) },
        { typeof( Blazorise.BarEnd ), typeof( Components.BarEnd ) },
        { typeof( Blazorise.BarIcon ), typeof( Components.BarIcon ) },
        { typeof( Blazorise.BarItem ), typeof( Components.BarItem ) },
        { typeof( Blazorise.BarLink ), typeof( Components.BarLink ) },
        { typeof( Blazorise.BarMenu ), typeof( Components.BarMenu ) },
        { typeof( Blazorise.BarStart ), typeof( Components.BarStart ) },
        { typeof( Blazorise.BarToggler ), typeof( Components.BarToggler ) },
        { typeof( Blazorise.Breadcrumb ), typeof( Components.Breadcrumb ) },
        { typeof( Blazorise.BreadcrumbItem ), typeof( Components.BreadcrumbItem ) },
        { typeof( Blazorise.BreadcrumbLink ), typeof( Components.BreadcrumbLink ) },
        { typeof( Blazorise.Button ), typeof( Components.Button ) },
        { typeof( Blazorise.CardHeader ), typeof( Components.CardHeader ) },
        { typeof( Blazorise.CardLink ), typeof( Components.CardLink ) },
        { typeof( Blazorise.Carousel ), typeof( Components.Carousel ) },
        { typeof( Blazorise.CarouselSlide ), typeof( Components.CarouselSlide ) },
        { typeof( Blazorise.Check<> ), typeof( Components.Check<> ) },
        { typeof( Blazorise.CloseButton ), typeof( Components.CloseButton ) },
        { typeof( Blazorise.CollapseHeader ), typeof( Components.CollapseHeader ) },
        { typeof( Blazorise.Dropdown ), typeof( Components.Dropdown ) },
        { typeof( Blazorise.DropdownItem ), typeof( Components.DropdownItem ) },
        { typeof( Blazorise.DropdownMenu ), typeof( Components.DropdownMenu ) },
        { typeof( Blazorise.DropdownToggle ), typeof( Components.DropdownToggle ) },
        { typeof( Blazorise.Field ), typeof( Components.Field ) },
        { typeof( Blazorise.FieldBody ), typeof( Components.FieldBody ) },
        { typeof( Blazorise.FieldLabel ), typeof( Components.FieldLabel ) },
        { typeof( Blazorise.FileEdit ), typeof( Components.FileEdit ) },
        { typeof( Blazorise.ListGroup ), typeof( Components.ListGroup ) },
        { typeof( Blazorise.ModalContent ), typeof( Components.ModalContent ) },
        { typeof( Blazorise.Offcanvas ), typeof( Components.Offcanvas ) },
        { typeof( Blazorise.OffcanvasHeader ), typeof( Components.OffcanvasHeader ) },
        { typeof( Blazorise.Progress ), typeof( Components.Progress ) },
        { typeof( Blazorise.Radio<> ), typeof( Components.Radio<> ) },
        { typeof( Blazorise.Rating ), typeof( Components.Rating ) },
        { typeof( Blazorise.RatingItem ), typeof( Components.RatingItem ) },
        { typeof( Blazorise.Select<> ), typeof( Components.Select<> ) },
        { typeof( Blazorise.SelectGroup ), typeof( Components.SelectGroup ) },
        { typeof( Blazorise.SelectItem<> ), typeof( Components.SelectItem<> ) },
        { typeof( Blazorise.Slider<> ), typeof( Components.Slider<> ) },
        { typeof( Blazorise.Step ), typeof( Components.Step ) },
        { typeof( Blazorise.Switch<> ), typeof( Components.Switch<> ) },
        { typeof( Blazorise.Tab ), typeof( Components.Tab ) },
        { typeof( Blazorise.Table ), typeof( Components.Table ) },
        { typeof( Blazorise.TableRowHeader ), typeof( Components.TableRowHeader ) },
        { typeof( Blazorise.TabPanel ), typeof( Components.TabPanel ) },
        { typeof( Blazorise.Tabs ), typeof( Components.Tabs ) },
        { typeof( Blazorise.TabsContent ), typeof( Components.TabsContent ) },
        { typeof( Blazorise.TextEdit ), typeof( Components.TextEdit ) },
        { typeof( Blazorise.Toast ), typeof( Components.Toast ) },
    };
}