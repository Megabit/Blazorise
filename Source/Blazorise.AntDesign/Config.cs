#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.AntDesign
{
    public static class Config
    {
        /// <summary>
        /// Adds a ant design providers and component mappings.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddAntDesignProviders( this IServiceCollection serviceCollection, Action<IClassProvider> configureClassProvider = null )
        {
            var classProvider = new AntDesignClassProvider();

            configureClassProvider?.Invoke( classProvider );

            serviceCollection.AddSingleton<IClassProvider>( classProvider );
            serviceCollection.AddSingleton<IStyleProvider, AntDesignStyleProvider>();
            serviceCollection.AddScoped<IJSRunner, AntDesignJSRunner>();
            serviceCollection.AddScoped<IThemeGenerator, AntDesignThemeGenerator>();

            serviceCollection.AddTransient<Blazorise.Addons, AntDesign.Addons>();
            serviceCollection.AddTransient<Blazorise.Addon, AntDesign.Addon>();
            serviceCollection.AddTransient<Blazorise.AddonLabel, AntDesign.AddonLabel>();
            serviceCollection.AddTransient<Blazorise.AlertMessage, AntDesign.AlertMessage>();
            serviceCollection.AddTransient<Blazorise.AlertDescription, AntDesign.AlertDescription>();
            serviceCollection.AddTransient<Blazorise.Badge, AntDesign.Badge>();
            serviceCollection.AddTransient<Blazorise.Bar, AntDesign.Bar>();
            serviceCollection.AddTransient<Blazorise.BarBrand, AntDesign.BarBrand>();
            serviceCollection.AddTransient<Blazorise.BarIcon, AntDesign.BarIcon>();
            serviceCollection.AddTransient<Blazorise.BarItem, AntDesign.BarItem>();
            serviceCollection.AddTransient<Blazorise.BarMenu, AntDesign.BarMenu>();
            serviceCollection.AddTransient<Blazorise.BarStart, AntDesign.BarStart>();
            serviceCollection.AddTransient<Blazorise.BarEnd, AntDesign.BarEnd>();
            serviceCollection.AddTransient<Blazorise.BarDropdown, AntDesign.BarDropdown>();
            serviceCollection.AddTransient<Blazorise.BarLink, AntDesign.BarLink>();
            serviceCollection.AddTransient<Blazorise.BarDropdownMenu, AntDesign.BarDropdownMenu>();
            serviceCollection.AddTransient<Blazorise.BarDropdownItem, AntDesign.BarDropdownItem>();
            serviceCollection.AddTransient<Blazorise.BarDropdownToggle, AntDesign.BarDropdownToggle>();
            serviceCollection.AddTransient<Blazorise.BarToggler, AntDesign.BarToggler>();
            serviceCollection.AddTransient<Blazorise.Breadcrumb, AntDesign.Breadcrumb>();
            serviceCollection.AddTransient<Blazorise.BreadcrumbItem, AntDesign.BreadcrumbItem>();
            serviceCollection.AddTransient<Blazorise.BreadcrumbLink, AntDesign.BreadcrumbLink>();
            serviceCollection.AddTransient( typeof( Blazorise.Check<> ), typeof( AntDesign.Check<> ) );
            serviceCollection.AddTransient<Blazorise.Button, AntDesign.Button>();
            serviceCollection.AddTransient<Blazorise.CardHeader, AntDesign.CardHeader>();
            serviceCollection.AddTransient<Blazorise.CardLink, AntDesign.CardLink>();
            serviceCollection.AddTransient<Blazorise.Carousel, AntDesign.Carousel>();
            serviceCollection.AddTransient<Blazorise.CloseButton, AntDesign.CloseButton>();
            serviceCollection.AddTransient<Blazorise.CollapseHeader, AntDesign.CollapseHeader>();
            serviceCollection.AddTransient<Blazorise.Dropdown, AntDesign.Dropdown>();
            serviceCollection.AddTransient<Blazorise.DropdownMenu, AntDesign.DropdownMenu>();
            serviceCollection.AddTransient<Blazorise.DropdownItem, AntDesign.DropdownItem>();
            serviceCollection.AddTransient<Blazorise.DropdownToggle, AntDesign.DropdownToggle>();
            serviceCollection.AddTransient<Blazorise.Field, AntDesign.Field>();
            serviceCollection.AddTransient<Blazorise.FieldBody, AntDesign.FieldBody>();
            serviceCollection.AddTransient<Blazorise.FieldLabel, AntDesign.FieldLabel>();
            serviceCollection.AddTransient<Blazorise.FileEdit, AntDesign.FileEdit>();
            serviceCollection.AddTransient<Blazorise.ListGroup, AntDesign.ListGroup>();
            serviceCollection.AddTransient<Blazorise.ModalContent, AntDesign.ModalContent>();
            serviceCollection.AddTransient<Blazorise.Progress, AntDesign.Progress>();
            serviceCollection.AddTransient( typeof( Blazorise.Select<> ), typeof( AntDesign.Select<> ) );
            serviceCollection.AddTransient( typeof( Blazorise.SelectItem<> ), typeof( AntDesign.SelectItem<> ) );
            serviceCollection.AddTransient<Blazorise.SelectGroup, AntDesign.SelectGroup>();
            serviceCollection.AddTransient( typeof( Blazorise.Radio<> ), typeof( AntDesign.Radio<> ) );
            serviceCollection.AddTransient( typeof( Blazorise.Slider<> ), typeof( AntDesign.Slider<> ) );
            serviceCollection.AddTransient( typeof( Blazorise.Switch<> ), typeof( AntDesign.Switch<> ) );
            serviceCollection.AddTransient<Blazorise.Tabs, AntDesign.Tabs>();
            serviceCollection.AddTransient<Blazorise.Tab, AntDesign.Tab>();
            serviceCollection.AddTransient<Blazorise.TabPanel, AntDesign.TabPanel>();
            serviceCollection.AddTransient<Blazorise.TabsContent, AntDesign.TabsContent>();
            serviceCollection.AddTransient<Blazorise.Table, AntDesign.Table>();
            serviceCollection.AddTransient<Blazorise.TableRowHeader, AntDesign.TableRowHeader>();
            serviceCollection.AddTransient<Blazorise.TextEdit, AntDesign.TextEdit>();
            serviceCollection.AddTransient<Blazorise.Step, AntDesign.Step>();

            return serviceCollection;
        }
    }
}
