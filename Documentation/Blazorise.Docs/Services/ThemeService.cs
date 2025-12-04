using System;

namespace Blazorise.Docs.Services;

public class ThemeService
{
    public const string DarkTheme = "Dark";
    public const string LightTheme = "Light";
    public const string SystemTheme = "System";

    public string CurrentTheme { get; private set; } = SystemTheme;
    public bool SystemIsDarkMode { get; private set; }

    public EventHandler<string> ThemeChanged;

    public Background BarBackground = Background.Light;
    public ThemeContrast BarThemeContrast = ThemeContrast.Light;

    public bool ShouldDark => IsDark || ( IsSystem && SystemIsDarkMode );

    public bool IsDark => CurrentTheme == DarkTheme;

    public bool IsLight => CurrentTheme == LightTheme;

    public bool IsSystem => CurrentTheme == SystemTheme;

    public void SetDarkTheme()
        => SetTheme( DarkTheme, SystemIsDarkMode );

    public void SetLightTheme()
        => SetTheme( LightTheme, SystemIsDarkMode );

    public void SetSystemTheme()
        => SetTheme( SystemTheme, SystemIsDarkMode );

    public void SetTheme( string theme, bool systemIsDarkMode )
    {
        CurrentTheme = NormalizeTheme( theme );
        SystemIsDarkMode = systemIsDarkMode;

        if ( ShouldDark )
        {
            BarBackground = Background.Dark;
            BarThemeContrast = ThemeContrast.Dark;
        }
        else
        {
            BarBackground = Background.Light;
            BarThemeContrast = ThemeContrast.Light;
        }

        ThemeChanged?.Invoke( this, CurrentTheme );
    }

    private static string NormalizeTheme( string theme )
        => theme switch
        {
            DarkTheme => DarkTheme,
            LightTheme => LightTheme,
            SystemTheme => SystemTheme,
            _ => SystemTheme
        };
}
