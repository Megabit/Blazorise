#region Using directives
using System;
using Blazorise.Bootstrap;
using Blazorise.Localization;
using Blazorise.Modules;
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
            services.AddScoped<IJSNumericEditModule, JSNumericEditModule>();
            services.AddScoped<IJSDatePickerModule, JSDatePickerModule>();
            services.AddScoped<IJSTimePickerModule, JSTimePickerModule>();
            services.AddScoped<IJSColorPickerModule, JSColorPickerModule>();
            services.AddScoped<IJSFileEditModule, JSFileEditModule>();
            services.AddScoped<IJSTableModule, JSTableModule>();
            services.AddScoped<IJSSelectModule, JSSelectModule>();

            services.AddScoped<IJSModalModule, Bootstrap.Modules.BootstrapJSModalModule>();
            services.AddScoped<IJSTooltipModule, Bootstrap.Modules.BootstrapJSTooltipModule>();
        }

        public static class JSInterop
        {
            public static void AddButton( BunitJSInterop jsInterop )
            {
                var module = jsInterop.SetupModule( new JSButtonModule( jsInterop.JSRuntime ).ModuleFileName );
                module.SetupVoid( "initialize", _ => true );
                module.SetupVoid( "destroy", _ => true );
            }

            public static void AddTextEdit( BunitJSInterop jsInterop )
            {
                var module = jsInterop.SetupModule( new JSTextEditModule( jsInterop.JSRuntime ).ModuleFileName );
                module.SetupVoid( "initialize", _ => true );
                module.SetupVoid( "destroy", _ => true );
            }

            public static void AddDatePicker( BunitJSInterop jsInterop )
            {
                jsInterop.SetupModule( new JSDatePickerModule( jsInterop.JSRuntime ).ModuleFileName )
                         .SetupVoid( "initialize", _ => true );
            }

            public static void AddCloseable( BunitJSInterop jsInterop )
            {
                var module = jsInterop.SetupModule( new JSClosableModule( jsInterop.JSRuntime ).ModuleFileName );
                module.SetupVoid( "registerClosableComponent", _ => true );
                module.SetupVoid( "unregisterClosableComponent", _ => true );
            }

            public static void AddNumericEdit( BunitJSInterop jsInterop )
            {
                var module = jsInterop.SetupModule( new JSNumericEditModule( jsInterop.JSRuntime ).ModuleFileName );
                module.SetupVoid( "initialize", _ => true );
                module.SetupVoid( "destroy", _ => true );
            }

            public static void AddSelect( BunitJSInterop jsInterop )
            {
                var module = jsInterop.SetupModule( new JSSelectModule( jsInterop.JSRuntime ).ModuleFileName );
                module.Setup<String[]>( "getSelectedOptions", _ => true );
            }
        }
    }
}
