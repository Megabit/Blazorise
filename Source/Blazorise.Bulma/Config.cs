#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Bulma
{
    public static class Config
    {
        public static IServiceCollection AddBulmaClassProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, BulmaClassProvider>();
            serviceCollection.AddSingleton<IJSRunner, JSRunner>();

            return serviceCollection;
        }

        public static IServiceCollection AddBulmaStyleProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IStyleProvider, BulmaStyleProvider>();

            return serviceCollection;
        }
    }
}
