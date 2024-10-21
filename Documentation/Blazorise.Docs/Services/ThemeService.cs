using System;

namespace Blazorise.Docs.Services;

public class ThemeService
{
    const string DarkTheme = "Dark";
    const string LightTheme = "Light";
    const string SystemTheme = "System";

    public string CurrentTheme = LightTheme;

    public EventHandler<string> ThemeChanged;

    public Background BarBackground = Background.Light;
    public ThemeContrast BarThemeContrast = ThemeContrast.Light;

    public bool IsDark => CurrentTheme == DarkTheme;

    public void SetDarkTheme()
        => SetTheme( DarkTheme );

    public void SetLightTheme()
        => SetTheme( LightTheme );

    public void SetSystemTheme()
        => SetTheme( SystemTheme );

    public void SetTheme( string theme )
    {
        CurrentTheme = theme;

        if ( IsDark )
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
}
