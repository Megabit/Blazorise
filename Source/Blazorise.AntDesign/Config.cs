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
            serviceCollection.AddSingleton<IComponentMapper, ComponentMapper>();
            serviceCollection.AddScoped<IThemeGenerator, AntDesignThemeGenerator>();

            return serviceCollection;
        }

        private static void RegisterComponents( IComponentMapper componentMapper )
        {
            componentMapper.Register<Blazorise.Addons, AntDesign.Addons>();
            componentMapper.Register<Blazorise.Addon, AntDesign.Addon>();
            componentMapper.Register<Blazorise.AddonLabel, AntDesign.AddonLabel>();
            componentMapper.Register<Blazorise.AlertMessage, AntDesign.AlertMessage>();
            componentMapper.Register<Blazorise.AlertDescription, AntDesign.AlertDescription>();
            componentMapper.Register<Blazorise.Bar, AntDesign.Bar>();
            componentMapper.Register<Blazorise.BarBrand, AntDesign.BarBrand>();
            componentMapper.Register<Blazorise.BarItem, AntDesign.BarItem>();
            componentMapper.Register<Blazorise.BarMenu, AntDesign.BarMenu>();
            componentMapper.Register<Blazorise.BarStart, AntDesign.BarStart>();
            componentMapper.Register<Blazorise.BarEnd, AntDesign.BarEnd>();
            componentMapper.Register<Blazorise.BarDropdown, AntDesign.BarDropdown>();
            componentMapper.Register<Blazorise.BarDropdownMenu, AntDesign.BarDropdownMenu>();
            componentMapper.Register<Blazorise.BarDropdownItem, AntDesign.BarDropdownItem>();
            componentMapper.Register<Blazorise.BarDropdownToggle, AntDesign.BarDropdownToggle>();
            componentMapper.Register<Blazorise.BarToggler, AntDesign.BarToggler>();
            componentMapper.Register<Blazorise.Breadcrumb, AntDesign.Breadcrumb>();
            componentMapper.Register<Blazorise.BreadcrumbItem, AntDesign.BreadcrumbItem>();
            componentMapper.Register<Blazorise.BreadcrumbLink, AntDesign.BreadcrumbLink>();
            componentMapper.Register( typeof( Blazorise.Check<> ), typeof( AntDesign.Check<> ) );
            componentMapper.Register<Blazorise.Button, AntDesign.Button>();
            componentMapper.Register<Blazorise.CardHeader, AntDesign.CardHeader>();
            componentMapper.Register<Blazorise.CardLink, AntDesign.CardLink>();
            componentMapper.Register<Blazorise.CloseButton, AntDesign.CloseButton>();
            componentMapper.Register<Blazorise.CollapseHeader, AntDesign.CollapseHeader>();
            componentMapper.Register<Blazorise.Dropdown, AntDesign.Dropdown>();
            componentMapper.Register<Blazorise.DropdownMenu, AntDesign.DropdownMenu>();
            componentMapper.Register<Blazorise.DropdownItem, AntDesign.DropdownItem>();
            componentMapper.Register<Blazorise.DropdownToggle, AntDesign.DropdownToggle>();
            componentMapper.Register<Blazorise.Field, AntDesign.Field>();
            componentMapper.Register<Blazorise.FieldBody, AntDesign.FieldBody>();
            componentMapper.Register<Blazorise.FieldLabel, AntDesign.FieldLabel>();
            componentMapper.Register<Blazorise.FileEdit, AntDesign.FileEdit>();
            componentMapper.Register<Blazorise.Heading, AntDesign.Heading>();
            componentMapper.Register<Blazorise.ListGroup, AntDesign.ListGroup>();
            componentMapper.Register<Blazorise.ModalBackdrop, AntDesign.ModalBackdrop>();
            componentMapper.Register<Blazorise.ModalContent, AntDesign.ModalContent>();
            componentMapper.Register<Blazorise.Progress, AntDesign.Progress>();
            componentMapper.Register( typeof( Blazorise.Select<> ), typeof( AntDesign.Select<> ) );
            componentMapper.Register( typeof( Blazorise.SelectItem<> ), typeof( AntDesign.SelectItem<> ) );
            componentMapper.Register<Blazorise.SelectGroup, AntDesign.SelectGroup>();
            componentMapper.Register( typeof( Blazorise.Radio<> ), typeof( AntDesign.Radio<> ) );
            componentMapper.Register( typeof( Blazorise.Slider<> ), typeof( AntDesign.Slider<> ) );
            componentMapper.Register( typeof( Blazorise.Switch<> ), typeof( AntDesign.Switch<> ) );
            componentMapper.Register<Blazorise.Tabs, AntDesign.Tabs>();
            componentMapper.Register<Blazorise.Tab, AntDesign.Tab>();
            componentMapper.Register<Blazorise.TabPanel, AntDesign.TabPanel>();
            componentMapper.Register<Blazorise.TabsContent, AntDesign.TabsContent>();
            componentMapper.Register<Blazorise.Table, AntDesign.Table>();
            componentMapper.Register<Blazorise.TableRowHeader, AntDesign.TableRowHeader>();
            componentMapper.Register<Blazorise.TextEdit, AntDesign.TextEdit>();
        }

        /// <summary>
        /// Registers the custom rules for ant design components.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IServiceProvider UseAntDesignProviders( this IServiceProvider serviceProvider )
        {
            var componentMapper = serviceProvider.GetRequiredService<IComponentMapper>();

            RegisterComponents( componentMapper );

            return serviceProvider;
        }
    }
}
