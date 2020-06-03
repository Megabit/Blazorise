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
        public static IServiceCollection AddBootstrapProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
        {
            var classProvider = new BootstrapClassProvider();

            configureClassProvider?.Invoke( classProvider );

            serviceCollection.AddSingleton<IClassProvider>( classProvider );
            serviceCollection.AddSingleton<IStyleProvider, BootstrapStyleProvider>();
            serviceCollection.AddScoped<IJSRunner, BootstrapJSRunner>();
            serviceCollection.AddSingleton<IComponentMapper, ComponentMapper>();
            serviceCollection.AddScoped<IThemeGenerator, BootstrapThemeGenerator>();

            return serviceCollection;
        }

        private static void RegisterComponents( IComponentMapper componentMapper )
        {
            componentMapper.Register<Blazorise.Addon, Bootstrap.Addon>();
            componentMapper.Register<Blazorise.BarToggler, Bootstrap.BarToggler>();
            componentMapper.Register<Blazorise.BarDropdown, Bootstrap.BarDropdown>();
            componentMapper.Register<Blazorise.CardTitle, Bootstrap.CardTitle>();
            componentMapper.Register<Blazorise.CardSubtitle, Bootstrap.CardSubtitle>();
            componentMapper.Register<Blazorise.Carousel, Bootstrap.Carousel>();
            componentMapper.Register<Blazorise.CloseButton, Bootstrap.CloseButton>();
            componentMapper.Register( typeof( Blazorise.Check<> ), typeof( Bootstrap.Check<> ) );
            componentMapper.Register<Blazorise.Field, Bootstrap.Field>();
            componentMapper.Register<Blazorise.FieldBody, Bootstrap.FieldBody>();
            componentMapper.Register<Blazorise.FileEdit, Bootstrap.FileEdit>();
            componentMapper.Register<Blazorise.ModalContent, Bootstrap.ModalContent>();
            componentMapper.Register<Blazorise.Button, Bootstrap.Button>();
            componentMapper.Register( typeof( Blazorise.Radio<> ), typeof( Bootstrap.Radio<> ) );
            componentMapper.Register( typeof( Blazorise.Switch<> ), typeof( Bootstrap.Switch<> ) );
        }

        /// <summary>
        /// Registers the custom rules for bootstrap components.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IServiceProvider UseBootstrapProviders( this IServiceProvider serviceProvider )
        {
            var componentMapper = serviceProvider.GetRequiredService<IComponentMapper>();

            RegisterComponents( componentMapper );

            return serviceProvider;
        }
    }
}
