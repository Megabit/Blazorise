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
        /// <summary>
        /// Adds a bootstrap providers and component mappings.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddBootstrapProviders( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, BootstrapClassProvider>();
            serviceCollection.AddSingleton<IStyleProvider, BootstrapStyleProvider>();
            serviceCollection.AddSingleton<IJSRunner, JSRunner>();

            var componentMapper = new ComponentMapper();

            componentMapper.Register<Blazorise.Addon, Addon>();
            componentMapper.Register<Blazorise.Addons, Addons>();
            componentMapper.Register<Blazorise.BarToggler, BarToggler>();
            componentMapper.Register<Blazorise.CardSubtitle, CardSubtitle>();
            componentMapper.Register<Blazorise.CloseButton, CloseButton>();
            componentMapper.Register<Blazorise.CheckEdit, CheckEdit>();
            componentMapper.Register<Blazorise.DateEdit, DateEdit>();
            componentMapper.Register<Blazorise.Field, Field>();
            componentMapper.Register<Blazorise.FileEdit, FileEdit>();
            componentMapper.Register<Blazorise.ModalContent, ModalContent>();
            componentMapper.Register<Blazorise.MemoEdit, MemoEdit>();
            componentMapper.Register<Blazorise.SelectEdit, SelectEdit>();
            componentMapper.Register<Blazorise.SimpleButton, SimpleButton>();
            componentMapper.Register<Blazorise.TextEdit, TextEdit>();

            serviceCollection.AddSingleton<IComponentMapper>( ( p ) => componentMapper );

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
