#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Licensing;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Providers;
using Blazorise.Themes;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise;

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
        serviceCollection.AddScoped<IComponentActivator, ComponentActivator>();
        serviceCollection.AddScoped<IComponentDisposer, ComponentDisposer>();

        // Shared component context. Must be defined as scoped as we want to make it available for the user session.
        serviceCollection.AddScoped<IModalSharedContext, ModalSharedContext>();

        // If options handler is not defined we will get an exception so
        // we need to initialize an empty action.
        configureOptions ??= _ => { };

        serviceCollection.AddSingleton( configureOptions );
        serviceCollection.AddSingleton<BlazoriseOptions>();
        serviceCollection.AddSingleton<IVersionProvider, VersionProvider>();
        serviceCollection.AddSingleton<IIdGenerator, IdGenerator>();
        serviceCollection.AddSingleton<IThemeCache, ThemeCache>();
        serviceCollection.AddSingleton<IValidationMessageLocalizerAttributeFinder, ValidationMessageLocalizerAttributeFinder>();
        serviceCollection.AddSingleton<IDateTimeDisplayFormatConverter, DateTimeDisplayFormatConverter>();
        serviceCollection.AddSingleton<IDateTimeInputFormatConverter, DateTimeInputFormatConverter>();

        foreach ( var mapping in LocalizationMap
                     .Concat( ValidationMap )
                     .Concat( ServiceMap )
                     .Concat( JSModuleMap ) )
        {
            serviceCollection.AddScoped( mapping.Key, mapping.Value );
        }

        serviceCollection.AddScoped<BlazoriseLicenseProvider>();
        serviceCollection.AddScoped<BlazoriseLicenseChecker>();

        return serviceCollection;
    }

    /// <summary>
    /// Gets the list of localization services that are ready for DI registration.
    /// </summary>
    public static IDictionary<Type, Type> LocalizationMap => new Dictionary<Type, Type>
    {
        { typeof( ITextLocalizerService ), typeof( TextLocalizerService ) },
        { typeof( ITextLocalizer<> ), typeof( TextLocalizer<> ) },
    };

    /// <summary>
    /// Gets the list of validation handlers and services that are ready for DI registration.
    /// </summary>
    public static IDictionary<Type, Type> ValidationMap => new Dictionary<Type, Type>
    {
        { typeof( IEditContextValidator ), typeof( EditContextValidator ) },
        { typeof( IValidationHandlerFactory ), typeof( ValidationHandlerFactory ) },
        { typeof( ValidatorValidationHandler ), typeof( ValidatorValidationHandler ) },
        { typeof( PatternValidationHandler ), typeof( PatternValidationHandler ) },
        { typeof( DataAnnotationValidationHandler ), typeof( DataAnnotationValidationHandler ) },
    };

    /// <summary>
    /// Gets the list of services that are ready for DI registration.
    /// </summary>
    public static IDictionary<Type, Type> ServiceMap => new Dictionary<Type, Type>
    {
        { typeof( IMessageService ), typeof( MessageService ) },
        { typeof( INotificationService ), typeof( NotificationService ) },
        { typeof( IPageProgressService ), typeof( PageProgressService ) },
        { typeof( IModalService ), typeof( ModalService ) },
    };

    /// <summary>
    /// Gets the list of JS modules that are ready for DI registration.
    /// </summary>
    public static IDictionary<Type, Type> JSModuleMap => new Dictionary<Type, Type>
    {
        { typeof( IJSUtilitiesModule ), typeof( JSUtilitiesModule ) },
        { typeof( IJSButtonModule ), typeof( JSButtonModule ) },
        { typeof( IJSClosableModule ), typeof( JSClosableModule ) },
        { typeof( IJSBreakpointModule ), typeof( JSBreakpointModule ) },
        { typeof( IJSTextEditModule ), typeof( JSTextEditModule ) },
        { typeof( IJSMemoEditModule ), typeof( JSMemoEditModule ) },
        { typeof( IJSNumericPickerModule ), typeof( JSNumericPickerModule ) },
        { typeof( IJSDatePickerModule ), typeof( JSDatePickerModule ) },
        { typeof( IJSTimePickerModule ), typeof( JSTimePickerModule ) },
        { typeof( IJSColorPickerModule ), typeof( JSColorPickerModule ) },
        { typeof( IJSFileEditModule ), typeof( JSFileEditModule ) },
        { typeof( IJSFilePickerModule ), typeof( JSFilePickerModule ) },
        { typeof( IJSFileModule ), typeof( JSFileModule ) },
        { typeof( IJSTableModule ), typeof( JSTableModule ) },
        { typeof( IJSInputMaskModule ), typeof( JSInputMaskModule ) },
        { typeof( IJSDragDropModule ), typeof( JSDragDropModule ) },
        { typeof( IJSDropdownModule ), typeof( JSDropdownModule ) },
    };

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
        serviceCollection.AddSingleton<IBehaviourProvider, EmptyBehaviourProvider>();

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