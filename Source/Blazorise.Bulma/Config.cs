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
        public static IServiceCollection AddBulmaProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
        {
            var classProvider = new BulmaClassProvider();

            configureClassProvider?.Invoke( classProvider );

            serviceCollection.AddSingleton<IClassProvider>( classProvider );
            serviceCollection.AddSingleton<IStyleProvider, BulmaStyleProvider>();
            serviceCollection.AddScoped<IJSRunner, BulmaJSRunner>();
            serviceCollection.AddSingleton<IThemeGenerator, BulmaThemeGenerator>();

            serviceCollection.AddTransient<Blazorise.Addons, Bulma.Addons>();
            serviceCollection.AddTransient<Blazorise.Badge, Bulma.Badge>();
            serviceCollection.AddTransient<Blazorise.BarToggler, Bulma.BarToggler>();
            serviceCollection.AddTransient<Blazorise.BarDropdown, Bulma.BarDropdown>();
            serviceCollection.AddTransient<Blazorise.Breadcrumb, Bulma.Breadcrumb>();
            serviceCollection.AddTransient<Blazorise.BreadcrumbLink, Bulma.BreadcrumbLink>();
            serviceCollection.AddTransient<Blazorise.CardImage, Bulma.CardImage>();
            serviceCollection.AddTransient<Blazorise.CardTitle, Bulma.CardTitle>();
            serviceCollection.AddTransient<Blazorise.CardSubtitle, Bulma.CardSubtitle>();
            serviceCollection.AddTransient<Blazorise.Carousel, Bulma.Carousel>();
            serviceCollection.AddTransient( typeof( Blazorise.Check<> ), typeof( Bulma.Check<> ) );
            serviceCollection.AddTransient( typeof( Blazorise.DateEdit<> ), typeof( Bulma.DateEdit<> ) );
            serviceCollection.AddTransient<Blazorise.DropdownDivider, Bulma.DropdownDivider>();
            serviceCollection.AddTransient<Blazorise.DropdownMenu, Bulma.DropdownMenu>();
            serviceCollection.AddTransient<Blazorise.DropdownToggle, Bulma.DropdownToggle>();
            serviceCollection.AddTransient<Blazorise.Field, Bulma.Field>();
            serviceCollection.AddTransient<Blazorise.FieldLabel, Bulma.FieldLabel>();
            serviceCollection.AddTransient<Blazorise.FieldHelp, Bulma.FieldHelp>();
            serviceCollection.AddTransient<Blazorise.FieldBody, Bulma.FieldBody>();
            serviceCollection.AddTransient<Blazorise.Fields, Bulma.Fields>();
            serviceCollection.AddTransient<Blazorise.FileEdit, Bulma.FileEdit>();
            serviceCollection.AddTransient<Blazorise.Heading, Bulma.Heading>();
            serviceCollection.AddTransient<Blazorise.Jumbotron, Bulma.Jumbotron>();
            serviceCollection.AddTransient<Blazorise.JumbotronSubtitle, Bulma.JumbotronSubtitle>();
            serviceCollection.AddTransient( typeof( Blazorise.Radio<> ), typeof( Bulma.Radio<> ) );
            serviceCollection.AddTransient( typeof( Blazorise.Select<> ), typeof( Bulma.Select<> ) );
            serviceCollection.AddTransient( typeof( Blazorise.Switch<> ), typeof( Bulma.Switch<> ) );
            serviceCollection.AddTransient<Blazorise.Button, Bulma.Button>();
            serviceCollection.AddTransient<Blazorise.Table, Bulma.Table>();
            serviceCollection.AddTransient<Blazorise.Tabs, Bulma.Tabs>();
            serviceCollection.AddTransient<Blazorise.TextEdit, Bulma.TextEdit>();
            serviceCollection.AddTransient( typeof( Blazorise.TimeEdit<> ), typeof( Bulma.TimeEdit<> ) );
            serviceCollection.AddTransient( typeof( Blazorise.NumericEdit<> ), typeof( Bulma.NumericEdit<> ) );
            serviceCollection.AddTransient<Blazorise.Pagination, Bulma.Pagination>();
            serviceCollection.AddTransient<Blazorise.PaginationLink, Bulma.PaginationLink>();
            serviceCollection.AddTransient<Blazorise.Steps, Bulma.Steps>();
            serviceCollection.AddTransient<Blazorise.Step, Bulma.Step>();

            return serviceCollection;
        }
    }
}
