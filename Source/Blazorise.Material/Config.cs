#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Bootstrap;
#if NETSTANDARD2_0
using Microsoft.AspNetCore.Components.Builder;
#elif NETCORE3_0
using Microsoft.AspNetCore.Builder;
#endif
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Material
{
    public static class Config
    {
        public static IServiceCollection AddMaterialProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
        {
            var classProvider = new MaterialClassProvider();

            configureClassProvider?.Invoke( classProvider );

            serviceCollection.AddSingleton<IClassProvider>( classProvider );
            serviceCollection.AddSingleton<IStyleProvider, MaterialStyleProvider>();
            serviceCollection.AddScoped<IJSRunner, MaterialJSRunner>();
            serviceCollection.AddSingleton<IComponentMapper, ComponentMapper>();

            return serviceCollection;
        }

#if NETSTANDARD2_0

        /// <summary>
        /// Registers the custom rules for material components.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IComponentsApplicationBuilder UseMaterialProviders( this IComponentsApplicationBuilder app )
        {
            // same components as in bootstrap provider
            app.UseBootstrapProviders();

            return app;
        }

#else

        /// <summary>
        /// Registers the custom rules for material components.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMaterialProviders( this IApplicationBuilder app )
        {
            // same components as in bootstrap provider
            app.UseBootstrapProviders();

            return app;
        }

#endif
    }
}
