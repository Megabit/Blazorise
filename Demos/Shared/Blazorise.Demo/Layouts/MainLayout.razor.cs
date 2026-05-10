using System.Threading.Tasks;
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

    protected Task OnThemeGradientChanged( bool value )
    {
        if ( Theme == null )
            return Task.CompletedTask;

        Theme.IsGradient = value;

        //if ( Theme.GradientOptions == null )
        //    Theme.GradientOptions = new GradientOptions();

        //Theme.GradientOptions.BlendPercentage = 80;

        Theme.ThemeHasChanged();

        return Task.CompletedTask;
    }

    protected Task OnThemeRoundedChanged( bool value )
    {
        if ( Theme == null )
            return Task.CompletedTask;

        Theme.IsRounded = value;

        Theme.ThemeHasChanged();

        return Task.CompletedTask;
    }

    protected Task OnThemeColorChanged( string value )
    {
        if ( Theme == null )
            return Task.CompletedTask;

        Theme.ColorOptions ??= new();

        Theme.BackgroundOptions ??= new();

        Theme.TextColorOptions ??= new();

        Theme.ColorOptions.Primary = value;
        Theme.BackgroundOptions.Primary = value;
        Theme.TextColorOptions.Primary = value;

        Theme.InputOptions ??= new();

        //Theme.InputOptions.Color = value;
        Theme.InputOptions.CheckColor = value;
        Theme.InputOptions.SliderColor = value;

        Theme.SpinKitOptions ??= new();

        Theme.SpinKitOptions.Color = value;

        Theme.ThemeHasChanged();

        return Task.CompletedTask;
    }

    [Inject] protected ITextLocalizerService LocalizationService { get; set; }

    [CascadingParameter] protected Theme Theme { get; set; }
}