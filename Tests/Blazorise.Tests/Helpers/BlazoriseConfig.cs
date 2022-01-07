#region Using directives
using System;
using Blazorise.Bootstrap;
using Blazorise.DataGrid;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Providers;
using Blazorise.Utilities;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
#endregion

namespace Blazorise.Tests.Helpers
{
    public static class BlazoriseConfig
    {
        public static void AddBootstrapProviders( TestServiceProvider services )
        {
            services.AddSingleton<IIdGenerator>( new IdGenerator() );
            services.AddSingleton<IEditContextValidator>( new EditContextValidator( new ValidationMessageLocalizerAttributeFinder() ) );
            services.AddSingleton<IClassProvider>( new BootstrapClassProvider() );
            services.AddSingleton<IStyleProvider>( new BootstrapStyleProvider() );
            services.AddSingleton<IThemeGenerator>( new BootstrapThemeGenerator( new Mock<IThemeCache>().Object ) );
            services.AddSingleton<IIconProvider>( new Mock<IIconProvider>().Object );
            services.AddSingleton<IValidationHandlerFactory, ValidationHandlerFactory>();
            services.AddSingleton<ValidatorValidationHandler>();
            services.AddSingleton<PatternValidationHandler>();
            services.AddSingleton<DataAnnotationValidationHandler>();
            services.AddSingleton<IDateTimeFormatConverter, DateTimeFormatConverter>();
            services.AddSingleton<IVersionProvider, VersionProvider>();
            services.AddScoped<ITextLocalizerService, TextLocalizerService>();
            services.AddScoped( typeof( ITextLocalizer<> ), typeof( TextLocalizer<> ) );

            Action<BlazoriseOptions> configureOptions = ( options ) =>
            {
            };

            services.AddSingleton( configureOptions );
            services.AddSingleton<BlazoriseOptions>();


            services.AddScoped<IJSUtilitiesModule, JSUtilitiesModule>();
            services.AddScoped<IJSButtonModule, JSButtonModule>();
            services.AddScoped<IJSClosableModule, JSClosableModule>();
            services.AddScoped<IJSBreakpointModule, JSBreakpointModule>();
            services.AddScoped<IJSTextEditModule, JSTextEditModule>();
            services.AddScoped<IJSMemoEditModule, JSMemoEditModule>();
            services.AddScoped<IJSNumericPickerModule, JSNumericPickerModule>();
            services.AddScoped<IJSDatePickerModule, JSDatePickerModule>();
            services.AddScoped<IJSTimePickerModule, JSTimePickerModule>();
            services.AddScoped<IJSColorPickerModule, JSColorPickerModule>();
            services.AddScoped<IJSFileEditModule, JSFileEditModule>();
            services.AddScoped<IJSTableModule, JSTableModule>();
            services.AddScoped<IJSSelectModule, JSSelectModule>();
            services.AddScoped<IJSInputMaskModule, JSInputMaskModule>();

            services.AddScoped<IJSModalModule, Bootstrap.Modules.BootstrapJSModalModule>();
            services.AddScoped<IJSTooltipModule, Bootstrap.Modules.BootstrapJSTooltipModule>();

            services.AddMemoryCache();
            services.AddScoped<Blazorise.Shared.Data.EmployeeData>();
            services.AddScoped<Blazorise.Shared.Data.CountryData>();
        }

        internal class VersionProvider : IVersionProvider
        {
            public string Version => "";

            public string MilestoneVersion => "";
        }

        public static class JSInterop
        {
            public static void AddButton( BunitJSInterop jsInterop )
            {
                AddUtilities( jsInterop );

                var module = jsInterop.SetupModule( new JSButtonModule( jsInterop.JSRuntime, new VersionProvider() ).ModuleFileName );
                module.SetupVoid( "import", _ => true );
                module.SetupVoid( "initialize", _ => true );
                module.SetupVoid( "destroy", _ => true );
            }

            public static void AddTextEdit( BunitJSInterop jsInterop )
            {
                AddUtilities( jsInterop );

                var module = jsInterop.SetupModule( new JSTextEditModule( jsInterop.JSRuntime, new VersionProvider() ).ModuleFileName );
                module.SetupVoid( "import", _ => true );
                module.SetupVoid( "initialize", _ => true );
                module.SetupVoid( "destroy", _ => true );
            }

