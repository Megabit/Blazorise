using System;

namespace Blazorise.Docs.Services;

public class ThemeService
{
    public string CurrentTheme = "Light";

    public EventHandler<string> ThemeChanged;

    public void SetTheme( string theme )
    {
        CurrentTheme = theme;

        ThemeChanged?.Invoke( this, CurrentTheme );
    }
}
