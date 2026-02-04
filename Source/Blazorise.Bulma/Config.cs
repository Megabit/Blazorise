#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Bulma.Providers;
using Blazorise.Modules;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Bulma;

public static class Config
{
    public static IServiceCollection AddBulmaProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
    {
        var classProvider = new BulmaClassProvider();

        configureClassProvider?.Invoke( classProvider );

        serviceCollection.AddSingleton<IClassProvider>( classProvider );
        serviceCollection.AddSingleton<IStyleProvider, BulmaStyleProvider>();
        serviceCollection.AddSingleton<IBehaviourProvider, BulmaBehaviourProvider>();
        serviceCollection.AddScoped<IThemeGenerator, BulmaThemeGenerator>();

        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        serviceCollection.AddScoped<IJSModalModule, Modules.BulmaJSModalModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.BulmaJSTooltipModule>();

        Enumeration<Background>.SetNameBuilder( new BulmaEnumerationNameBuilder<Background>() );
        Enumeration<TextColor>.SetNameBuilder( new BulmaEnumerationNameBuilder<TextColor>() );

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>
    {
        { typeof( Blazorise.AccordionToggle ), typeof( Components.AccordionToggle ) },
        { typeof( Blazorise.Addons ), typeof( Components.Addons ) },
        { typeof( Blazorise.Badge ), typeof( Components.Badge ) },
        { typeof( Blazorise.BarToggler ), typeof( Components.BarToggler ) },
        { typeof( Blazorise.BarDropdown ), typeof( Components.BarDropdown ) },
        { typeof( Blazorise.BarDropdownToggle ), typeof( Components.BarDropdownToggle ) },
        { typeof( Blazorise.Breadcrumb ), typeof( Components.Breadcrumb ) },
        { typeof( Blazorise.BreadcrumbLink ), typeof( Components.BreadcrumbLink ) },
        { typeof( Blazorise.CardImage ), typeof( Components.CardImage ) },
        { typeof( Blazorise.CardTitle ), typeof( Components.CardTitle ) },
        { typeof( Blazorise.CardSubtitle ), typeof( Components.CardSubtitle ) },
        { typeof( Blazorise.Carousel ), typeof( Components.Carousel ) },
        { typeof( Blazorise.CarouselSlide ), typeof( Components.CarouselSlide ) },
        { typeof( Blazorise.Check<> ), typeof( Components.Check<> ) },
        { typeof( Blazorise.DateInput<> ), typeof( Components.DateInput<> ) },
        { typeof( Blazorise.DropdownDivider ), typeof( Components.DropdownDivider ) },
        { typeof( Blazorise.Dropdown ), typeof( Components.Dropdown ) },
        { typeof( Blazorise.DropdownMenu ), typeof( Components.DropdownMenu ) },
        { typeof( Blazorise.DropdownToggle ), typeof( Components.DropdownToggle ) },
        { typeof( Blazorise.Field ), typeof( Components.Field ) },
        { typeof( Blazorise.FieldLabel ), typeof( Components.FieldLabel ) },
        { typeof( Blazorise.FieldHelp ), typeof( Components.FieldHelp ) },
        { typeof( Blazorise.FieldBody ), typeof( Components.FieldBody ) },
        { typeof( Blazorise.Fields ), typeof( Components.Fields ) },
        { typeof( Blazorise.FileInput ), typeof( Components.FileInput ) },
        { typeof( Blazorise.Heading ), typeof( Components.Heading ) },
        { typeof( Blazorise.Jumbotron ), typeof( Components.Jumbotron ) },
        { typeof( Blazorise.JumbotronSubtitle ), typeof( Components.JumbotronSubtitle ) },
        { typeof( Blazorise.ModalContent ), typeof( Components.ModalContent ) },
        { typeof( Blazorise.Radio<> ), typeof( Components.Radio<> ) },
        { typeof( Blazorise.Select<> ), typeof( Components.Select<> ) },
        { typeof( Blazorise.Switch<> ), typeof( Components.Switch<> ) },
        { typeof( Blazorise.Button ), typeof( Components.Button ) },
        { typeof( Blazorise.Table ), typeof( Components.Table ) },
        { typeof( Blazorise.Tabs ), typeof( Components.Tabs ) },
        { typeof( Blazorise.TextInput ), typeof( Components.TextInput ) },
        { typeof( Blazorise.TimeInput<> ), typeof( Components.TimeInput<> ) },
        { typeof( Blazorise.NumericInput<> ), typeof( Components.NumericInput<> ) },
        { typeof( Blazorise.NumericPicker<> ), typeof( Components.NumericPicker<> ) },
        { typeof( Blazorise.Pagination ), typeof( Components.Pagination ) },
        { typeof( Blazorise.PaginationLink ), typeof( Components.PaginationLink ) },
        { typeof( Blazorise.Progress ), typeof( Components.Progress ) },
        { typeof( Blazorise.ProgressBar ), typeof( Components.ProgressBar ) },
        { typeof( Blazorise.Steps ), typeof( Components.Steps ) },
        { typeof( Blazorise.Step ), typeof( Components.Step ) },
    };
}