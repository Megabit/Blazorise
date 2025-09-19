#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Material.Providers;
using Blazorise.Modules;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Material;

public static class Config
{
    public static IServiceCollection AddMaterialProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
    {
        var classProvider = new MaterialClassProvider();

        configureClassProvider?.Invoke( classProvider );

        serviceCollection.AddSingleton<IClassProvider>( classProvider );
        serviceCollection.AddSingleton<IStyleProvider, MaterialStyleProvider>();
        serviceCollection.AddSingleton<IBehaviourProvider, MaterialBehaviourProvider>();
        serviceCollection.AddScoped<IThemeGenerator, MaterialThemeGenerator>();

        foreach ( var mapping in ComponentMap )
        {
            serviceCollection.AddTransient( mapping.Key, mapping.Value );
        }

        serviceCollection.AddScoped<IJSModalModule, Modules.MaterialJSModalModule>();
        serviceCollection.AddScoped<IJSTooltipModule, Modules.MaterialJSTooltipModule>();

        return serviceCollection;
    }

    public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>( Bootstrap.Config.ComponentMap )
    {
        [typeof( Blazorise.Badge )] = typeof( Components.Badge ),
        [typeof( Blazorise.Check<> )] = typeof( Components.Check<> ),
        [typeof( Blazorise.CarouselSlide )] = typeof( Components.CarouselSlide ),
        [typeof( Blazorise.DropdownToggle )] = typeof( Components.DropdownToggle ),
        [typeof( Blazorise.NumericPicker<> )] = typeof( Components.NumericPicker<> ),
        [typeof( Blazorise.Radio<> )] = typeof( Components.Radio<> ),
        [typeof( Blazorise.Switch<> )] = typeof( Components.Switch<> ),
        [typeof( Blazorise.Step )] = typeof( Components.Step ),
        [typeof( Blazorise.Steps )] = typeof( Components.Steps ),
    };
}