#region Using directives
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Icons.FluentUI;

public static class Config
{
    public static IServiceCollection AddFluentUIIcons( this IServiceCollection serviceCollection )
    {
        serviceCollection.AddSingleton<IIconProvider, FluentUIIconProvider>();

        serviceCollection.AddTransient<Blazorise.Icon, FluentUI.Icon>();

        return serviceCollection;
    }
}