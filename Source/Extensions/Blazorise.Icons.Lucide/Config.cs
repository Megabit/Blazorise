#region Using directives
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Icons.Lucide;

public static class Config
{
    public static IServiceCollection AddLucideIcons( this IServiceCollection serviceCollection )
    {
        serviceCollection.AddSingleton<LucideIconProvider>();
        serviceCollection.AddSingleton<IIconProvider>( serviceProvider => serviceProvider.GetRequiredService<LucideIconProvider>() );
        serviceCollection.AddTransient<Blazorise.Icon, Icon>();

        return serviceCollection;
    }
}