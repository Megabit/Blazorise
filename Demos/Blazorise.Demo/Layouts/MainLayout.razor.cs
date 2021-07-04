using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Layouts
{
    public partial class MainLayout
    {
        private bool topbarVisible = false;
        private bool uiElementsVisible = true;
        private bool utilitiesVisible = false;

        protected override async Task OnInitializedAsync()
        {
            await SelectCulture( "en-US" );

            await base.OnInitializedAsync();
        }

        Task SelectCulture( string name )
        {
            LocalizationService.ChangeLanguage( name );

            return Task.CompletedTask;
        }

        void OnThemeEnabledChanged( bool value )
        {
            if ( Theme == null )
                return;

            Theme.Enabled = value;

            Theme.ThemeHasChanged();
        }

        void OnGradientChanged( bool value )
        {
            if ( Theme == null )
                return;

            Theme.IsGradient = value;

            //if ( Theme.GradientOptions == null )
            //    Theme.GradientOptions = new GradientOptions();

            //Theme.GradientOptions.BlendPercentage = 80;

            Theme.ThemeHasChanged();
        }

        void OnRoundedChanged( bool value )
        {
            if ( Theme == null )
                return;

            Theme.IsRounded = value;

            Theme.ThemeHasChanged();
        }

        void OnThemeColorChanged( string value )
        {
            if ( Theme == null )
                return;

            if ( Theme.ColorOptions == null )
                Theme.ColorOptions = new ThemeColorOptions();

            if ( Theme.BackgroundOptions == null )
                Theme.BackgroundOptions = new ThemeBackgroundOptions();

            if ( Theme.TextColorOptions == null )
                Theme.TextColorOptions = new ThemeTextColorOptions();

            Theme.ColorOptions.Primary = value;
            Theme.BackgroundOptions.Primary = value;
            Theme.TextColorOptions.Primary = value;

            if ( Theme.InputOptions == null )
                Theme.InputOptions = new ThemeInputOptions();

            //Theme.InputOptions.Color = value;
            Theme.InputOptions.CheckColor = value;
            Theme.InputOptions.SliderColor = value;

            if ( Theme.SpinKitOptions == null )
                Theme.SpinKitOptions = new ThemeSpinKitOptions();

            Theme.SpinKitOptions.Color = value;

            Theme.ThemeHasChanged();
        }

        [Inject] protected ITextLocalizerService LocalizationService { get; set; }

        [CascadingParameter] protected Theme Theme { get; set; }
    }
}
