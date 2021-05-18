#region Using directives
using System;
using System.Collections.Generic;
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
            serviceCollection.AddSingleton<IThemeGenerator, FrolicThemeGenerator>();

            foreach ( var mapping in ComponentMap )
            {
                serviceCollection.AddTransient( mapping.Key, mapping.Value );
            }

            return serviceCollection;
        }

        public static IDictionary<Type, Type> ComponentMap => new Dictionary<Type, Type>
        {
            { typeof( Blazorise.Addon ), typeof( Frolic.Addon ) },
            { typeof( Blazorise.BarBrand ), typeof( Frolic.BarBrand ) },
            { typeof( Blazorise.BarDropdownToggle ), typeof( Frolic.BarDropdownToggle ) },
            { typeof( Blazorise.BarEnd ), typeof( Frolic.BarEnd ) },
            { typeof( Blazorise.BarItem ), typeof( Frolic.BarItem ) },
            { typeof( Blazorise.BarStart ), typeof( Frolic.BarStart ) },
            { typeof( Blazorise.BarToggler ), typeof( Frolic.BarToggler ) },
            { typeof( Blazorise.Breadcrumb ), typeof( Frolic.Breadcrumb ) },
            { typeof( Blazorise.CardText ), typeof( Frolic.CardText ) },
            { typeof( Blazorise.CardTitle ), typeof( Frolic.CardTitle ) },
            { typeof( Blazorise.Carousel ), typeof( Frolic.Carousel ) },
            { typeof( Blazorise.Check<> ), typeof( Frolic.Check<> ) },
            { typeof( Blazorise.Radio<> ), typeof( Frolic.Radio<> ) },
            { typeof( Blazorise.Switch<> ), typeof( Frolic.Switch<> ) },
            { typeof( Blazorise.DisplayHeading ), typeof( Frolic.DisplayHeading ) },
            { typeof( Blazorise.Dropdown ), typeof( Frolic.Dropdown ) },
            { typeof( Blazorise.DropdownToggle ), typeof( Frolic.DropdownToggle ) },
            { typeof( Blazorise.Field ), typeof( Frolic.Field ) },
            { typeof( Blazorise.Jumbotron ), typeof( Frolic.Jumbotron ) },
            { typeof( Blazorise.JumbotronSubtitle ), typeof( Frolic.JumbotronSubtitle ) },
            { typeof( Blazorise.Pagination ), typeof( Frolic.Pagination ) },
            { typeof( Blazorise.ProgressBar ), typeof( Frolic.ProgressBar ) },
            { typeof( Blazorise.Progress ), typeof( Frolic.Progress ) },
            { typeof( Blazorise.Button ), typeof( Frolic.Button ) },
            { typeof( Blazorise.Tabs ), typeof( Frolic.Tabs ) },
            { typeof( Blazorise.Steps ), typeof( Frolic.Steps ) },
            { typeof( Blazorise.Step ), typeof( Frolic.Step ) },
        };
    }
}
