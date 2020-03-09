#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.AntDesign
{
    public static class Config
    {
        /// <summary>
        /// Adds a ant design providers and component mappings.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddAntDesignProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
        {
            var classProvider = new AntDesignClassProvider();

            configureClassProvider?.Invoke( classProvider );

            serviceCollection.AddSingleton<IClassProvider>( classProvider );
            serviceCollection.AddSingleton<IStyleProvider, AntDesignStyleProvider>();
            serviceCollection.AddScoped<IJSRunner, AntDesignJSRunner>();
            serviceCollection.AddSingleton<IComponentMapper, ComponentMapper>();
            serviceCollection.AddScoped<IThemeGenerator, AntDesignThemeGenerator>();

            return serviceCollection;
        }

        private static void RegisterComponents( IComponentMapper componentMapper )
        {
            componentMapper.Register<Blazorise.CardHeader, AntDesign.CardHeader>();
            componentMapper.Register<Blazorise.DropdownMenu, AntDesign.DropdownMenu>();
            componentMapper.Register<Blazorise.DropdownItem, AntDesign.DropdownItem>();
            componentMapper.Register<Blazorise.ModalBackdrop, AntDesign.ModalBackdrop>();
            componentMapper.Register<Blazorise.ModalContent, AntDesign.ModalContent>();
            componentMapper.Register<Blazorise.Progress, AntDesign.Progress>();
            componentMapper.Register( typeof( Blazorise.Select<> ), typeof( AntDesign.Select<> ) );
        }

        /// <summary>
        /// Registers the custom rules for ant design components.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IServiceProvider UseAntDesignProviders( this IServiceProvider serviceProvider )
        {
            var componentMapper = serviceProvider.GetRequiredService<IComponentMapper>();

            RegisterComponents( componentMapper );

            return serviceProvider;
        }
    }
}
