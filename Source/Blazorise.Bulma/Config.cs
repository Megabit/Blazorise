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
        public static IServiceCollection AddBulmaProviders( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, BulmaClassProvider>();
            serviceCollection.AddSingleton<IStyleProvider, BulmaStyleProvider>();
            serviceCollection.AddSingleton<IJSRunner, JSRunner>();

            var componentMapper = new ComponentMapper();

            componentMapper.Register<Blazorise.Addons, Bulma.Addons>();
            componentMapper.Register<Blazorise.BarToggler, Bulma.BarToggler>();
            componentMapper.Register<Blazorise.CardImage, Bulma.CardImage>();
            componentMapper.Register<Blazorise.CardSubtitle, Bulma.CardSubtitle>();
            componentMapper.Register<Blazorise.CheckEdit, Bulma.CheckEdit>();
            componentMapper.Register<Blazorise.DropdownDivider, Bulma.DropdownDivider>();
            componentMapper.Register<Blazorise.DropdownMenu, Bulma.DropdownMenu>();
            componentMapper.Register<Blazorise.Field, Bulma.Field>();
            componentMapper.Register<Blazorise.FieldLabel, Bulma.FieldLabel>();
            componentMapper.Register<Blazorise.FieldBody, Bulma.FieldBody>();
            componentMapper.Register<Blazorise.Fields, Bulma.Fields>();
            componentMapper.Register<Blazorise.FileEdit, Bulma.FileEdit>();
            componentMapper.Register<Blazorise.SelectEdit, Bulma.SelectEdit>();
            componentMapper.Register<Blazorise.SimpleButton, Bulma.SimpleButton>();
            componentMapper.Register<Blazorise.Tabs, Bulma.Tabs>();

            serviceCollection.AddSingleton<IComponentMapper>( componentMapper );

            return serviceCollection;
        }

        [Obsolete( "AddBulmaClassProvider is deprecated, please use AddBulmaProviders instead." )]
        public static IServiceCollection AddBulmaClassProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, BulmaClassProvider>();
            serviceCollection.AddSingleton<IJSRunner, JSRunner>();

            return serviceCollection;
        }

        [Obsolete( "AddBulmaStyleProvider is deprecated, please use AddBulmaProviders instead." )]
        public static IServiceCollection AddBulmaStyleProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IStyleProvider, BulmaStyleProvider>();

            return serviceCollection;
        }
    }
}
