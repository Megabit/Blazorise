#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Modules;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.FluentUI2;

public static class Config
{
    public static IServiceCollection AddFluentUI2Providers( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
    {
        var classProvider = new FluentUI2ClassProvider();

        configureClassProvider?.Invoke( classProvider );

        serviceCollection.AddSingleton<IClassProvider>( classProvider );
        serviceCollection.AddSingleton<IStyleProvider, FluentUI2StyleProvider>();
        serviceCollection.AddSingleton<IBehaviourProvider, FluentUI2BehaviourProvider>();
        serviceCollection.AddSingleton<IThemeGenerator, FluentUI2ThemeGenerator>();

        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        serviceCollection.AddScoped<IJSModalModule, Modules.FluentUI2JSModalModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.FluentUI2JSTooltipModule>();

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>
    {
        { typeof( Blazorise.Addon ), typeof( Components.Addon ) },        
        { typeof( Blazorise.Alert ), typeof( Components.Alert ) },
        { typeof( Blazorise.AlertDescription ), typeof( Components.AlertDescription ) },
        { typeof( Blazorise.AlertMessage ), typeof( Components.AlertMessage ) },
        { typeof( Blazorise.Breadcrumb ), typeof( Components.Breadcrumb ) },
        { typeof( Blazorise.BreadcrumbItem ), typeof( Components.BreadcrumbItem ) },
        { typeof( Blazorise.BreadcrumbLink ), typeof( Components.BreadcrumbLink ) },
        { typeof( Blazorise.CardHeader ), typeof( Components.CardHeader ) },
        { typeof( Blazorise.Check<> ), typeof( Components.Check<> ) },
        { typeof( Blazorise.ColorEdit ), typeof( Components.ColorEdit ) },
        { typeof( Blazorise.ColorPicker ), typeof( Components.ColorPicker ) },
        { typeof( Blazorise.CloseButton ), typeof( Components.CloseButton ) },
        { typeof( Blazorise.DateEdit<> ), typeof( Components.DateEdit<> ) },
        { typeof( Blazorise.DatePicker<> ), typeof( Components.DatePicker<> ) },
        { typeof( Blazorise.DropdownHeader ), typeof( Components.DropdownHeader ) },
        { typeof( Blazorise.DropdownItem ), typeof( Components.DropdownItem ) },        
        { typeof( Blazorise.DropdownMenu ), typeof( Components.DropdownMenu ) },
        { typeof( Blazorise.DropdownToggle ), typeof( Components.DropdownToggle ) },
        { typeof( Blazorise.Field ), typeof( Components.Field ) },
        { typeof( Blazorise.FieldHelp ), typeof( Components.FieldHelp ) },
        { typeof( Blazorise.FieldLabel ), typeof( Components.FieldLabel ) },
        { typeof( Blazorise.FileEdit ), typeof( Components.FileEdit ) },
        { typeof( Blazorise.MemoEdit ), typeof( Components.MemoEdit ) },
        { typeof( Blazorise.Modal ), typeof( Components.Modal ) },
        { typeof( Blazorise.ModalContent ), typeof( Components.ModalContent ) },
        { typeof( Blazorise.Offcanvas ), typeof( Components.Offcanvas ) },
        { typeof( Blazorise.NumericEdit<> ), typeof( Components.NumericEdit<> ) },
        { typeof( Blazorise.TextEdit ), typeof( Components.TextEdit ) },
        { typeof( Blazorise.TimeEdit<> ), typeof( Components.TimeEdit<> ) },
        { typeof( Blazorise.TimePicker<> ), typeof( Components.TimePicker<> ) },
        { typeof( Blazorise.ProgressBar ), typeof( Components.ProgressBar ) },
        { typeof( Blazorise.Radio<> ), typeof( Components.Radio<> ) },
        { typeof( Blazorise.RatingItem ), typeof( Components.RatingItem ) },
        { typeof( Blazorise.Select<> ), typeof( Components.Select<> ) },
        { typeof( Blazorise.Slider<> ), typeof( Components.Slider<> ) },
        { typeof( Blazorise.Switch<> ), typeof( Components.Switch<> ) },
        { typeof( Blazorise.ValidationError ), typeof( Components.ValidationError ) },
        { typeof( Blazorise.ValidationSuccess ), typeof( Components.ValidationSuccess ) },
    };
}