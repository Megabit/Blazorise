#region Using directives
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Icons.AntDesign;

public static class Config
{
    public static IServiceCollection AddAntDesignIcons( this IServiceCollection serviceCollection )
    {
        serviceCollection.AddSingleton<AntDesignIconProvider>();
        serviceCollection.AddSingleton<IIconProvider>( serviceProvider => serviceProvider.GetRequiredService<AntDesignIconProvider>() );
        serviceCollection.AddTransient<Blazorise.Icon, Icon>();

        return serviceCollection;
    }
}