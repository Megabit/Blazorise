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
            serviceCollection.AddSingleton<IThemeGenerator, FrolicThemeGenerator>();

            serviceCollection.AddTransient<Blazorise.Addon, Frolic.Addon>();
            serviceCollection.AddTransient<Blazorise.BarBrand, Frolic.BarBrand>();
            serviceCollection.AddTransient<Blazorise.BarDropdownToggle, Frolic.BarDropdownToggle>();
            serviceCollection.AddTransient<Blazorise.BarEnd, Frolic.BarEnd>();
            serviceCollection.AddTransient<Blazorise.BarItem, Frolic.BarItem>();
            serviceCollection.AddTransient<Blazorise.BarStart, Frolic.BarStart>();
            serviceCollection.AddTransient<Blazorise.BarToggler, Frolic.BarToggler>();
            serviceCollection.AddTransient<Blazorise.Breadcrumb, Frolic.Breadcrumb>();
            serviceCollection.AddTransient<Blazorise.CardText, Frolic.CardText>();
            serviceCollection.AddTransient<Blazorise.CardTitle, Frolic.CardTitle>();
            serviceCollection.AddTransient<Blazorise.Carousel, Frolic.Carousel>();
            serviceCollection.AddTransient( typeof( Blazorise.Check<> ), typeof( Frolic.Check<> ) );
            serviceCollection.AddTransient( typeof( Blazorise.Radio<> ), typeof( Frolic.Radio<> ) );
            serviceCollection.AddTransient( typeof( Blazorise.Switch<> ), typeof( Frolic.Switch<> ) );
            serviceCollection.AddTransient<Blazorise.DisplayHeading, Frolic.DisplayHeading>();
            serviceCollection.AddTransient<Blazorise.Dropdown, Frolic.Dropdown>();
            serviceCollection.AddTransient<Blazorise.DropdownToggle, Frolic.DropdownToggle>();
            serviceCollection.AddTransient<Blazorise.Field, Frolic.Field>();
            serviceCollection.AddTransient<Blazorise.Jumbotron, Frolic.Jumbotron>();
            serviceCollection.AddTransient<Blazorise.JumbotronSubtitle, Frolic.JumbotronSubtitle>();
            serviceCollection.AddTransient<Blazorise.Pagination, Frolic.Pagination>();
            serviceCollection.AddTransient<Blazorise.ProgressBar, Frolic.ProgressBar>();
            serviceCollection.AddTransient<Blazorise.Progress, Frolic.Progress>();
            serviceCollection.AddTransient<Blazorise.Button, Frolic.Button>();
            serviceCollection.AddTransient<Blazorise.Tabs, Frolic.Tabs>();
            serviceCollection.AddTransient<Blazorise.Steps, Frolic.Steps>();
            serviceCollection.AddTransient<Blazorise.Step, Frolic.Step>();

            return serviceCollection;
        }
    }
}
