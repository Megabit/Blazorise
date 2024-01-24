#region Using directives
using Blazorise.Icons.Fabric;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Icons.Fabric;

public static class Config
{
    public static IServiceCollection AddFabricIcons( this IServiceCollection serviceCollection )
    {
        serviceCollection.AddSingleton<IIconProvider, FabricIconProvider>();

        serviceCollection.AddTransient<Blazorise.Icon, Fabric.Icon>();

        return serviceCollection;
    }
}