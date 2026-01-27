using System.Threading.Tasks;
using Blazorise;
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

    private readonly Theme lightTheme = new Theme
    {
        LuminanceThreshold = 170,
        BarOptions = new ThemeBarOptions
        {
            HorizontalHeight = "64px",
            VerticalBrandHeight = "64px",
            LightColors = new ThemeBarColorOptions
            {
                ItemColorOptions = new ThemeBarItemColorOptions
                {
                    ActiveBackgroundColor = "#dedede",
                    ActiveColor = "#000000",
                    HoverBackgroundColor = "#dedede",
                    HoverColor = "#000000",
                },
            },
        },
        ColorOptions = new ThemeColorOptions
        {
            Primary = "#9333ea",
            Secondary = "#d7dae7",
            Success = "#13a668",
            Danger = "#e11d48",
            Warning = "#FFA800",
            Light = "#dce7ed",
            Dark = "#181C32",
            Info = "#48addb",
        },
        BackgroundOptions = new ThemeBackgroundOptions
        {
            Primary = "#9333ea",
            Secondary = "#d7dae7",
            Success = "#13a668",
            Danger = "#e11d48",
            Warning = "#FFA800",
            Light = "#dce7ed",
            Dark = "#181C32",
            Info = "#48addb",
        },
        TextColorOptions = new ThemeTextColorOptions
        {
            Primary = "#9333ea",
            Secondary = "#d7dae7",
            Success = "#13a668",
            Danger = "#e11d48",
            Warning = "#FFA800",
            Light = "#dce7ed",
            Dark = "#181C32",
            Info = "#48addb",
        },
        InputOptions = new ThemeInputOptions
        {
            CheckColor = "#9333ea",
        },
    };

    private readonly Theme darkTheme = new Theme
    {
        LuminanceThreshold = 170,
        BodyOptions = new ThemeBodyOptions
        {
            BackgroundColor = "#1f232a",
            TextColor = "#e5e7eb",
        },
        BarOptions = new ThemeBarOptions
        {
            HorizontalHeight = "64px",
            VerticalBrandHeight = "64px",
            LightColors = new ThemeBarColorOptions
            {
                ItemColorOptions = new ThemeBarItemColorOptions
                {
                    ActiveBackgroundColor = "#dedede",
                    ActiveColor = "#000000",
                    HoverBackgroundColor = "#dedede",
                    HoverColor = "#000000",
                },
            },
            DarkColors = new ThemeBarColorOptions
            {
                BackgroundColor = "#151a22",
                Color = "#e5e7eb",
                ItemColorOptions = new ThemeBarItemColorOptions
                {
                    ActiveBackgroundColor = "#262c35",
                    ActiveColor = "#f8fafc",
                    HoverBackgroundColor = "#262c35",
                    HoverColor = "#f8fafc",
                },
            },
        },
        ColorOptions = new ThemeColorOptions
        {
            Primary = "#9333ea",
            Secondary = "#d7dae7",
            Success = "#13a668",
            Danger = "#e11d48",
            Warning = "#FFA800",
            Light = "#dce7ed",
            Dark = "#181C32",
            Info = "#48addb",
        },
        BackgroundOptions = new ThemeBackgroundOptions
        {
            Primary = "#9333ea",
            Secondary = "#d7dae7",
            Success = "#13a668",
            Danger = "#e11d48",
            Warning = "#FFA800",
            Light = "#dce7ed",
            Dark = "#181C32",
            Info = "#48addb",
        },
        TextColorOptions = new ThemeTextColorOptions
        {
            Primary = "#9333ea",
            Secondary = "#d7dae7",
            Success = "#13a668",
            Danger = "#e11d48",
            Warning = "#FFA800",
            Light = "#dce7ed",
            Dark = "#181C32",
            Info = "#48addb",
            Muted = "#9aa4b2",
        },
        InputOptions = new ThemeInputOptions
        {
            CheckColor = "#9333ea",
        },
    };

    private Theme ActiveTheme => ThemeService.ShouldDark ? darkTheme : lightTheme;
    private Theme theme => ActiveTheme;

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