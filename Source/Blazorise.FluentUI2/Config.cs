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
        { typeof( Blazorise.CardHeader ), typeof( FluentUI2.CardHeader ) },
        { typeof( Blazorise.ColorEdit ), typeof( FluentUI2.ColorEdit ) },
        { typeof( Blazorise.ColorPicker ), typeof( FluentUI2.ColorPicker ) },
        { typeof( Blazorise.DateEdit<> ), typeof( FluentUI2.DateEdit<> ) },
        { typeof( Blazorise.DatePicker<> ), typeof( FluentUI2.DatePicker<> ) },
        { typeof( Blazorise.FieldHelp ), typeof( FluentUI2.FieldHelp ) },
        { typeof( Blazorise.FieldLabel ), typeof( FluentUI2.FieldLabel ) },
        { typeof( Blazorise.FileEdit ), typeof( FluentUI2.FileEdit ) },
        { typeof( Blazorise.MemoEdit ), typeof( FluentUI2.MemoEdit ) },
        { typeof( Blazorise.Modal ), typeof( FluentUI2.Modal ) },
        { typeof( Blazorise.NumericEdit<> ), typeof( FluentUI2.NumericEdit<> ) },
        { typeof( Blazorise.TextEdit ), typeof( FluentUI2.TextEdit ) },
        { typeof( Blazorise.TimeEdit<> ), typeof( FluentUI2.TimeEdit<> ) },
        { typeof( Blazorise.TimePicker<> ), typeof( FluentUI2.TimePicker<> ) },
        { typeof( Blazorise.Radio<> ), typeof( FluentUI2.Radio<> ) },
        { typeof( Blazorise.Select<> ), typeof( FluentUI2.Select<> ) },
        { typeof( Blazorise.Slider<> ), typeof( FluentUI2.Slider<> ) },
        { typeof( Blazorise.ValidationError ), typeof( FluentUI2.ValidationError ) },
        { typeof( Blazorise.ValidationSuccess ), typeof( FluentUI2.ValidationSuccess ) },
    };
}