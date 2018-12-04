#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Bootstrap
{
    public static class Config
    {
        public static IServiceCollection AddBootstrap( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, BootstrapClassProvider>();
            serviceCollection.AddSingleton<IStyleProvider, BootstrapStyleProvider>();
            serviceCollection.AddSingleton<IJSRunner, JSRunner>();

            var mapper = new ComponentMapper();

            mapper.Register<Blazorise.Custom, CustomBS>();

            serviceCollection.AddSingleton<IComponentMapper>( ( p ) => mapper );

            return serviceCollection;
        }

        [Obsolete( "AddBootstrapClassProvider is deprecated, please use AddBootstrap instead." )]
        public static IServiceCollection AddBootstrapClassProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, BootstrapClassProvider>();
            serviceCollection.AddSingleton<IJSRunner, JSRunner>();

            return serviceCollection;
        }

        [Obsolete( "AddBootstrapStyleProvider is deprecated, please use AddBootstrap instead." )]
        public static IServiceCollection AddBootstrapStyleProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IStyleProvider, BootstrapStyleProvider>();

            return serviceCollection;
        }
    }
}
