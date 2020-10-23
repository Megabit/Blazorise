#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Blazorise.Providers;
using Microsoft.Extensions.Options;
#endregion

namespace Blazorise
{
    public static class Config
    {
        /// <summary>
        /// Register blazorise and configures the default behaviour.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddBlazorise( this IServiceCollection serviceCollection, Action<BlazoriseOptions> configureOptions = null )
        {
            serviceCollection.AddSingleton( configureOptions );
            serviceCollection.AddSingleton<BlazoriseOptions>();

            serviceCollection.AddScoped<IEditContextValidator, EditContextValidator>();

            return serviceCollection;
        }

        /// <summary>
        /// Registers an empty providers.
        /// </summary>
        /// <remarks>
        /// Generaly this should not be used, except when the user wants to use extensions without any providers like Bootstrap or Bulma.
        /// </remarks>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddEmptyProviders( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, EmptyClassProvider>();
            serviceCollection.AddSingleton<IStyleProvider, EmptyStyleProvider>();
            serviceCollection.AddSingleton<IComponentMapper, ComponentMapper>();

            serviceCollection.AddScoped<IJSRunner, EmptyJSRunner>();

            return serviceCollection;
        }

        /// <summary>
        /// Registers a custom class provider.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="classProviderFactory"></param>
        /// <returns></returns>
        public static IServiceCollection AddClassProvider( this IServiceCollection serviceCollection, Func<IClassProvider> classProviderFactory )
        {
            serviceCollection.AddSingleton( ( p ) => classProviderFactory() );

            return serviceCollection;
        }

        /// <summary>
        /// Registers a custom style provider.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="styleProviderFactory"></param>
        /// <returns></returns>
        public static IServiceCollection AddStyleProvider( this IServiceCollection serviceCollection, Func<IStyleProvider> styleProviderFactory )
        {
            serviceCollection.AddSingleton( ( p ) => styleProviderFactory() );

            return serviceCollection;
        }

        /// <summary>
        /// Registers a custom js runner.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="jsRunnerFactory"></param>
        /// <returns></returns>
        public static IServiceCollection AddJSRunner( this IServiceCollection serviceCollection, Func<IJSRunner> jsRunnerFactory )
        {
            serviceCollection.AddScoped( ( p ) => jsRunnerFactory() );

            return serviceCollection;
        }

        /// <summary>
        /// Registers a custom icon provider.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="iconProviderFactory"></param>
        public static IServiceCollection AddIconProvider( this IServiceCollection serviceCollection, Func<IIconProvider> iconProviderFactory )
        {
            serviceCollection.AddSingleton( ( p ) => iconProviderFactory() );

            return serviceCollection;
        }
    }
}
