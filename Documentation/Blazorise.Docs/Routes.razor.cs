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
        public const string Brand = "#9333ea";
        public const string BrandTextDark = "#c084fc";

        public const string Body = "#3f4d62";
        public const string BodyStrong = "#111827";
        public const string Muted = "#667085";

        public const string DarkBody = "#e5e7eb";
        public const string DarkBodyStrong = "#f8fafc";
        public const string DarkMuted = "#9aa4b2";
        public const string DarkSurface = "#1f232a";
        public const string DarkSurfaceElevated = "#151a22";
        public const string DarkTopBarItemBackground = "#262c35";

        public const string Secondary = "#d7dae7";
        public const string Success = "#13a668";
        public const string Danger = "#e11d48";
        public const string Warning = "#ffa800";
        public const string Info = "#48addb";
        public const string Light = "#dce7ed";
        public const string Dark = "#181c32";
        public const string TopBarActiveBackground = "#dedede";
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
                ActiveBackgroundColor = DocsThemeColors.TopBarActiveBackground,
                ActiveColor = DocsThemeColors.BodyStrong,
                HoverBackgroundColor = DocsThemeColors.TopBarActiveBackground,
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
                ActiveBackgroundColor = DocsThemeColors.TopBarActiveBackground,
                ActiveColor = DocsThemeColors.BodyStrong,
                HoverBackgroundColor = DocsThemeColors.TopBarActiveBackground,
                HoverColor = DocsThemeColors.BodyStrong,
            },
        },
        DarkColors = new ThemeBarColorOptions
        {
            BackgroundColor = DocsThemeColors.DarkSurfaceElevated,
            Color = DocsThemeColors.DarkBody,
            ItemColorOptions = new ThemeBarItemColorOptions
            {
                ActiveBackgroundColor = DocsThemeColors.DarkTopBarItemBackground,
                ActiveColor = DocsThemeColors.DarkBodyStrong,
                HoverBackgroundColor = DocsThemeColors.DarkTopBarItemBackground,
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