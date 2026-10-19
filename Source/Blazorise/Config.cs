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
using Blazorise.Utilities.Vendors;
using Blazorise.Vendors;
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
        serviceCollection.AddSingleton<IInputMaskDateTimeInputFormatConverter, InputMaskDateTimeInputFormatConverter>();

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
    /// Registers a scoped theme and makes it available as a root-level cascading value.
    /// </summary>
    /// <param name="serviceCollection">The services to configure.</param>
    /// <param name="configureTheme">An optional callback to configure the theme for each scope.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// Register this in both server and client projects when using Interactive WebAssembly or Interactive Auto.
    /// Render a <see cref="ThemeProvider"/> to generate the theme CSS. Call <see cref="Theme.ThemeHasChanged"/> after changing theme settings.
    /// </remarks>
    public static IServiceCollection AddBlazoriseTheme( this IServiceCollection serviceCollection, Action<Theme> configureTheme = null )
    {
        return serviceCollection.AddBlazoriseTheme( _ =>
        {
            var theme = new Theme();
            configureTheme?.Invoke( theme );

            return theme;
        } );
    }

    /// <summary>
    /// Registers a scoped theme factory and makes its theme available as a root-level cascading value.
    /// </summary>
    /// <param name="serviceCollection">The services to configure.</param>
    /// <param name="themeFactory">A factory that creates the theme for each scope.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// The factory should create a separate theme for each scope so that user-specific settings remain isolated.
    /// Root-level registration does not transfer theme state between static rendering, server circuits, and WebAssembly.
    /// </remarks>
    public static IServiceCollection AddBlazoriseTheme( this IServiceCollection serviceCollection, Func<IServiceProvider, Theme> themeFactory )
    {
        ArgumentNullException.ThrowIfNull( serviceCollection );
        ArgumentNullException.ThrowIfNull( themeFactory );

        serviceCollection.AddScoped( themeFactory );
        serviceCollection.TryAddCascadingValue<Theme>( serviceProvider => new ThemeCascadingValueSource( serviceProvider.GetRequiredService<Theme>() ) );

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
        { typeof( IBreakpointService ), typeof( BreakpointService ) },
        { typeof( IDocumentObserver ), typeof( DocumentObserver ) },
        { typeof( IGestureService ), typeof( GestureService ) },
        { typeof( IModalService ), typeof( ModalService ) },
        { typeof( IToastService ), typeof( ToastService ) },
        { typeof( IDropdownCoordinator ), typeof( DropdownCoordinator ) },
        { typeof( IOffcanvasService ), typeof( OffcanvasService ) },
        { typeof( IOnScreenKeyboardService ), typeof( OnScreenKeyboardService ) },
        { typeof( IFontProvider ), typeof( FontProvider ) },
    };

    /// <summary>
    /// Gets the list of JS modules that are ready for DI registration.
    /// </summary>
    public static IDictionary<Type, Type> JSModuleMap => new Dictionary<Type, Type>
    {
        { typeof( IJSUtilitiesModule ), typeof( JSUtilitiesModule ) },
        { typeof( IJSFocusTrapModule ), typeof( JSFocusTrapModule ) },
        { typeof( IJSButtonModule ), typeof( JSButtonModule ) },
        { typeof( IJSClosableModule ), typeof( JSClosableModule ) },
        { typeof( IJSCollapseModule ), typeof( JSCollapseModule ) },
        { typeof( IJSBreakpointModule ), typeof( JSBreakpointModule ) },
        { typeof( IJSDocumentObserverModule ), typeof( JSDocumentObserverModule ) },
        { typeof( IJSGesturesModule ), typeof( JSGesturesModule ) },
        { typeof( IJSTextInputModule ), typeof( JSTextInputModule ) },
        { typeof( IJSRangeSliderModule ), typeof( JSRangeSliderModule ) },
        { typeof( IJSResizerModule ), typeof( JSResizerModule ) },
        { typeof( IJSMemoInputModule ), typeof( JSMemoInputModule ) },
        { typeof( IJSNumericPickerModule ), typeof( JSNumericPickerModule ) },
        { typeof( IJSFileInputModule ), typeof( JSFileInputModule ) },
        { typeof( IJSFilePickerModule ), typeof( JSFilePickerModule ) },
        { typeof( IJSFileModule ), typeof( JSFileModule ) },
        { typeof( IJSTableModule ), typeof( JSTableModule ) },
        { typeof( IJSTabsModule ), typeof( JSTabsModule ) },
        { typeof( IJSInputMaskModule ), typeof( JSInputMaskModule ) },
        { typeof( IJSDragDropModule ), typeof( JSDragDropModule ) },
        { typeof( IJSDropdownModule ), typeof( JSDropdownModule ) },
        { typeof( IJSContextMenuModule ), typeof( JSContextMenuModule ) },
        { typeof( IJSDockLayoutModule ), typeof( JSDockLayoutModule ) },
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