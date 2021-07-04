using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Pages.Tests
{
    public partial class ThemingPage
    {
        private Size? selectedSize;

        void OnThemeInputSizeChanged( Size? size )
        {
            selectedSize = size;

            if ( Theme == null )
                return;

            if ( Theme.ButtonOptions == null )
                Theme.ButtonOptions = new ThemeButtonOptions();

            if ( Theme.DropdownOptions == null )
                Theme.DropdownOptions = new ThemeDropdownOptions();

            if ( Theme.InputOptions == null )
                Theme.InputOptions = new ThemeInputOptions();

            if ( Theme.PaginationOptions == null )
                Theme.PaginationOptions = new ThemePaginationOptions();

            if ( Theme.ProgressOptions == null )
                Theme.ProgressOptions = new ThemeProgressOptions();

            Theme.ButtonOptions.Size = size;
            Theme.DropdownOptions.Size = size;
            Theme.InputOptions.Size = size;
            Theme.PaginationOptions.Size = size;
            Theme.ProgressOptions.Size = size;

            Theme.ThemeHasChanged();
        }

        [CascadingParameter] protected Theme Theme { get; set; }
    }
}
