namespace Blazorise.E2E.Tests.Tests.Components.Dropdown;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class DropdownTests : BlazorisePageTest
{
    [Test]
    public async Task CanShowAndHideDropdownComponent()
    {
        await SelectTestComponent<DropdownComponent>();

        var button = Page.GetByRole( AriaRole.Button );
        var dropdown = Page.Locator( ".dropdown" );
        var dropdownMenu = Page.Locator( ".dropdown-menu" );

        await button.ClickAsync();
        await Expect( dropdown ).ToHaveClassAsync( "dropdown show" );
        await Expect( dropdownMenu ).ToHaveClassAsync( "dropdown-menu show" );

        await button.ClickAsync();
        await Expect( dropdown ).Not.ToHaveClassAsync( "dropdown show" );
        await Expect( dropdownMenu ).Not.ToHaveClassAsync( "dropdown-menu show" );

    }
}
