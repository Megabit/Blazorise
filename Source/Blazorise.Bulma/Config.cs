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

            var mapper = new ComponentMapper();

            mapper.Register<Blazorise.Addons, Addons>();
            mapper.Register<Blazorise.BarToggler, BarToggler>();
            mapper.Register<Blazorise.CardImage, CardImage>();
            mapper.Register<Blazorise.CardSubtitle, CardSubtitle>();
            mapper.Register<Blazorise.CheckEdit, CheckEdit>();
            mapper.Register<Blazorise.DropdownDivider, DropdownDivider>();
            mapper.Register<Blazorise.DropdownMenu, DropdownMenu>();
            mapper.Register<Blazorise.Field, Field>();
            mapper.Register<Blazorise.Fields, Fields>();
            mapper.Register<Blazorise.FileEdit, FileEdit>();
            mapper.Register<Blazorise.SelectEdit, SelectEdit>();
            mapper.Register<Blazorise.SimpleButton, SimpleButton>();
            mapper.Register<Blazorise.Tabs, Tabs>();

            serviceCollection.AddSingleton<IComponentMapper>( ( p ) => mapper );

            return serviceCollection;
        }

        [Obsolete( "AddBulmaClassProvider is deprecated, please use AddBulma instead." )]
        public static IServiceCollection AddBulmaClassProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, BulmaClassProvider>();
            serviceCollection.AddSingleton<IJSRunner, JSRunner>();

            return serviceCollection;
        }

        [Obsolete( "AddBulmaStyleProvider is deprecated, please use AddBulma instead." )]
        public static IServiceCollection AddBulmaStyleProvider( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IStyleProvider, BulmaStyleProvider>();

            return serviceCollection;
        }
    }
}
