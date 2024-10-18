using Blazored.LocalStorage;
using Blazorise.Docs.Services;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Docs;

public partial class Routes
{
    [Inject] private ThemeService ThemeService { get; set; }
    [Inject] private IJSUtilitiesModule JSUtilitiesModule { get; set; }
    [Inject] ILocalStorageService LocalStorage { get; set; }

    protected override void OnInitialized()
    {
        ThemeService.ThemeChanged += OnThemeChanged;

        base.OnInitialized();
    }

    protected override async void OnAfterRender( bool firstRender )
    {
        if ( firstRender )
        {
            var theme = await LocalStorage.GetItemAsync<string>( "theme" );

            if ( ThemeService.CurrentTheme != theme )
            {
                ThemeService.SetTheme( theme );
            }
        }

        base.OnAfterRender( firstRender );
    }

    public void Dispose()
    {
        ThemeService.ThemeChanged -= OnThemeChanged;
    }

    async void OnThemeChanged( object sender, string theme )
    {
        if ( theme == "Dark" )
        {
            await JSUtilitiesModule.AddAttributeToBody( "data-bs-theme", "dark" );
        }
        else
        {
            await JSUtilitiesModule.RemoveAttributeFromBody( "data-bs-theme" );
        }

        await LocalStorage.SetItemAsync( "theme", theme );

        await InvokeAsync( StateHasChanged );
    }
}
