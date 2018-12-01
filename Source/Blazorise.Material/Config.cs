#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Material
{
    public static class Config
    {
        public static IServiceCollection AddMaterialClassProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, MaterialClassProvider>();
            serviceCollection.AddSingleton<IJSRunner, JSRunner>();

            return serviceCollection;
        }

        public static IServiceCollection AddMaterialStyleProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IStyleProvider, MaterialStyleProvider>();

            return serviceCollection;
        }
    }
}
