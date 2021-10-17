#region Using directives
using System;
using Blazorise.Bootstrap;
using Blazorise.Localization;
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
            services.AddSingleton<IJSRunner>( new BootstrapJSRunner( new Mock<IJSRuntime>().Object ) );
            services.AddSingleton<IThemeGenerator>( new BootstrapThemeGenerator( new Mock<IThemeCache>().Object ) );
            services.AddSingleton<IIconProvider>( new Mock<IIconProvider>().Object );
            services.AddSingleton<IValidationHandlerFactory, ValidationHandlerFactory>();
            services.AddSingleton<ValidatorValidationHandler>();
            services.AddSingleton<PatternValidationHandler>();
            services.AddSingleton<DataAnnotationValidationHandler>();
            services.AddSingleton<IDateTimeFormatConverter, DateTimeFormatConverter>();
            services.AddScoped<ITextLocalizerService, TextLocalizerService>();
            services.AddScoped( typeof( ITextLocalizer<> ), typeof( TextLocalizer<> ) );
            services.AddTransient(typeof ( Check<>), typeof (Blazorise.Bootstrap.Check<>) );

            Action<BlazoriseOptions> configureOptions = ( options ) =>
            {
            };

            services.AddSingleton( configureOptions );
            services.AddSingleton<BlazoriseOptions>();
        }

        public static void AddBootstrap5Providers( TestServiceProvider services )
        {
            services.AddSingleton<IIdGenerator>( new IdGenerator() );
            services.AddSingleton<IEditContextValidator>( new EditContextValidator( new ValidationMessageLocalizerAttributeFinder() ) );
            services.AddSingleton<IClassProvider>( new Blazorise.Bootstrap5.BootstrapClassProvider() );
            services.AddSingleton<IStyleProvider>( new Blazorise.Bootstrap5.Bootstrap5StyleProvider() );
            services.AddSingleton<IJSRunner>( new Blazorise.Bootstrap5.BootstrapJSRunner( new Mock<IJSRuntime>().Object ) );
            services.AddSingleton<IThemeGenerator>( new Blazorise.Bootstrap5.BootstrapThemeGenerator( new Mock<IThemeCache>().Object ) );
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
        }
    }
}
