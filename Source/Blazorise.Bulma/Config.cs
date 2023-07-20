#region Using directives
using System;
using System.Collections.Generic;
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
        serviceCollection.AddSingleton<IThemeGenerator, BulmaThemeGenerator>();

        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        serviceCollection.AddScoped<IJSModalModule, Modules.BulmaJSModalModule>();
        serviceCollection.AddScoped<IJSOffcanvasModule, Modules.BulmaJSOffcanvasModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.BulmaJSTooltipModule>();

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>
    {
        { typeof( Blazorise.Addons ), typeof( Bulma.Addons ) },
        { typeof( Blazorise.Badge ), typeof( Bulma.Badge ) },
        { typeof( Blazorise.BarToggler ), typeof( Bulma.BarToggler ) },
        { typeof( Blazorise.BarDropdown ), typeof( Bulma.BarDropdown ) },
        { typeof( Blazorise.BarDropdownToggle ), typeof( Bulma.BarDropdownToggle ) },
        { typeof( Blazorise.Breadcrumb ), typeof( Bulma.Breadcrumb ) },
        { typeof( Blazorise.BreadcrumbLink ), typeof( Bulma.BreadcrumbLink ) },
        { typeof( Blazorise.CardImage ), typeof( Bulma.CardImage ) },
        { typeof( Blazorise.CardTitle ), typeof( Bulma.CardTitle ) },
        { typeof( Blazorise.CardSubtitle ), typeof( Bulma.CardSubtitle ) },
        { typeof( Blazorise.Carousel ), typeof( Bulma.Carousel ) },
        { typeof( Blazorise.Check<> ), typeof( Bulma.Check<> ) },
        { typeof( Blazorise.DateEdit<> ), typeof( Bulma.DateEdit<> ) },
        { typeof( Blazorise.DropdownDivider ), typeof( Bulma.DropdownDivider ) },
        { typeof( Blazorise.Dropdown ), typeof( Bulma.Dropdown ) },
        { typeof( Blazorise.DropdownMenu ), typeof( Bulma.DropdownMenu ) },
        { typeof( Blazorise.DropdownToggle ), typeof( Bulma.DropdownToggle ) },
        { typeof( Blazorise.Field ), typeof( Bulma.Field ) },
        { typeof( Blazorise.FieldLabel ), typeof( Bulma.FieldLabel ) },
        { typeof( Blazorise.FieldHelp ), typeof( Bulma.FieldHelp ) },
        { typeof( Blazorise.FieldBody ), typeof( Bulma.FieldBody ) },
        { typeof( Blazorise.Fields ), typeof( Bulma.Fields ) },
        { typeof( Blazorise.FileEdit ), typeof( Bulma.FileEdit ) },
        { typeof( Blazorise.Heading ), typeof( Bulma.Heading ) },
        { typeof( Blazorise.Jumbotron ), typeof( Bulma.Jumbotron ) },
        { typeof( Blazorise.JumbotronSubtitle ), typeof( Bulma.JumbotronSubtitle ) },
        { typeof( Blazorise.ModalContent ), typeof( Bulma.ModalContent ) },
        { typeof( Blazorise.Radio<> ), typeof( Bulma.Radio<> ) },
        { typeof( Blazorise.Select<> ), typeof( Bulma.Select<> ) },
        { typeof( Blazorise.Switch<> ), typeof( Bulma.Switch<> ) },
        { typeof( Blazorise.Button ), typeof( Bulma.Button ) },
        { typeof( Blazorise.Table ), typeof( Bulma.Table ) },
        { typeof( Blazorise.Tabs ), typeof( Bulma.Tabs ) },
        { typeof( Blazorise.TextEdit ), typeof( Bulma.TextEdit ) },
        { typeof( Blazorise.TimeEdit<> ), typeof( Bulma.TimeEdit<> ) },
        { typeof( Blazorise.NumericEdit<> ), typeof( Bulma.NumericEdit<> ) },
        { typeof( Blazorise.NumericPicker<> ), typeof( Bulma.NumericPicker<> ) },
        { typeof( Blazorise.Pagination ), typeof( Bulma.Pagination ) },
        { typeof( Blazorise.PaginationLink ), typeof( Bulma.PaginationLink ) },
        { typeof( Blazorise.Progress ), typeof( Bulma.Progress ) },
        { typeof( Blazorise.ProgressBar ), typeof( Bulma.ProgressBar ) },
        { typeof( Blazorise.Steps ), typeof( Bulma.Steps ) },
        { typeof( Blazorise.Step ), typeof( Bulma.Step ) },
    };
}