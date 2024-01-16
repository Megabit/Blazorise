#region Using directives
using System;
using System.Collections.Generic;
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
        { typeof( Blazorise.Addons ), typeof( AntDesign.Addons ) },
        { typeof( Blazorise.Addon ), typeof( AntDesign.Addon ) },
        { typeof( Blazorise.AddonLabel ), typeof( AntDesign.AddonLabel ) },
        { typeof( Blazorise.AlertMessage ), typeof( AntDesign.AlertMessage ) },
        { typeof( Blazorise.AlertDescription ), typeof( AntDesign.AlertDescription ) },
        { typeof( Blazorise.Badge ), typeof( AntDesign.Badge ) },
        { typeof( Blazorise.Bar ), typeof( AntDesign.Bar ) },
        { typeof( Blazorise.BarBrand ), typeof( AntDesign.BarBrand ) },
        { typeof( Blazorise.BarIcon ), typeof( AntDesign.BarIcon ) },
        { typeof( Blazorise.BarItem ), typeof( AntDesign.BarItem ) },
        { typeof( Blazorise.BarMenu ), typeof( AntDesign.BarMenu ) },
        { typeof( Blazorise.BarStart ), typeof( AntDesign.BarStart ) },
        { typeof( Blazorise.BarEnd ), typeof( AntDesign.BarEnd ) },
        { typeof( Blazorise.BarDropdown ), typeof( AntDesign.BarDropdown ) },
        { typeof( Blazorise.BarLink ), typeof( AntDesign.BarLink ) },
        { typeof( Blazorise.BarDropdownMenu ), typeof( AntDesign.BarDropdownMenu ) },
        { typeof( Blazorise.BarDropdownItem ), typeof( AntDesign.BarDropdownItem ) },
        { typeof( Blazorise.BarDropdownDivider ), typeof( AntDesign.BarDropdownDivider ) },
        { typeof( Blazorise.BarDropdownToggle ), typeof( AntDesign.BarDropdownToggle ) },
        { typeof( Blazorise.BarToggler ), typeof( AntDesign.BarToggler ) },
        { typeof( Blazorise.Breadcrumb ), typeof( AntDesign.Breadcrumb ) },
        { typeof( Blazorise.BreadcrumbItem ), typeof( AntDesign.BreadcrumbItem ) },
        { typeof( Blazorise.BreadcrumbLink ), typeof( AntDesign.BreadcrumbLink ) },
        { typeof( Blazorise.Check<> ), typeof( AntDesign.Check<> ) },
        { typeof( Blazorise.Button ), typeof( AntDesign.Button ) },
        { typeof( Blazorise.CardHeader ), typeof( AntDesign.CardHeader ) },
        { typeof( Blazorise.CardLink ), typeof( AntDesign.CardLink ) },
        { typeof( Blazorise.Carousel ), typeof( AntDesign.Carousel ) },
        { typeof( Blazorise.CarouselSlide ), typeof( AntDesign.CarouselSlide ) },
        { typeof( Blazorise.CloseButton ), typeof( AntDesign.CloseButton ) },
        { typeof( Blazorise.CollapseHeader ), typeof( AntDesign.CollapseHeader ) },
        { typeof( Blazorise.Dropdown ), typeof( AntDesign.Dropdown ) },
        { typeof( Blazorise.DropdownMenu ), typeof( AntDesign.DropdownMenu ) },
        { typeof( Blazorise.DropdownItem ), typeof( AntDesign.DropdownItem ) },
        { typeof( Blazorise.DropdownToggle ), typeof( AntDesign.DropdownToggle ) },
        { typeof( Blazorise.Field ), typeof( AntDesign.Field ) },
        { typeof( Blazorise.FieldBody ), typeof( AntDesign.FieldBody ) },
        { typeof( Blazorise.FieldLabel ), typeof( AntDesign.FieldLabel ) },
        { typeof( Blazorise.FileEdit ), typeof( AntDesign.FileEdit ) },
        { typeof( Blazorise.ListGroup ), typeof( AntDesign.ListGroup ) },
        { typeof( Blazorise.ModalContent ), typeof( AntDesign.ModalContent ) },
        { typeof( Blazorise.Offcanvas ), typeof( AntDesign.Offcanvas ) },
        { typeof( Blazorise.OffcanvasHeader ), typeof( AntDesign.OffcanvasHeader ) },
        { typeof( Blazorise.Progress ), typeof( AntDesign.Progress ) },
        { typeof( Blazorise.Select<> ), typeof( AntDesign.Select<> ) },
        { typeof( Blazorise.SelectItem<> ), typeof( AntDesign.SelectItem<> ) },
        { typeof( Blazorise.SelectGroup ), typeof( AntDesign.SelectGroup ) },
        { typeof( Blazorise.Radio<> ), typeof( AntDesign.Radio<> ) },
        { typeof( Blazorise.Slider<> ), typeof( AntDesign.Slider<> ) },
        { typeof( Blazorise.Switch<> ), typeof( AntDesign.Switch<> ) },
        { typeof( Blazorise.Tabs ), typeof( AntDesign.Tabs ) },
        { typeof( Blazorise.Tab ), typeof( AntDesign.Tab ) },
        { typeof( Blazorise.TabPanel ), typeof( AntDesign.TabPanel ) },
        { typeof( Blazorise.TabsContent ), typeof( AntDesign.TabsContent ) },
        { typeof( Blazorise.Table ), typeof( AntDesign.Table ) },
        { typeof( Blazorise.TableRowHeader ), typeof( AntDesign.TableRowHeader ) },
        { typeof( Blazorise.TextEdit ), typeof( AntDesign.TextEdit ) },
        { typeof( Blazorise.Step ), typeof( AntDesign.Step ) },
        { typeof( Blazorise.Rating ), typeof( AntDesign.Rating ) },
        { typeof( Blazorise.RatingItem ), typeof( AntDesign.RatingItem ) },
    };
}