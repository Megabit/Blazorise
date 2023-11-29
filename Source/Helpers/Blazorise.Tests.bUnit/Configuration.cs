#region Using directives
using Blazorise.Bootstrap;
using Blazorise.Licensing;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Utilities;
using Blazorise.Utilities.Vendors;
using Blazorise.Vendors;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
#endregion

namespace Blazorise.Tests.bUnit;

public static class Configuration
{
    public static TestServiceProvider AddBlazoriseTests( this TestServiceProvider services )
    {
        services.Replace( ServiceDescriptor.Transient<IComponentActivator, ComponentActivator>() );
        services.AddSingleton( new Mock<IComponentDisposer>().Object );
        services.AddSingleton<IIdGenerator>( new IdGenerator() );
        services.AddSingleton<IEditContextValidator>( sp => new EditContextValidator( new ValidationMessageLocalizerAttributeFinder(), sp ) );
        services.AddSingleton<IValidationHandlerFactory, ValidationHandlerFactory>();
        services.AddSingleton<ValidatorValidationHandler>();
        services.AddSingleton<PatternValidationHandler>();
        services.AddSingleton<DataAnnotationValidationHandler>();
        services.AddSingleton<IFlatPickrDateTimeDisplayFormatConverter, FlatPickrDateTimeDisplayFormatConverter>();
        services.AddSingleton<IInputMaskDateTimeInputFormatConverter, InputMaskDateTimeInputFormatConverter>();
        services.AddSingleton<IVersionProvider, MockVersionProvider>();
        services.AddScoped<ITextLocalizerService, TextLocalizerService>();
        services.AddScoped( typeof( ITextLocalizer<> ), typeof( TextLocalizer<> ) );

        // Shared component context. Must be defined as scoped as we want to make it available for the user session.
        services.AddScoped<IModalSharedContext, ModalSharedContext>();

        services.AddSingleton( sp => new BlazoriseOptions( sp, ( options ) => { } ) );

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
        services.AddScoped<IJSInputMaskModule, JSInputMaskModule>();
        services.AddScoped<IJSDropdownModule, JSDropdownModule>();
        services.AddScoped<IJSDragDropModule, JSDragDropModule>();

        services.AddScoped<IJSModalModule, Bootstrap.Modules.BootstrapJSModalModule>();
        services.AddScoped<IJSTooltipModule, Bootstrap.Modules.BootstrapJSTooltipModule>();

        services.AddScoped<BlazoriseLicenseProvider>();
        services.AddScoped<BlazoriseLicenseChecker>();
        return services;
    }

    public static TestServiceProvider AddBootstrapProvidersTests( this TestServiceProvider services )
    {
        services.AddSingleton<IClassProvider>( new BootstrapClassProvider() );
        services.AddSingleton<IStyleProvider>( new BootstrapStyleProvider() );
        services.AddSingleton<IBehaviourProvider>( new BootstrapBehaviourProvider() );
        services.AddSingleton<IThemeGenerator>( new BootstrapThemeGenerator( new Mock<IThemeCache>().Object ) );
        services.AddSingleton( new Mock<IIconProvider>().Object );
        return services;
    }
}