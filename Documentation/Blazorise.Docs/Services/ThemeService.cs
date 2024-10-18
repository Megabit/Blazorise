using System;

namespace Blazorise.Docs.Services;

public class ThemeService
{
    public string CurrentTheme = "Light";

    public EventHandler<string> ThemeChanged;

    public Background BarBackground = Background.Light;
    public ThemeContrast BarThemeContrast = ThemeContrast.Light;

    public bool IsDark => CurrentTheme == "Dark";

    public void SetTheme( string theme )
    {
        CurrentTheme = theme;

        if ( CurrentTheme == "Dark" )
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
