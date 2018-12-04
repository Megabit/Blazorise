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

            mapper.Register<Blazorise.Addon, Addon>();
            mapper.Register<Blazorise.Addons, Addons>();
            mapper.Register<Blazorise.BarToggler, BarToggler>();
            mapper.Register<Blazorise.CardSubtitle, CardSubtitle>();
            mapper.Register<Blazorise.CloseButton, CloseButton>();
            mapper.Register<Blazorise.CheckEdit, CheckEdit>();
            mapper.Register<Blazorise.DateEdit, DateEdit>();
            mapper.Register<Blazorise.Field, Field>();
            mapper.Register<Blazorise.FileEdit, FileEdit>();
            mapper.Register<Blazorise.ModalContent, ModalContent>();
            mapper.Register<Blazorise.MemoEdit, MemoEdit>();
            mapper.Register<Blazorise.SelectEdit, SelectEdit>();
            mapper.Register<Blazorise.SimpleButton, SimpleButton>();
            mapper.Register<Blazorise.TextEdit, TextEdit>();

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
