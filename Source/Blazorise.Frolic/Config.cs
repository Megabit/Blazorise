#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if NETSTANDARD2_0
using Microsoft.AspNetCore.Components.Builder;
#elif NETCORE3_0
using Microsoft.AspNetCore.Builder;
#endif
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
            componentMapper.Register<Blazorise.Breadcrumb, Frolic.Breadcrumb>();
            componentMapper.Register<Blazorise.CardText, Frolic.CardText>();
            componentMapper.Register<Blazorise.CardTitle, Frolic.CardTitle>();
            componentMapper.Register<Blazorise.CheckEdit, Frolic.CheckEdit>();
            componentMapper.Register<Blazorise.Dropdown, Frolic.Dropdown>();
            componentMapper.Register<Blazorise.DropdownToggle, Frolic.DropdownToggle>();
            componentMapper.Register<Blazorise.Field, Frolic.Field>();
            componentMapper.Register<Blazorise.Heading, Frolic.Heading>();
            componentMapper.Register<Blazorise.Pagination, Frolic.Pagination>();
            componentMapper.Register<Blazorise.ProgressBar, Frolic.ProgressBar>();
            componentMapper.Register<Blazorise.ProgressGroup, Frolic.ProgressGroup>();
            componentMapper.Register<Blazorise.SimpleButton, Frolic.SimpleButton>();
            componentMapper.Register<Blazorise.Tabs, Frolic.Tabs>();
        }

#if NETSTANDARD2_0

        public static IComponentsApplicationBuilder UseFrolicProviders( this IComponentsApplicationBuilder app )
        {
            var componentMapper = app.Services.GetRequiredService<IComponentMapper>();

            RegisterComponents( componentMapper );

            return app;
        }

#elif NETCORE3_0

        public static IApplicationBuilder UseFrolicProviders( this IApplicationBuilder app )
        {
            var componentMapper = app.ApplicationServices.GetRequiredService<IComponentMapper>();

            RegisterComponents( componentMapper );

            return app;
        }

#endif
    }
}
