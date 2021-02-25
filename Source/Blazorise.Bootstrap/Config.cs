#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            serviceCollection.AddScoped<IThemeGenerator, BootstrapThemeGenerator>();

            serviceCollection.AddBootstrapComponents();

            return serviceCollection;
        }

        public static IServiceCollection AddBootstrapComponents( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddTransient<Blazorise.Field, Bootstrap.Field>();
            serviceCollection.AddTransient<Blazorise.Addon, Bootstrap.Addon>();
            serviceCollection.AddTransient<Blazorise.BarToggler, Bootstrap.BarToggler>();
            serviceCollection.AddTransient<Blazorise.BarDropdown, Bootstrap.BarDropdown>();
            serviceCollection.AddTransient<Blazorise.CardTitle, Bootstrap.CardTitle>();
            serviceCollection.AddTransient<Blazorise.CardSubtitle, Bootstrap.CardSubtitle>();
            serviceCollection.AddTransient<Blazorise.Carousel, Bootstrap.Carousel>();
            serviceCollection.AddTransient<Blazorise.CloseButton, Bootstrap.CloseButton>();
            serviceCollection.AddTransient( typeof( Blazorise.Check<> ), typeof( Bootstrap.Check<> ) );
            serviceCollection.AddTransient<Blazorise.Field, Bootstrap.Field>();
            serviceCollection.AddTransient<Blazorise.FieldBody, Bootstrap.FieldBody>();
            serviceCollection.AddTransient<Blazorise.FileEdit, Bootstrap.FileEdit>();
            serviceCollection.AddTransient<Blazorise.Modal, Bootstrap.Modal>();
            serviceCollection.AddTransient<Blazorise.ModalContent, Bootstrap.ModalContent>();
            serviceCollection.AddTransient<Blazorise.Button, Bootstrap.Button>();
            serviceCollection.AddTransient( typeof( Blazorise.Radio<> ), typeof( Bootstrap.Radio<> ) );
            serviceCollection.AddTransient( typeof( Blazorise.Switch<> ), typeof( Bootstrap.Switch<> ) );
            serviceCollection.AddTransient<Blazorise.Step, Bootstrap.Step>();

            return serviceCollection;
        }
    }
}
