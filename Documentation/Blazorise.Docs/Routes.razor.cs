using System.Threading.Tasks;
using Blazorise.Docs.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace Blazorise.Docs;

public partial class Routes
{
    private const string ThemeCookieName = "bdocs-theme";
    private const string SystemThemeCookieName = "bdocs-system";
    private bool jsReady;

    [Inject] private ThemeService ThemeService { get; set; }
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }

    protected override void OnInitialized()
    {
        ThemeService.ThemeChanged += OnThemeChanged;

        TryLoadThemeFromCookie();

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            var theme = await JSRuntime.InvokeAsync<string>( "blazoriseDocs.theme.getStoredTheme" );
            var systemIsDarkMode = await JSRuntime.InvokeAsync<bool>( "blazoriseDocs.theme.isSystemDark" );

            jsReady = true;

            ThemeService.SetTheme( theme, systemIsDarkMode );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    public void Dispose()
    {
        ThemeService.ThemeChanged -= OnThemeChanged;
    }

    async void OnThemeChanged( object sender, string theme )
    {
        if ( !jsReady )
            return;

        await JSRuntime.InvokeVoidAsync( "blazoriseDocs.theme.save", theme, ThemeService.SystemIsDarkMode );

        await InvokeAsync( StateHasChanged );
    }

    private void TryLoadThemeFromCookie()
    {
        var httpContext = HttpContextAccessor.HttpContext;

        if ( httpContext is null )
            return;

        if ( httpContext.Request.Cookies.TryGetValue( ThemeCookieName, out var themeFromCookie ) )
        {
            var systemIsDarkMode = httpContext.Request.Cookies.TryGetValue( SystemThemeCookieName, out var systemCookie )
                && bool.TryParse( systemCookie, out var parsedSystemDark )
                && parsedSystemDark;

            ThemeService.SetTheme( themeFromCookie, systemIsDarkMode );
        }
    }
}
