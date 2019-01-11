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
            serviceCollection.AddSingleton<IComponentMapper, ComponentMapper>();

            return serviceCollection;
        }

        public static IServiceProvider UseMaterialProviders( this IServiceProvider serviceProvider )
        {
            var componentMapper = serviceProvider.GetRequiredService<IComponentMapper>();

            // same components as in bootstrap provider
            componentMapper.Register<Blazorise.Addon, Bootstrap.Addon>();
            //componentMapper.Register<Blazorise.Addons, Bootstrap.Addons>();
            componentMapper.Register<Blazorise.BarToggler, Bootstrap.BarToggler>();
            componentMapper.Register<Blazorise.CardSubtitle, Bootstrap.CardSubtitle>();
            componentMapper.Register<Blazorise.CloseButton, Bootstrap.CloseButton>();
            componentMapper.Register<Blazorise.CheckEdit, Bootstrap.CheckEdit>();
            //componentMapper.Register<Blazorise.DateEdit, Bootstrap.DateEdit>();
            componentMapper.Register<Blazorise.Field, Bootstrap.Field>();
            componentMapper.Register<Blazorise.FieldBody, Bootstrap.FieldBody>();
            componentMapper.Register<Blazorise.FileEdit, Bootstrap.FileEdit>();
            componentMapper.Register<Blazorise.ModalContent, Bootstrap.ModalContent>();
            //componentMapper.Register<Blazorise.MemoEdit, Bootstrap.MemoEdit>();
            //componentMapper.Register<Blazorise.SelectEdit, Bootstrap.SelectEdit>();
            componentMapper.Register<Blazorise.SimpleButton, Bootstrap.SimpleButton>();
            //componentMapper.Register<Blazorise.TextEdit, Bootstrap.TextEdit>();

            return serviceProvider;
        }
    }
}
