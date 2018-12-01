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
        public static IServiceCollection AddBootstrapClassProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, BootstrapClassProvider>();
            serviceCollection.AddSingleton<IJSRunner, JSRunner>();

            return serviceCollection;
        }

        public static IServiceCollection AddBootstrapStyleProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IStyleProvider, BootstrapStyleProvider>();

            return serviceCollection;
        }
    }
}
