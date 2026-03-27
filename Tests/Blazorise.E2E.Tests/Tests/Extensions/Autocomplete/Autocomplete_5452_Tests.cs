namespace Blazorise.E2E.Tests.Tests.Extensions.Autocomplete;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class Autocomplete_5452_Tests : BlazorisePageTest
{
    [SetUp]
    public async Task Init()
    {
        await SelectTestComponent<Autocomplete_5452Component>();
    }

    [Test]
    public async Task TabbingOutWithOpenListFocusesNextElement()
    {
        var autocomplete = Page.Locator( ".b-is-autocomplete input[type=search]" );
        var dropdownMenu = Page.Locator( ".dropdown-menu.show" );
        var nextButton = Page.Locator( "#nextButton" );

        await autocomplete.ClickAsync();
        await dropdownMenu.WaitForAsync();

        await autocomplete.PressAsync( "Tab" );

        await Expect( nextButton ).ToBeFocusedAsync();
    }
}