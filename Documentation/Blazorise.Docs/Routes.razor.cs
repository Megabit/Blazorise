using System.Threading.Tasks;
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

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            var theme = await LocalStorage.GetItemAsync<string>( "theme" );
            var systemIsDarkMode = await JSUtilitiesModule.IsDarkMode();

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
        if ( ThemeService.ShouldDark )
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
