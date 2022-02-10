#region Using directives
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Icons.Bootstrap
{
    public static class Config
    {
        public static IServiceCollection AddBootstrapIcons( this IServiceCollection serviceCollection ) => serviceCollection.AddSingleton<IIconProvider, BootstrapIconProvider>();
    }
}
