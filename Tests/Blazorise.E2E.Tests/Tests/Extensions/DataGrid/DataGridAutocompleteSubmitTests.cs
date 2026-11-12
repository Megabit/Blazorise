namespace Blazorise.E2E.Tests.Tests.Extensions.DataGrid;

public class DataGridAutocompleteSubmitTests : BlazorisePageTest
{
    [TestCase( Blazorise.DataGrid.DataGridEditMode.Form )]
    [TestCase( Blazorise.DataGrid.DataGridEditMode.Inline )]
    [TestCase( Blazorise.DataGrid.DataGridEditMode.Popup )]
    [TestCase( Blazorise.DataGrid.DataGridEditMode.Cell )]
    public async Task EnterInAutocomplete_ShouldSelectOptionWithoutSubmittingForm( Blazorise.DataGrid.DataGridEditMode editMode )
    {
        await SelectTestComponent<DataGridAutocompleteSubmitComponent>();
        await Page.Locator( $"#mode-{editMode}" ).ClickAsync();

        if ( editMode == Blazorise.DataGrid.DataGridEditMode.Cell )
        {
            await Page.Locator( "#dataGrid tbody tr[data-row-index='0'] td" ).First.ClickAsync();
        }
        else
        {
            await Page.Locator( "#editButton" ).ClickAsync();
        }

        var autocomplete = Page.Locator( ".b-is-autocomplete input[type=search]" );

        await Expect( autocomplete ).ToBeVisibleAsync();

        await autocomplete.FillAsync( "Be" );
        await Expect( autocomplete ).ToBeFocusedAsync();

        await Expect( Page.Locator( ".dropdown-menu.show" ).GetByText( "Beta", new() { Exact = true } ) ).ToBeVisibleAsync();
        await autocomplete.PressAsync( "ArrowDown" );

        var pageInstance = await Page.Locator( "#pageInstance" ).InnerTextAsync();

        await autocomplete.PressAsync( "Enter" );

        await Expect( Page.Locator( "#selectedValue" ) ).ToHaveTextAsync( "beta" );
        await Expect( Page.Locator( "#pageInstance" ) ).ToHaveTextAsync( pageInstance );
    }
}