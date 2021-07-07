#region Using directives
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Icons.Material
{
    public static class Config
    {
        public static IServiceCollection AddMaterialIcons( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IIconProvider, MaterialIconProvider>();

            serviceCollection.AddTransient<Blazorise.Icon, Material.Icon>();

            return serviceCollection;
        }
    }
}
