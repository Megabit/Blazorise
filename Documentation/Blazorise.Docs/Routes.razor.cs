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

    private static class DocsThemeColors
    {
        public const string Brand = "#7c3aed";
        public const string BrandTextDark = "#c4b5fd";

        public const string Body = "#475569";
        public const string BodyStrong = "#0f172a";
        public const string Muted = "#64748b";

        public const string DarkBody = "#e5e7eb";
        public const string DarkBodyStrong = "#f8fafc";
        public const string DarkMuted = "#94a3b8";
        public const string DarkSurface = "#1e293b";
        public const string DarkSurfaceElevated = "#273449";

        public const string Secondary = "#64748b";
        public const string Success = "#0f9f6e";
        public const string Danger = "#dc2626";
        public const string Warning = "#f59e0b";
        public const string Info = "#0284c7";
        public const string Light = "#f1f5f9";
        public const string Dark = "#1e293b";
        public const string BarItemActiveBackground = "#eef2f7";
        public const string BarItemHoverBackground = "#f5f7fb";
        public const string DarkBarItemActiveBackground = "#334155";
        public const string DarkBarItemHoverBackground = "#2b384b";
    }

    private readonly Theme lightTheme = new Theme
    {
        LuminanceThreshold = 170,
        BarOptions = CreateLightBarOptions(),
        ColorOptions = CreateColorOptions( DocsThemeColors.Body ),
        BackgroundOptions = CreateBackgroundOptions(),
        TextColorOptions = CreateLightTextColorOptions(),
        InputOptions = new ThemeInputOptions
        {
            CheckColor = DocsThemeColors.Brand,
            SliderColor = DocsThemeColors.Brand,
        },
        SpinKitOptions = new ThemeSpinKitOptions
        {
            Color = DocsThemeColors.Brand,
        },
    };

    private readonly Theme darkTheme = new Theme
    {
        LuminanceThreshold = 170,
        BodyOptions = new ThemeBodyOptions
        {
            BackgroundColor = DocsThemeColors.DarkSurface,
            TextColor = DocsThemeColors.DarkBody,
        },
        BarOptions = CreateDarkBarOptions(),
        ColorOptions = CreateColorOptions( DocsThemeColors.DarkBody ),
        BackgroundOptions = CreateBackgroundOptions(),
        TextColorOptions = CreateDarkTextColorOptions(),
        InputOptions = new ThemeInputOptions
        {
            CheckColor = DocsThemeColors.Brand,
            SliderColor = DocsThemeColors.Brand,
        },
        SpinKitOptions = new ThemeSpinKitOptions
        {
            Color = DocsThemeColors.Brand,
        },
    };

    private static ThemeColorOptions CreateColorOptions( string linkColor ) => new()
    {
        Primary = DocsThemeColors.Brand,
        Secondary = DocsThemeColors.Secondary,
        Success = DocsThemeColors.Success,
        Danger = DocsThemeColors.Danger,
        Warning = DocsThemeColors.Warning,
        Light = DocsThemeColors.Light,
        Dark = DocsThemeColors.Dark,
        Info = DocsThemeColors.Info,
        Link = linkColor,
    };

    private static ThemeBackgroundOptions CreateBackgroundOptions() => new()
    {
        Primary = DocsThemeColors.Brand,
        Secondary = DocsThemeColors.Secondary,
        Success = DocsThemeColors.Success,
        Danger = DocsThemeColors.Danger,
        Warning = DocsThemeColors.Warning,
        Light = DocsThemeColors.Light,
        Dark = DocsThemeColors.Dark,
        Info = DocsThemeColors.Info,
    };

    private static ThemeTextColorOptions CreateLightTextColorOptions() => new()
    {
        Primary = DocsThemeColors.Brand,
        Secondary = DocsThemeColors.Body,
        Success = DocsThemeColors.Success,
        Danger = DocsThemeColors.Danger,
        Warning = DocsThemeColors.Warning,
        Light = DocsThemeColors.Light,
        Dark = DocsThemeColors.BodyStrong,
        Info = DocsThemeColors.Info,
        Body = DocsThemeColors.Body,
        Muted = DocsThemeColors.Muted,
    };

    private static ThemeTextColorOptions CreateDarkTextColorOptions() => new()
    {
        Primary = DocsThemeColors.BrandTextDark,
        Secondary = DocsThemeColors.DarkBody,
        Success = DocsThemeColors.Success,
        Danger = DocsThemeColors.Danger,
        Warning = DocsThemeColors.Warning,
        Light = DocsThemeColors.DarkBody,
        Dark = DocsThemeColors.DarkBodyStrong,
        Info = DocsThemeColors.Info,
        Body = DocsThemeColors.DarkBody,
        Muted = DocsThemeColors.DarkMuted,
    };

    private static ThemeBarOptions CreateLightBarOptions() => new()
    {
        VerticalWidth = "280px",
        HorizontalHeight = "64px",
        VerticalBrandHeight = "64px",
        LightColors = new ThemeBarColorOptions
        {
            ItemColorOptions = new ThemeBarItemColorOptions
            {
                ActiveBackgroundColor = DocsThemeColors.BarItemActiveBackground,
                ActiveColor = DocsThemeColors.BodyStrong,
                HoverBackgroundColor = DocsThemeColors.BarItemHoverBackground,
                HoverColor = DocsThemeColors.BodyStrong,
            },
        },
    };

    private static ThemeBarOptions CreateDarkBarOptions() => new()
    {
        VerticalWidth = "280px",
        HorizontalHeight = "64px",
        VerticalBrandHeight = "64px",
        LightColors = new ThemeBarColorOptions
        {
            ItemColorOptions = new ThemeBarItemColorOptions
            {
                ActiveBackgroundColor = DocsThemeColors.BarItemActiveBackground,
                ActiveColor = DocsThemeColors.BodyStrong,
                HoverBackgroundColor = DocsThemeColors.BarItemHoverBackground,
                HoverColor = DocsThemeColors.BodyStrong,
            },
        },
        DarkColors = new ThemeBarColorOptions
        {
            BackgroundColor = DocsThemeColors.DarkSurfaceElevated,
            Color = DocsThemeColors.DarkBody,
            ItemColorOptions = new ThemeBarItemColorOptions
            {
                ActiveBackgroundColor = DocsThemeColors.DarkBarItemActiveBackground,
                ActiveColor = DocsThemeColors.DarkBodyStrong,
                HoverBackgroundColor = DocsThemeColors.DarkBarItemHoverBackground,
                HoverColor = DocsThemeColors.DarkBodyStrong,
            },
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