            public static void AddDatePicker( BunitJSInterop jsInterop )
            {
                AddUtilities( jsInterop );

                jsInterop.SetupModule( new JSDatePickerModule( jsInterop.JSRuntime, new VersionProvider() ).ModuleFileName )
                         .SetupVoid( "initialize", _ => true );
            }

            public static void AddClosable( BunitJSInterop jsInterop )
            {
                AddUtilities( jsInterop );

                var module = jsInterop.SetupModule( new JSClosableModule( jsInterop.JSRuntime, new VersionProvider() ).ModuleFileName );
                module.SetupVoid( "import", _ => true );
                module.SetupVoid( "registerClosableComponent", _ => true );
                module.SetupVoid( "unregisterClosableComponent", _ => true );
            }

            public static void AddNumericEdit( BunitJSInterop jsInterop )
            {
                AddUtilities( jsInterop );

                var module = jsInterop.SetupModule( new JSNumericPickerModule( jsInterop.JSRuntime, new VersionProvider() ).ModuleFileName );
                module.SetupVoid( "import", _ => true );
                module.SetupVoid( "initialize", _ => true );
                module.SetupVoid( "destroy", _ => true );
            }

            public static void AddSelect( BunitJSInterop jsInterop )
            {
                AddUtilities( jsInterop );

                var module = jsInterop.SetupModule( new JSSelectModule( jsInterop.JSRuntime, new VersionProvider() ).ModuleFileName );
                module.SetupVoid( "import", _ => true );
                module.Setup<String[]>( "getSelectedOptions", _ => true );
            }

            public static void AddUtilities( BunitJSInterop jsInterop )
            {
                var module = jsInterop.SetupModule( new JSUtilitiesModule( jsInterop.JSRuntime, new VersionProvider() ).ModuleFileName );
                module.SetupVoid( "import", _ => true );
                module.SetupVoid( "setProperty", _ => true );
                module.Setup<string>( "getUserAgent", _ => true );
            }

            public static void AddModal( BunitJSInterop jsInterop )
            {
                AddUtilities( jsInterop );

                var module = jsInterop.SetupModule( new MockJsModule( jsInterop.JSRuntime, new VersionProvider() ).ModuleFileName );
                module.SetupVoid( "import", _ => true );
                module.SetupVoid( "open", _ => true );
                module.SetupVoid( "close", _ => true );
            }

            public static void AddTable( BunitJSInterop jsInterop )
            {
                AddUtilities( jsInterop );

                var module = jsInterop.SetupModule( new JSTableModule( jsInterop.JSRuntime, new VersionProvider() ).ModuleFileName );
                module.SetupVoid( "initializeTableFixedHeader", _ => true );
                module.SetupVoid( "destroyTableFixedHeader", _ => true );
                module.SetupVoid( "fixedHeaderScrollTableToPixels", _ => true );
                module.SetupVoid( "fixedHeaderScrollTableToRow", _ => true );
                module.SetupVoid( "initializeResizable", _ => true );
                module.SetupVoid( "destroyResizable", _ => true );
            }

            public static void AddDataGrid( BunitJSInterop jsInterop )
            {
                AddButton( jsInterop );
                AddTextEdit( jsInterop );
                AddModal( jsInterop );
                AddTable( jsInterop );
                AddSelect( jsInterop );
                AddClosable( jsInterop );

                var module = jsInterop.SetupModule( new JSDataGridModule( jsInterop.JSRuntime, new VersionProvider() ).ModuleFileName );
                module.SetupVoid( "initialize", _ => true );
                module.SetupVoid( "scrollTo", _ => true );
            }
        }

        public class MockJsModule : JSModalModule
        {
            public MockJsModule( IJSRuntime jsRuntime, IVersionProvider versionProvider ) : base( jsRuntime, versionProvider )
            {
            }

            public override string ModuleFileName => $"./_content/Blazorise.Bootstrap/modal.js?v={VersionProvider.Version}";
        }
    }
}
