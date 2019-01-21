#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Bootstrap;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Material
{
    public static class Config
    {
        public static IServiceCollection AddMaterialProviders( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, MaterialClassProvider>();
            serviceCollection.AddSingleton<IStyleProvider, MaterialStyleProvider>();
            serviceCollection.AddSingleton<IJSRunner, MaterialJSRunner>();
            serviceCollection.AddSingleton<IComponentMapper, ComponentMapper>();

            return serviceCollection;
        }

        /// <summary>
        /// Registers the custom rules for material components.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IBlazorApplicationBuilder UseMaterialProviders( this IBlazorApplicationBuilder app )
        {
            // same components as in bootstrap provider
            app.UseBootstrapProviders();

            return app;
        }
    }
}
