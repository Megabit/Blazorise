#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Builder;
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
        public static IServiceCollection AddBootstrapProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
        {
            var classProvider = new BootstrapClassProvider();

            configureClassProvider?.Invoke( classProvider );

            serviceCollection.AddSingleton<IClassProvider>( classProvider );
            serviceCollection.AddSingleton<IStyleProvider, BootstrapStyleProvider>();
            serviceCollection.AddScoped<IJSRunner, BootstrapJSRunner>();
            serviceCollection.AddSingleton<IComponentMapper, ComponentMapper>();

            return serviceCollection;
        }

        /// <summary>
        /// Registers the custom rules for bootstrap components.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IComponentsApplicationBuilder UseBootstrapProviders( this IComponentsApplicationBuilder app )
        {
            var componentMapper = app.Services.GetRequiredService<IComponentMapper>();

            componentMapper.Register<Blazorise.Addon, Bootstrap.Addon>();
            //componentMapper.Register<Blazorise.Addons, Bootstrap.Addons>();
            componentMapper.Register<Blazorise.BarToggler, Bootstrap.BarToggler>();
            componentMapper.Register<Blazorise.BarDropdown, Bootstrap.BarDropdown>();
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

            return app;
        }
    }
}
