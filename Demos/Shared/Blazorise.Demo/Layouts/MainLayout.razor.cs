using System.Threading.Tasks;
using Blazorise.Demo.Components;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Layouts;

public partial class MainLayout
{
    protected string layoutType = "fixed-header";

    protected override async Task OnInitializedAsync()
    {
        await SelectCulture( "en-US" );

        await base.OnInitializedAsync();
    }

    private Task SelectCulture( string name )
    {
        LocalizationService.ChangeLanguage( name );

        return Task.CompletedTask;
    }

    protected Task OnThemeEnabledChanged( bool value )
    {
        if ( Theme == null )
            return Task.CompletedTask;

        Theme.Enabled = value;

        Theme.ThemeHasChanged();

        return Task.CompletedTask;
    }

    protected Task OnThemeColorChanged( ThemeColorChangedEventArgs eventArgs )
    {
        if ( Theme == null )
            return Task.CompletedTask;

        Theme.ColorOptions ??= new();

        Theme.BackgroundOptions ??= new();

        Theme.TextColorOptions ??= new();

        ApplyThemeColor( Theme, eventArgs );

        if ( eventArgs.Variant == Color.Primary )
        {
            Theme.InputOptions ??= new();
            Theme.InputOptions.CheckColor = eventArgs.Value;
            Theme.InputOptions.SliderColor = eventArgs.Value;

            Theme.SpinKitOptions ??= new();
            Theme.SpinKitOptions.Color = eventArgs.Value;
        }

        Theme.ThemeHasChanged();

        return Task.CompletedTask;
    }

    private static void ApplyThemeColor( Theme theme, ThemeColorChangedEventArgs eventArgs )
    {
        switch ( eventArgs.Variant.Name )
        {
            case "primary":
                theme.ColorOptions.Primary = eventArgs.Value;
                theme.BackgroundOptions.Primary = eventArgs.Value;
                theme.TextColorOptions.Primary = eventArgs.Value;
                break;
            case "secondary":
                theme.ColorOptions.Secondary = eventArgs.Value;
                theme.BackgroundOptions.Secondary = eventArgs.Value;
                theme.TextColorOptions.Secondary = eventArgs.Value;
                break;
            case "success":
                theme.ColorOptions.Success = eventArgs.Value;
                theme.BackgroundOptions.Success = eventArgs.Value;
                theme.TextColorOptions.Success = eventArgs.Value;
                break;
            case "danger":
                theme.ColorOptions.Danger = eventArgs.Value;
                theme.BackgroundOptions.Danger = eventArgs.Value;
                theme.TextColorOptions.Danger = eventArgs.Value;
                break;
            case "warning":
                theme.ColorOptions.Warning = eventArgs.Value;
                theme.BackgroundOptions.Warning = eventArgs.Value;
                theme.TextColorOptions.Warning = eventArgs.Value;
                break;
            case "info":
                theme.ColorOptions.Info = eventArgs.Value;
                theme.BackgroundOptions.Info = eventArgs.Value;
                theme.TextColorOptions.Info = eventArgs.Value;
                break;
            case "light":
                theme.ColorOptions.Light = eventArgs.Value;
                theme.BackgroundOptions.Light = eventArgs.Value;
                theme.TextColorOptions.Light = eventArgs.Value;
                break;
            case "dark":
                theme.ColorOptions.Dark = eventArgs.Value;
                theme.BackgroundOptions.Dark = eventArgs.Value;
                theme.TextColorOptions.Dark = eventArgs.Value;
                break;
        }
    }

    [Inject] protected ITextLocalizerService LocalizationService { get; set; }

    [CascadingParameter] protected Theme Theme { get; set; }
}