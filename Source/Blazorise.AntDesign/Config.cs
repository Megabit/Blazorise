#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.AntDesign.Modules;
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
        serviceCollection.AddScoped<AntDesignJSSegmentedModule>();

        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        serviceCollection.AddScoped<IJSModalModule, Modules.AntDesignJSModalModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.AntDesignJSTooltipModule>();

        Enumeration<Background>.SetNameBuilder( new AntDesignEnumerationNameBuilder<Background>() );
        Enumeration<TextColor>.SetNameBuilder( new AntDesignEnumerationNameBuilder<TextColor>() );

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>
    {
        { typeof( Blazorise.AccordionHeader ), typeof( Components.AccordionHeaderCollapse ) },
        { typeof( Blazorise.AccordionToggle ), typeof( Components.AccordionToggle ) },
        { typeof( Blazorise.Addon ), typeof( Components.Addon ) },
        { typeof( Blazorise.AddonLabel ), typeof( Components.AddonLabel ) },
        { typeof( Blazorise.Addons ), typeof( Components.Addons ) },
        { typeof( Blazorise.Alert ), typeof( Components.Alert ) },
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
        { typeof( Blazorise.CardActions ), typeof( Components.CardActions ) },
        { typeof( Blazorise.CardHeader ), typeof( Components.CardHeader ) },
        { typeof( Blazorise.CardImage ), typeof( Components.CardImage ) },
        { typeof( Blazorise.CardLink ), typeof( Components.CardLink ) },
        { typeof( Blazorise.Carousel ), typeof( Components.Carousel ) },
        { typeof( Blazorise.CarouselSlide ), typeof( Components.CarouselSlide ) },
        { typeof( Blazorise.Check<> ), typeof( Components.Check<> ) },
        { typeof( Blazorise.CloseButton ), typeof( Components.CloseButton ) },
        { typeof( Blazorise.CollapseHeader ), typeof( Components.CollapseHeader ) },
        { typeof( Blazorise.ColorInput ), typeof( Components.ColorInput ) },
        { typeof( Blazorise.Dropdown ), typeof( Components.Dropdown ) },
        { typeof( Blazorise.DropdownDivider ), typeof( Components.DropdownDivider ) },
        { typeof( Blazorise.DropdownHeader ), typeof( Components.DropdownHeader ) },
        { typeof( Blazorise.DropdownItem ), typeof( Components.DropdownItem ) },
        { typeof( Blazorise.DropdownMenu ), typeof( Components.DropdownMenu ) },
        { typeof( Blazorise.DropdownToggle ), typeof( Components.DropdownToggle ) },
        { typeof( Blazorise.DateInput<> ), typeof( Components.DateInput<> ) },
        { typeof( Blazorise.DatePicker<> ), typeof( Components.DatePicker<> ) },
        { typeof( Blazorise.Field ), typeof( Components.Field ) },
        { typeof( Blazorise.FieldBody ), typeof( Components.FieldBody ) },
        { typeof( Blazorise.FieldLabel ), typeof( Components.FieldLabel ) },
        { typeof( Blazorise.FilePicker ), typeof( Components.FilePicker ) },
        { typeof( Blazorise.FileInput ), typeof( Components.FileInput ) },
        { typeof( Blazorise.ListGroup ), typeof( Components.ListGroup ) },
        { typeof( Blazorise.MemoInput ), typeof( Components.MemoInput ) },
        { typeof( Blazorise.ModalContent ), typeof( Components.ModalContent ) },
        { typeof( Blazorise.NumericInput<> ), typeof( Components.NumericInput<> ) },
        { typeof( Blazorise.NumericPicker<> ), typeof( Components.NumericPicker<> ) },
        { typeof( Blazorise.Offcanvas ), typeof( Components.Offcanvas ) },
        { typeof( Blazorise.OffcanvasHeader ), typeof( Components.OffcanvasHeader ) },
        { typeof( Blazorise.Progress ), typeof( Components.Progress ) },
        { typeof( Blazorise.ProgressBar ), typeof( Components.ProgressBar ) },
        { typeof( Blazorise.PageProgress ), typeof( Components.PageProgress ) },
        { typeof( Blazorise.Radio<> ), typeof( Components.Radio<> ) },
        { typeof( Blazorise.RadioGroup<> ), typeof( Components.RadioGroup<> ) },
        { typeof( Blazorise.Rating ), typeof( Components.Rating ) },
        { typeof( Blazorise.RatingItem ), typeof( Components.RatingItem ) },
        { typeof( Blazorise.Select<> ), typeof( Components.Select<> ) },
        { typeof( Blazorise.SelectGroup ), typeof( Components.SelectGroup ) },
        { typeof( Blazorise.SelectItem<> ), typeof( Components.SelectItem<> ) },
        { typeof( Blazorise.SkeletonItem ), typeof( Components.SkeletonItem ) },
        { typeof( Blazorise.Slider<> ), typeof( Components.Slider<> ) },
        { typeof( Blazorise.Steps ), typeof( Components.Steps ) },
        { typeof( Blazorise.Step ), typeof( Components.Step ) },
        { typeof( Blazorise.StepPanel ), typeof( Components.StepPanel ) },
        { typeof( Blazorise.StepsContent ), typeof( Components.StepsContent ) },
        { typeof( Blazorise.Switch<> ), typeof( Components.Switch<> ) },
        { typeof( Blazorise.Tab ), typeof( Components.Tab ) },
        { typeof( Blazorise.Table ), typeof( Components.Table ) },
        { typeof( Blazorise.TableBody ), typeof( Components.TableBody ) },
        { typeof( Blazorise.TableCaption ), typeof( Components.TableCaption ) },
        { typeof( Blazorise.TableFooter ), typeof( Components.TableFooter ) },
        { typeof( Blazorise.TableHeader ), typeof( Components.TableHeader ) },
        { typeof( Blazorise.TableRowHeader ), typeof( Components.TableRowHeader ) },
        { typeof( Blazorise.TabPanel ), typeof( Components.TabPanel ) },
        { typeof( Blazorise.Tabs ), typeof( Components.Tabs ) },
        { typeof( Blazorise.TabsContent ), typeof( Components.TabsContent ) },
        { typeof( Blazorise.TextInput ), typeof( Components.TextInput ) },
        { typeof( Blazorise.TimeInput<> ), typeof( Components.TimeInput<> ) },
        { typeof( Blazorise.TimePicker<> ), typeof( Components.TimePicker<> ) },
        { typeof( Blazorise.Toaster ), typeof( Components.Toaster ) },
        { typeof( Blazorise.Toast ), typeof( Components.Toast ) },
        { typeof( Blazorise.ToastBody ), typeof( Components.ToastBody ) },
        { typeof( Blazorise.ToastHeader ), typeof( Components.ToastHeader ) },
    };
}