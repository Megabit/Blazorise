﻿#region Using directives
using System;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Providers;
using Blazorise.Themes;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Extension methods for building the blazorise options.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Register blazorise and configures the default behaviour.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddBlazorise( this IServiceCollection serviceCollection, Action<BlazoriseOptions> configureOptions = null )
        {
            serviceCollection.Replace( ServiceDescriptor.Transient<IComponentActivator, ComponentActivator>() );

            // If options handler is not defined we will get an exception so
            // we need to initialize and empty action.
            configureOptions ??= _ => { };

            serviceCollection.AddSingleton( configureOptions );
            serviceCollection.AddSingleton<BlazoriseOptions>();
            serviceCollection.AddSingleton<IVersionProvider, VersionProvider>();

            serviceCollection.AddSingleton<IIdGenerator, IdGenerator>();
            serviceCollection.AddSingleton<IThemeCache, ThemeCache>();
            serviceCollection.AddSingleton<IValidationMessageLocalizerAttributeFinder, ValidationMessageLocalizerAttributeFinder>();
            serviceCollection.AddScoped<IEditContextValidator, EditContextValidator>();

            serviceCollection.AddScoped<ITextLocalizerService, TextLocalizerService>();
            serviceCollection.AddScoped( typeof( ITextLocalizer<> ), typeof( TextLocalizer<> ) );

            serviceCollection.AddScoped<IValidationHandlerFactory, ValidationHandlerFactory>();
            serviceCollection.AddScoped<ValidatorValidationHandler>();
            serviceCollection.AddScoped<PatternValidationHandler>();
            serviceCollection.AddScoped<DataAnnotationValidationHandler>();
            serviceCollection.AddScoped<IMessageService, MessageService>();
            serviceCollection.AddScoped<INotificationService, NotificationService>();
            serviceCollection.AddScoped<IPageProgressService, PageProgressService>();

            serviceCollection.AddSingleton<IDateTimeFormatConverter, DateTimeFormatConverter>();

            serviceCollection.AddScoped<IJSUtilitiesModule, JSUtilitiesModule>();
            serviceCollection.AddScoped<IJSButtonModule, JSButtonModule>();
            serviceCollection.AddScoped<IJSClosableModule, JSClosableModule>();
            serviceCollection.AddScoped<IJSBreakpointModule, JSBreakpointModule>();
            serviceCollection.AddScoped<IJSTextEditModule, JSTextEditModule>();
            serviceCollection.AddScoped<IJSMemoEditModule, JSMemoEditModule>();
            serviceCollection.AddScoped<IJSNumericEditModule, JSNumericEditModule>();
            serviceCollection.AddScoped<IJSDatePickerModule, JSDatePickerModule>();
            serviceCollection.AddScoped<IJSTimePickerModule, JSTimePickerModule>();
            serviceCollection.AddScoped<IJSColorPickerModule, JSColorPickerModule>();
            serviceCollection.AddScoped<IJSFileEditModule, JSFileEditModule>();
            serviceCollection.AddScoped<IJSTableModule, JSTableModule>();
            serviceCollection.AddScoped<IJSSelectModule, JSSelectModule>();

            return serviceCollection;
        }

        /// <summary>
        /// Registers an empty providers.
        /// </summary>
        /// <remarks>
        /// Generally this should not be used, except when the user wants to use extensions without any providers like Bootstrap or Bulma.
        /// </remarks>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddEmptyProviders( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IClassProvider, EmptyClassProvider>();
            serviceCollection.AddSingleton<IStyleProvider, EmptyStyleProvider>();

            return serviceCollection;
        }

        /// <summary>
        /// Registers a custom class provider.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="classProviderFactory"></param>
        /// <returns></returns>
        public static IServiceCollection AddClassProvider( this IServiceCollection serviceCollection, Func<IClassProvider> classProviderFactory )
        {
            serviceCollection.AddSingleton( ( p ) => classProviderFactory() );

            return serviceCollection;
        }

        /// <summary>
        /// Registers a custom style provider.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="styleProviderFactory"></param>
        /// <returns></returns>
        public static IServiceCollection AddStyleProvider( this IServiceCollection serviceCollection, Func<IStyleProvider> styleProviderFactory )
        {
            serviceCollection.AddSingleton( ( p ) => styleProviderFactory() );

            return serviceCollection;
        }

        /// <summary>
        /// Registers a custom icon provider.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="iconProviderFactory"></param>
        public static IServiceCollection AddIconProvider( this IServiceCollection serviceCollection, Func<IIconProvider> iconProviderFactory )
        {
            serviceCollection.AddSingleton( ( p ) => iconProviderFactory() );

            return serviceCollection;
        }
    }
}
