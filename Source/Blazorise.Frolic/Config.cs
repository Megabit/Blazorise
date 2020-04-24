#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Frolic
{
    public static class Config
    {
        public static IServiceCollection AddFrolicProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
        {
            var classProvider = new FrolicClassProvider();

            configureClassProvider?.Invoke( classProvider );

            serviceCollection.AddSingleton<IClassProvider>( classProvider );
            serviceCollection.AddSingleton<IStyleProvider, FrolicStyleProvider>();
            serviceCollection.AddScoped<IJSRunner, FrolicJSRunner>();
            serviceCollection.AddSingleton<IComponentMapper, ComponentMapper>();
            serviceCollection.AddSingleton<IThemeGenerator, FrolicThemeGenerator>();

            return serviceCollection;
        }

        private static void RegisterComponents( IComponentMapper componentMapper )
        {
            componentMapper.Register<Blazorise.Addon, Frolic.Addon>();
            componentMapper.Register<Blazorise.Bar, Frolic.Bar>();
            componentMapper.Register<Blazorise.BarBrand, Frolic.BarBrand>();
            componentMapper.Register<Blazorise.BarDropdownToggle, Frolic.BarDropdownToggle>();
            componentMapper.Register<Blazorise.BarEnd, Frolic.BarEnd>();
            componentMapper.Register<Blazorise.BarItem, Frolic.BarItem>();
            componentMapper.Register<Blazorise.BarMenu, Frolic.BarMenu>();
            componentMapper.Register<Blazorise.BarStart, Frolic.BarStart>();
            componentMapper.Register<Blazorise.BarToggler, Frolic.BarToggler>();
            componentMapper.Register<Blazorise.Breadcrumb, Frolic.Breadcrumb>();
            componentMapper.Register<Blazorise.CardText, Frolic.CardText>();
            componentMapper.Register<Blazorise.CardTitle, Frolic.CardTitle>();
            componentMapper.Register( typeof( Blazorise.Check<> ), typeof( Frolic.Check<> ) );
            componentMapper.Register( typeof( Blazorise.Radio<> ), typeof( Frolic.Radio<> ) );
            componentMapper.Register<Blazorise.DisplayHeading, Frolic.DisplayHeading>();
            componentMapper.Register<Blazorise.Dropdown, Frolic.Dropdown>();
            componentMapper.Register<Blazorise.DropdownToggle, Frolic.DropdownToggle>();
            componentMapper.Register<Blazorise.Field, Frolic.Field>();
            componentMapper.Register<Blazorise.Heading, Frolic.Heading>();
            componentMapper.Register<Blazorise.Jumbotron, Frolic.Jumbotron>();
            componentMapper.Register<Blazorise.JumbotronSubtitle, Frolic.JumbotronSubtitle>();
            componentMapper.Register<Blazorise.Pagination, Frolic.Pagination>();
            componentMapper.Register<Blazorise.ProgressBar, Frolic.ProgressBar>();
            componentMapper.Register<Blazorise.Progress, Frolic.Progress>();
            componentMapper.Register<Blazorise.Button, Frolic.Button>();
            componentMapper.Register<Blazorise.Tabs, Frolic.Tabs>();
        }

        public static IServiceProvider UseFrolicProviders( this IServiceProvider serviceProvider )
        {
            var componentMapper = serviceProvider.GetRequiredService<IComponentMapper>();

            RegisterComponents( componentMapper );

            return serviceProvider;
        }
    }
}
