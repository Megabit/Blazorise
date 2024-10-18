using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Docs.Services;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Docs.Layouts;

public class BaseDocsLayout : LayoutComponentBase, IDisposable
{
    [Inject] private ThemeService ThemeService { get; set; }
    [Inject] private IJSUtilitiesModule JSUtilitiesModule { get; set; }

    protected Background BarBackground = Background.Light;
    protected ThemeContrast BarThemeContrast = ThemeContrast.Light;

    protected override void OnInitialized()
    {
        if ( ThemeService?.CurrentTheme == "Dark" )
        {
            BarBackground = Background.Dark;
            BarThemeContrast = ThemeContrast.Dark;
        }
        else
        {
            BarBackground = Background.Light;
            BarThemeContrast = ThemeContrast.Light;
        }

        ThemeService.ThemeChanged += OnThemeChanged;

        base.OnInitialized();
    }

    public void Dispose()
    {
        ThemeService.ThemeChanged -= OnThemeChanged;
    }

    async void OnThemeChanged( object sender, string theme )
    {
        if ( theme == "Dark" )
        {
            BarBackground = Background.Dark;
            BarThemeContrast = ThemeContrast.Dark;

            await JSUtilitiesModule.AddAttributeToBody( "data-bs-theme", "dark" );
        }
        else
        {
            BarBackground = Background.Light;
            BarThemeContrast = ThemeContrast.Light;

            await JSUtilitiesModule.RemoveAttributeFromBody( "data-bs-theme" );
        }

        await InvokeAsync( StateHasChanged );
    }
}
