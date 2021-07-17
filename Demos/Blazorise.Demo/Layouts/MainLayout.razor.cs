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

        private Bar sideBar;
        private Bar topBar;

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
        }

        [Inject] protected ITextLocalizerService LocalizationService { get; set; }

        [CascadingParameter] protected Theme Theme { get; set; }
    }
}
