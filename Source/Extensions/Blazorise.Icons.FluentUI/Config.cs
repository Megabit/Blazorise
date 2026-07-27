#region Using directives
using System;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Icons.FluentUI;

public static class Config
{
    public static IServiceCollection AddFluentUIIcons( this IServiceCollection serviceCollection )
    {
        return AddFluentUIIcons( serviceCollection, null );
    }

    public static IServiceCollection AddFluentUIIcons( this IServiceCollection serviceCollection, Action<FluentUIIconOptions> configureOptions )
    {
        FluentUIIconOptions options = new();
        configureOptions?.Invoke( options );

        serviceCollection.AddSingleton( options );
        serviceCollection.AddSingleton<FluentUIIconProvider>();
        serviceCollection.AddSingleton<IIconProvider>( serviceProvider => serviceProvider.GetRequiredService<FluentUIIconProvider>() );

        serviceCollection.AddTransient<Blazorise.Icon, FluentUI.Icon>();

        return serviceCollection;
    }
}