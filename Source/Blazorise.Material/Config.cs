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
        public static IServiceCollection AddMaterialProviders( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, MaterialClassProvider>();
            serviceCollection.AddSingleton<IStyleProvider, MaterialStyleProvider>();
            serviceCollection.AddSingleton<IJSRunner, JSRunner>();

            var componentMapper = new ComponentMapper();

            // same components as in bootstrap provider
            componentMapper.Register<Blazorise.Addon, Bootstrap.Addon>();
            //componentMapper.Register<Blazorise.Addons, Bootstrap.Addons>();
            componentMapper.Register<Blazorise.BarToggler, Bootstrap.BarToggler>();
            componentMapper.Register<Blazorise.CardSubtitle, Bootstrap.CardSubtitle>();
            componentMapper.Register<Blazorise.CloseButton, Bootstrap.CloseButton>();
            componentMapper.Register<Blazorise.CheckEdit, Bootstrap.CheckEdit>();
            //componentMapper.Register<Blazorise.DateEdit, Bootstrap.DateEdit>();
            componentMapper.Register<Blazorise.Field, Bootstrap.Field>();
            //componentMapper.Register<Blazorise.FileEdit, Bootstrap.FileEdit>();
            componentMapper.Register<Blazorise.ModalContent, Bootstrap.ModalContent>();
            //componentMapper.Register<Blazorise.MemoEdit, Bootstrap.MemoEdit>();
            //componentMapper.Register<Blazorise.SelectEdit, Bootstrap.SelectEdit>();
            //componentMapper.Register<Blazorise.SimpleButton, Bootstrap.SimpleButton>();
            //componentMapper.Register<Blazorise.TextEdit, Bootstrap.TextEdit>();

            serviceCollection.AddSingleton<IComponentMapper>( componentMapper );

            return serviceCollection;
        }

        [Obsolete( "AddMaterialClassProvider is deprecated, please use AddMaterialProviders instead." )]
        public static IServiceCollection AddMaterialClassProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, MaterialClassProvider>();
            serviceCollection.AddSingleton<IJSRunner, JSRunner>();

            return serviceCollection;
        }

        [Obsolete( "AddMaterialStyleProvider is deprecated, please use AddMaterialProviders instead." )]
        public static IServiceCollection AddMaterialStyleProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IStyleProvider, MaterialStyleProvider>();

            return serviceCollection;
        }
    }
}
