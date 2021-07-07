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

            Theme.ButtonOptions ??= new ThemeButtonOptions();

            Theme.DropdownOptions ??= new ThemeDropdownOptions();

            Theme.InputOptions ??= new ThemeInputOptions();

            Theme.PaginationOptions ??= new ThemePaginationOptions();

            Theme.ProgressOptions ??= new ThemeProgressOptions();

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
