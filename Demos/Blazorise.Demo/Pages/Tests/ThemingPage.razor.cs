using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Pages.Tests
{
    public partial class ThemingPage
    {
        private Size? selectedSize;

        private void OnThemeInputSizeChanged( Size? size )
        {
            selectedSize = size;

            if ( Theme == null )
                return;

            Theme.ButtonOptions ??= new();

            Theme.DropdownOptions ??= new();

            Theme.InputOptions ??= new();

            Theme.PaginationOptions ??= new();

            Theme.ProgressOptions ??= new();

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