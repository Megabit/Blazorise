using Microsoft.AspNetCore.Components.WebView.Maui;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

namespace Blazorise.Demo.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts( fonts =>
            {
                fonts.AddFont( "OpenSans-Regular.ttf", "OpenSansRegular" );
            } );

        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        builder.Services
            .SetupDemoServices( builder.Configuration["Licensing:LicenseKey"] )
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();

        return builder.Build();
    }
}